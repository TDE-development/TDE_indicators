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
    [DisplayName("TDE_dev")]
    public class TDE_dev : Indicator
    {
        #region Fields
        private int _vmulti = 1;  // default = 1 ...

        public DateTime b1time;
        public DateTime b0time;
        public DateTime AlertTime = DateTime.UtcNow.AddMinutes(-10);

        #endregion
        //--------------

        #region Settings

        [Display(Name = "ValueMultiplicator", GroupName = "Settings", Order = 10)]
        [Range(1, 10)]
        public int vMulti
        {
            get => _vmulti;
            set
            {
                if (_vmulti == value)
                    return;

                if (value <= 0)
                    return;

                _vmulti = value;

                RaisePropertyChanged(nameof(vMulti));
                RecalculateValues();
            }
        }

        [Display(Name = "UseAlerts", GroupName = "Settings", Order = 90)]
        public bool UseAlerts { get; set; }
        #endregion
        //--------

        #region DataSeries

        private ValueDataSeries _Xlong_Series = new("Xlong_Series", "X_long")
        {
            //Color = DefaultColors.Lime.Convert(),
            Color = System.Drawing.Color.FromArgb(255, 110, 142, 44).Convert(),  //-- FF6E8E2C  green
            VisualType = VisualMode.Histogram
        };

        private ValueDataSeries _Xshort_Series = new("Xshort_Series", "X_short")
        {
            Color = System.Drawing.Color.FromArgb(255, 229, 117, 114).Convert(),  //-- FFE57572  red
            VisualType = VisualMode.Histogram
        };

        private ValueDataSeries _Xdebug_Series = new("Xdebug_Series", "X_debug")
        {
            Color = System.Drawing.Color.FromArgb(255, 229, 117, 114).Convert(),  //-- FFE57572  red
            VisualType = VisualMode.Line
        };

        #endregion
        //--------
        public TDE_dev()
        {
            Panel = IndicatorDataProvider.NewPanel;

            ((ValueDataSeries)DataSeries[0]).VisualType = VisualMode.Hide; //-- 4 returnValues 

            DataSeries.Add(_Xlong_Series);
            DataSeries.Add(_Xshort_Series);
            DataSeries.Add(_Xdebug_Series);
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
            decimal timeFrame = Convert.ToDecimal(ChartInfo.TimeFrame);
            decimal tickSize  = InstrumentInfo.TickSize;

            decimal b4open  = 0;
            decimal b4close = 0;

            decimal b3open  = 0;
            decimal b3close = 0;

            decimal b2open  = 0;
            decimal b2close = 0;

            decimal b1open  = 0;
            decimal b1close = 0;

            decimal b0open  = 0;
            decimal b0close = 0;
          //decimal b0ticks = 0; // Environment.TickCount;

            var cAbove = 0;
            var cBelow = 0;

            if (bar > 9)   //-- avoid "Index was out of range.Error"
            {
                b4open  = GetCandle(bar - 4).Open;
                b4close = GetCandle(bar - 4).Close;

                b3open  = GetCandle(bar - 3).Open;
                b3close = GetCandle(bar - 3).Close;

                b2open  = GetCandle(bar - 2).Open;
                b2close = GetCandle(bar - 2).Close;

                b1open  = GetCandle(bar - 1).Open;
                b1close = GetCandle(bar - 1).Close;
                b1time  = GetCandle(bar - 1).Time;

                b0open  = GetCandle(bar - 0).Open;
                b0close = GetCandle(bar - 0).Close;
                b0time  = GetCandle(bar - 0).Time;
            }

            //----


            // bull  
            if       (b4open  > b4close &&    // red Candle
                      b3open  > b3close &&    // red Candle
                      b2open  > b2close &&    // red Candle
                      b1open  < b1close &&    // green Candle
                      b0open  < b0close &&    // green Candle

                      b2open  < b1close &&    // engulfing top
                      b2close > b1open  &&    // engulfing bottom

                      b0close - b0open == (timeFrame * tickSize)  //1  // b0 = 5steps == 1 Pkt                
                )
            {
                cAbove = (+1 * vMulti);
                cBelow = 0;
            }
            // bear  
            else if  (b4open  < b4close &&    // green Candle
                      b3open  < b3close &&    // green Candle
                      b2open  < b2close &&    // green Candle
                      b1open  > b1close &&    // red Candle
                      b0open  > b0close &&    // red Candle

                      b2close < b1open &&     // engulfing top  = identisch
                      b2open  > b1close &&    // engulfing bottom

                      b0open  - b0close == (timeFrame * tickSize)  // b0 = 5steps == 1 Pkt                
                    )
            {
                cAbove = 0;
                cBelow = (-1 * vMulti);
            }
            // ....       
            else
            {
                cAbove = 0;
                cBelow = 0;
            }

            // alert !!
            if (UseAlerts && AlertTime < b0time && (cAbove >= 1 || cBelow <= -1))
            {
                if (b0open < b0close)  // UP
                {
                    AddAlert("alert1", "alert UP >> " + this.GetType().Name + " <<");
                    AlertTime = DateTime.UtcNow;
                }
                else  // DOWN
                {
                    AddAlert("alert1", "alert DOWN >> " + this.GetType().Name + " <<");
                    AlertTime = DateTime.UtcNow;
                }
            }

            // return
            this[bar]  = cAbove + cBelow;  // the hidden ValueDataSeries
            _Xlong_Series[bar]  = cAbove;
            _Xshort_Series[bar] = cBelow;

            //_Xdebug_Series[bar] = Math.Abs(b0close - b0open);

        }
    }
}

//----
