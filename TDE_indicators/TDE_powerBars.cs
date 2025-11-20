namespace ATAS.Indicators.TDE_indicators
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using ATAS.Indicators;        //-- <HintPath> ..\..\..\..\Program Files(x86)\ATAS Platform\ATAS.Indicators.dll
    using ATAS.Indicators.Drawing;
    using Microsoft.VisualBasic;
    using Utils.Common.Attributes;
    using Utils.Common.Logging;   //-- <HintPath> ..\..\..\..\Program Files(x86)\ATAS Platform\Utils.Common.dll


    [Category("TDE_indicators")]
    [DisplayName("TDE_powerBars")]
    public class TDE_powerBars : Indicator
    {
        #region Fields
        private int _minSeconds;

        private int _barNumber;
        private int _barLine_offset;
        private int _lviBar;  // LastVisibleBarNumber
        private int _xByBar;
        private int _yByBar;

        public decimal b2open;
        public decimal b2high;
        public decimal b2low;
        public decimal b2close;
        public decimal HA2open;
        public decimal HA2close;

        public decimal b1open;
        public decimal b1high;
        public decimal b1low;
        public decimal b1close;
        public decimal HA1open;
        public decimal HA1close;

        public decimal b0open;
        public decimal b0high;
        public decimal b0low;
        public decimal b0close;
        public decimal HA0open;
        public decimal HA0close;

        public decimal b0maxVol_Pi__Price;
        public decimal b0maxVol_Pi__Volume;

        public DateTime candleTime;
        public DateTime candleLast;
        public TimeSpan difference;
        public double   diffInSec;
        public string   diffTimeOK;

        public string TradeDir;    //-- [ LONG | SHORT | ---- ]

        #endregion
        //--------------

        #region Settings

        [Display(Name = "Bar min. Time (sec)", GroupName = "Settings", Order = 10)]
        [Range(0, 30)]
        public int MinSeconds
        {
            get => _minSeconds;
            set
            {
                if (_minSeconds == value)
                    return;

                if (value <= 0)
                    return;

                _minSeconds = value;

                RaisePropertyChanged(nameof(MinSeconds));
                RecalculateValues();
            }
        }

        #endregion
        //--------

        #region DataSeries

        private ValueDataSeries _Xlong_Series = new("Xlong_Series", "X_long")
        {
            Color = System.Drawing.Color.FromArgb(255, 116, 166, 226).Convert(),   //-- FF74A6E2  blue
            VisualType = VisualMode.Histogram
        };

        private ValueDataSeries _Xshort_Series = new("Xshort_Series", "X_short")
        {
            Color = System.Drawing.Color.FromArgb(255, 229, 117, 114).Convert(),   //-- FFE57572  red
            VisualType = VisualMode.Histogram
        };

        #endregion
        //--------
        public TDE_powerBars()
        {
            Panel = IndicatorDataProvider.NewPanel;

            ((ValueDataSeries)DataSeries[0]).VisualType = VisualMode.Hide; //-- 4 returnValues 

            DataSeries.Add(_Xlong_Series);
            DataSeries.Add(_Xshort_Series);
        }

        protected override void OnInitialize()
        {
            this.LogInfo($"Indicator: " + this.GetType().Name + "  added.");
        }
        protected override void OnRecalculate()
        {
            DataSeries.ForEach(x => x.Clear());
        }
        protected override void OnCalculate(int bar, decimal value)
        {
            var cAbove = 0;
            var cBelow = 0;

            if (bar > 20)    //-- avoid "Index was out of range.Error"
            {

                IndicatorCandle candleC_2 = GetCandle(bar - 2);
                IndicatorCandle candleC_1 = GetCandle(bar - 1);
                IndicatorCandle candleC_0 = GetCandle(bar - 0);

                b2open  = candleC_2.Open;
                b2high  = candleC_2.High;
                b2low   = candleC_2.Low;
                b2close = candleC_2.Close;
                HA2open   = (b2open + b2close) / 2;
                HA2close  = (b2open + b2high + b2low + b2close) / 4;
                //-----                                                 
                b1open  = candleC_1.Open;
                b1high  = candleC_1.High;
                b1low   = candleC_1.Low;
                b1close = candleC_1.Close;
                HA1open   = (HA2open + HA2close) / 2;
                HA1close  = (b1open + b1high + b1low + b1close) / 4;
                //-----                                                 
                b0open  = candleC_0.Open;
                b0high  = candleC_0.High;
                b0low   = candleC_0.Low;
                b0close = candleC_0.Close;
                HA0open   = (HA1open + HA1close) / 2;
                HA0close  = (b0open + b0high + b0low + b0close) / 4;

                //-- POC
                PriceVolumeInfo maxVol_Pi = candleC_0.MaxVolumePriceInfo;
                b0maxVol_Pi__Price  = maxVol_Pi.Price;
                b0maxVol_Pi__Volume = maxVol_Pi.Volume;

                //-- Duration
                candleTime = GetCandle(bar).Time;
                candleLast = GetCandle(bar).LastTime;

                difference = candleLast - candleTime;
                diffInSec  = difference.TotalSeconds;    //-- difference in Seconds

                //------------------------------------------------------------------

                //---- long

                if       (b0open    < b0close                 &&   //-- green
                          HA0open   < HA0close                &&   //-- green
                          HA0open   < b0low                   &&   //-- buyPower
                          HA0open   < b0maxVol_Pi__Price      &&   //-- POC 
                          HA0close  > b0maxVol_Pi__Price      &&   //-- POC
                          diffInSec > 5.0                          //-- 5 sec
                        )
                {

                    cAbove =  +1;
                    cBelow =   0;
                }
                //---- short
                else if ( b0open    > b0close                 &&   //-- red
                          HA0open   > HA0close                &&   //-- red
                          HA0open   > b0high                  &&   //-- sellPower
                          HA0open   > b0maxVol_Pi__Price      &&   //-- POC
                          HA0close  < b0maxVol_Pi__Price      &&   //-- POC
                          diffInSec > 5.0                          //-- 5 sec
                        )
                {
                    cAbove =   0;
                    cBelow =  -1;
                }
                //----
                else
                {
                    cAbove =   0;
                    cBelow =   0;
                }

            }
            //----

            // return
            this[bar]  = cAbove + cBelow;  // the hidden ValueDataSeries
            _Xlong_Series[bar]  = cAbove;
            _Xshort_Series[bar] = cBelow;

        }
    }
}

//----
