namespace ATAS.Indicators.IndexPerformance
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Drawing;
    using System.Reflection.Metadata;
    using ATAS.Indicators;        //-- <HintPath> ..\..\..\..\Program Files(x86)\ATAS Platform\ATAS.Indicators.dll
    using ATAS.Indicators.Drawing;
    using Microsoft.VisualBasic;
    using OFT.Rendering.Context;
    using OFT.Rendering.Control;
    using OFT.Rendering.Tools;
    using Utils.Common.Attributes;
    using Utils.Common.Logging;   //-- <HintPath> ..\..\..\..\Program Files(x86)\ATAS Platform\Utils.Common.dll

 // using BiasDetermination;  //-- C:\Users\ {userName} \AppData\Roaming\ATAS\Indicators\2a0cfe1b-251d-4783-9b14-6134af72643b.dll


    [Category("TDE_indicators")]
    [DisplayName("TDE_infoBox_priceEMAs")]

    public class TDE_infoBox_priceEMAs : Indicator
    {
        #region Fields
        private int _barNumber;

        private readonly Technical.EMA  _ema1 = new();  //-- fast
        private readonly Technical.EMA  _ema2 = new();  //-- slow
        private readonly Technical.EMA  _ema3 = new();
     
     // private readonly BiasDetermination.BiasDeterminationTool _bias = new();

        public decimal bXopen;
        public decimal bXhigh;
        public decimal bXlow;
        public decimal bXclose;

        public decimal bEMA1;  //-- fast - EMA value
        public decimal bEMA2;  //-- slow - EMA value 

        public decimal bBias;

        #endregion

        #region Settings

        [Display(Name = "Bar Number", GroupName = "Settings", Order = 10)]
        [Range(-1, 20)]
        public int BarNumber
        {
            get => _barNumber;
            set
            {
                if (_barNumber == value)
                    return;

                if (value < 0)  // O = curr.Bar
                    return;

                _barNumber = value;

                RaisePropertyChanged(nameof(_barNumber));
                RecalculateValues();
            }
        }

        [Display(Name = "fast - EMA Period", GroupName = "EMA Settings", Order = 110)]
        [Range(1, 10000)]
        public int fastPeriod
        {
            get => _ema1.Period;
            set
            {
                if (_ema1.Period == value)
                    return;

                if (value < 0)  // O = curr.Bar
                    return;

                _ema1.Period = value;

                RaisePropertyChanged(nameof(_ema1.Period));
                RecalculateValues();
            }
        }

        [Display(Name = "slow - EMA Period", GroupName = "EMA Settings", Order = 120)]
        [Range(1, 10000)]
        public int slowPeriod
        {
            get => _ema2.Period;
            set
            {
                if (_ema2.Period == value)
                    return;

                if (value < 0)  // O = curr.Bar
                    return;

                _ema2.Period = value;

                RaisePropertyChanged(nameof(_ema2.Period));
                RecalculateValues();
            }
        }

        [Display(Name = "Darkmode", GroupName = "Settings", Order = 900)]
        public bool Darkmode { get; set; }
        #endregion
        //--------
        public TDE_infoBox_priceEMAs()
        {
            EnableCustomDrawing = true;
            SubscribeToDrawingEvents(DrawingLayouts.Final);

            _ema1.Period   = 10;
            _ema2.Period   = 20;

         // _bias.

            
        }

        protected override void OnInitialize()
        {
            this.LogInfo($"Indicator: " + this.GetType().Name + "  added.");
        }
        protected override void OnRender(RenderContext context, DrawingLayouts layout)
        {
            var colorLine = RenderPens.OrangeRed;
            var colorText = System.Drawing.Color.OrangeRed;

            if (Darkmode)
            {
                colorLine = RenderPens.WhiteSmoke;
                colorText = System.Drawing.Color.WhiteSmoke;
            }
            //-----------

                 var   infoText = $"InfoBox:   (bar - {BarNumber})"                                      + System.Environment.NewLine;
            infoText = infoText                                                                          + System.Environment.NewLine;
            infoText = infoText + $"Instrument: {InstrumentInfo.Instrument}  "                           + System.Environment.NewLine;
            infoText = infoText + $"TickSize  : {InstrumentInfo.TickSize}    "                           + System.Environment.NewLine;
            infoText = infoText + $"Chart.Time: {ChartInfo.ChartType}.{ChartInfo.TimeFrame} "            + System.Environment.NewLine;
            infoText = infoText                                                                          + System.Environment.NewLine;
            infoText = infoText + $"Open:       " + bXopen.ToString()                                    + System.Environment.NewLine;
            infoText = infoText + $"High:       " + bXhigh.ToString()                                    + System.Environment.NewLine;
            infoText = infoText + $"Low:        " + bXlow.ToString()                                     + System.Environment.NewLine;
            infoText = infoText + $"Close:      " + bXclose.ToString()                                   + System.Environment.NewLine;
            infoText = infoText                                                                          + System.Environment.NewLine;
            infoText = infoText + $"EMA Values  "                                                        + System.Environment.NewLine;
            infoText = infoText + $"EMA({fastPeriod})     " + bEMA1.ToString("0.00")                     + System.Environment.NewLine;
            infoText = infoText + $"EMA({slowPeriod})     " + bEMA2.ToString("0.00")                     + System.Environment.NewLine;

         // infoText = infoText + $"biasValue   " + bBias.ToString()                                     + System.Environment.NewLine;

            //------
            var textFont = new RenderFont("Courier New", 8);
            var textSize = context.MeasureString(infoText, textFont);
            var textRect = new Rectangle(ChartArea.Width / 2     , 15, (int)textSize.Width     , (int)textSize.Height     );
            //--                         x                       ,  y,               Widht     ,               Height

         // var lineRect = new Rectangle(ChartArea.Width / 2 - 10,  5,                 300     ,                  120     );
            var lineRect = new Rectangle(ChartArea.Width / 2 - 10,  5, (int)textSize.Width + 20, (int)textSize.Height + 20);

            context.DrawRectangle(colorLine, lineRect);
            context.DrawString(infoText, textFont, colorText, textRect);
        }

        protected override void OnCalculate(int bar, decimal value)
        {
            if (bar > 20)    //-- avoid "Index was out of range.Error"
            {
                var ema1 = _ema1.DataSeries[0][bar - BarNumber];
                var ema2 = _ema2.DataSeries[0][bar - BarNumber];

              //var bias = _bias.LineSeries.Count;

                IndicatorCandle candle = GetCandle(bar - BarNumber);

                bXopen  = candle.Open;
                bXhigh  = candle.High;
                bXlow   = candle.Low;
                bXclose = candle.Close;

                bEMA1   = (decimal)ema1;
                bEMA2   = (decimal)ema2;

              //bBias   = (decimal)bias;

            }
        }
    }
}

// add.Info @ https://docs.atas.net/en/md_DataFeedsCore_2Docs_2en_20070__Graphics.html
// ----