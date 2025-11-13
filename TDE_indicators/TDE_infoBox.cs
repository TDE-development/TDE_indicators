namespace ATAS.Indicators.TDE_indicators
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


    [Category("TDE_indicators")]
    [DisplayName("TDE_infoBox")]

    public class TDE_infoBox : Indicator
    {
        #region Fields
        private int _barNumber;


        public decimal bXopen;
        public decimal bXhigh;
        public decimal bXlow;
        public decimal bXclose;

        public decimal bXvol;
        public decimal bXdelta;
        public decimal bXmaxd;
        public decimal bXmind;
        public decimal bXticks;

        public decimal bXmaxVol_Pi__Price;
        public decimal bXmaxVol_Pi__Volume;
        public decimal bXmaxVol_Pi__bid;
        public decimal bXmaxVol_Pi__ask;

        public decimal bXmaxTik_Pi__Price;
        public decimal bXmaxTik_Pi__Volume;
        public decimal bXmaxAsk_Pi__Price;
        public decimal bXmaxAsk_Pi__Volume;
        public decimal bXmaxBid_Pi__Price;
        public decimal bXmaxBid_Pi__Volume;
        public decimal bXmaxPosDelta_Pi__Price;
        public decimal bXmaxPosDelta_Pi__Volume;
        public decimal bXmaxNegDelta_Pi__Price;
        public decimal bXmaxNegDelta_Pi__Volume;
        public decimal bXvalArea__High;
        public decimal bXvaLArea__Low;

        // public decimal  bXcust__ticks;
        // public readonly IndexPerformance.XcRXV_ticks _ticks = new();
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

        [Display(Name = "Darkmode", GroupName = "Settings", Order = 90)]
        public bool Darkmode { get; set; }
        #endregion
        //--------
        public TDE_infoBox()
        {
            EnableCustomDrawing = true;
            SubscribeToDrawingEvents(DrawingLayouts.Final);
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

                 var   infoText = "InfoBox:   (" + "bar - " + BarNumber.ToString() + ")"                 + System.Environment.NewLine;
            infoText = infoText                                                                          + System.Environment.NewLine;
            infoText = infoText + "Instrument: " + InstrumentInfo.Instrument                             + System.Environment.NewLine;
            infoText = infoText + "TickSize  : " + InstrumentInfo.TickSize                               + System.Environment.NewLine;
            infoText = infoText + "Chart.Time: " + ChartInfo.ChartType + "." +  ChartInfo.TimeFrame      + System.Environment.NewLine;
            infoText = infoText                                                                          + System.Environment.NewLine;
            infoText = infoText + "Open:   "     + bXopen.ToString()                                     + System.Environment.NewLine;
            infoText = infoText + "High:   "     + bXhigh.ToString()                                     + System.Environment.NewLine;
            infoText = infoText + "Low:    "     + bXlow.ToString()                                      + System.Environment.NewLine;
            infoText = infoText + "Close:  "     + bXclose.ToString()                                    + System.Environment.NewLine;
            infoText = infoText                                                                          + System.Environment.NewLine;
            infoText = infoText + "vol:    "     + bXvol.ToString()                                      + System.Environment.NewLine;
            infoText = infoText + "delta:  "     + bXdelta.ToString()                                    + System.Environment.NewLine;
            infoText = infoText + "maxd:   "     + bXmaxd.ToString()                                     + System.Environment.NewLine;
            infoText = infoText + "mind:   "     + bXmind.ToString()                                     + System.Environment.NewLine;
            infoText = infoText + "ticks:  "     + bXticks.ToString()                                    + System.Environment.NewLine;
            infoText = infoText                                                                          + System.Environment.NewLine;
            infoText = infoText + "maxVol_Pi__Price:  " + bXmaxVol_Pi__Price.ToString()                  + System.Environment.NewLine;
            infoText = infoText + "maxVol_Pi__Volume: " + bXmaxVol_Pi__Volume.ToString()                 + System.Environment.NewLine;
            infoText = infoText + "maxVol_Pi__bid:    " + bXmaxVol_Pi__bid.ToString()                    + System.Environment.NewLine;
            infoText = infoText + "maxVol_Pi__ask:    " + bXmaxVol_Pi__ask.ToString()                    + System.Environment.NewLine;
 
        //  infoText = infoText + "maxTik_Pi__Price:  " + bXmaxTik_Pi__Price.ToString()                  + System.Environment.NewLine;
        //  infoText = infoText + "maxTik_Pi__Volume: " + bXmaxTik_Pi__Volume.ToString()                 + System.Environment.NewLine;
        //  infoText = infoText + "maxAsk_Pi__Price:  " + bXmaxAsk_Pi__Price.ToString()                  + System.Environment.NewLine;
        //  infoText = infoText + "maxAsk_Pi__Volume: " + bXmaxAsk_Pi__Volume.ToString()                 + System.Environment.NewLine;
        //  infoText = infoText + "maxBid_Pi__Price:  " + bXmaxBid_Pi__Price.ToString()                  + System.Environment.NewLine;
        //  infoText = infoText + "maxBid_Pi__Volume: " + bXmaxBid_Pi__Volume.ToString()                 + System.Environment.NewLine;
            infoText = infoText                                                                          + System.Environment.NewLine;
            infoText = infoText + "maxPosDelta_Pi__Price:  " + bXmaxPosDelta_Pi__Price.ToString()        + System.Environment.NewLine;
            infoText = infoText + "maxPosDelta_Pi__Volume: " + bXmaxPosDelta_Pi__Volume.ToString()       + System.Environment.NewLine;
            infoText = infoText + "maxNegDelta_Pi__Price:  " + bXmaxNegDelta_Pi__Price.ToString()        + System.Environment.NewLine;
            infoText = infoText + "maxNegDelta_Pi__Volume: " + bXmaxNegDelta_Pi__Volume.ToString()       + System.Environment.NewLine;
            infoText = infoText                                                                          + System.Environment.NewLine;
            infoText = infoText + "bar valArea__High:      " + bXvalArea__High.ToString()                + System.Environment.NewLine;
            infoText = infoText + "bar vaLArea__Low:       " + bXvaLArea__Low.ToString()                 + System.Environment.NewLine;
            infoText = infoText                                                                          + System.Environment.NewLine;
        //  infoText = infoText + "XcRXV_ticks (value):    " + "coming soon..."                          + System.Environment.NewLine;

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
                IndicatorCandle candle = GetCandle(bar - BarNumber);

                bXopen  = candle.Open;
                bXhigh  = candle.High;
                bXlow   = candle.Low;
                bXclose = candle.Close;

                bXvol   = candle.Volume;
                bXdelta = candle.Delta;
                bXmaxd  = candle.MaxDelta;
                bXmind  = candle.MinDelta;
                bXticks = candle.Ticks;


                PriceVolumeInfo maxVol_Pi = candle.MaxVolumePriceInfo;
                bXmaxVol_Pi__Price  = maxVol_Pi.Price;
                bXmaxVol_Pi__Volume = maxVol_Pi.Volume;

                var volumeInfo = candle.GetPriceVolumeInfo(bXmaxVol_Pi__Price);
                bXmaxVol_Pi__bid = volumeInfo.Bid;
                bXmaxVol_Pi__ask = volumeInfo.Ask;

                //----
                PriceVolumeInfo maxTik_Pi = candle.MaxTickPriceInfo;
                bXmaxTik_Pi__Price  = maxTik_Pi.Price;
                bXmaxTik_Pi__Volume = maxTik_Pi.Volume;
                //----
                PriceVolumeInfo maxAsk_Pi = candle.MaxAskPriceInfo;
                bXmaxAsk_Pi__Price  = maxAsk_Pi.Price;
                bXmaxAsk_Pi__Volume = maxAsk_Pi.Volume;
                //----
                PriceVolumeInfo maxBid_Pi = candle.MaxBidPriceInfo;
                bXmaxBid_Pi__Price  = maxBid_Pi.Price;
                bXmaxBid_Pi__Volume = maxBid_Pi.Volume;
                //----
                PriceVolumeInfo maxPosDelta_Pi = candle.MaxPositiveDeltaPriceInfo;
                bXmaxPosDelta_Pi__Price  = maxPosDelta_Pi.Price;
                bXmaxPosDelta_Pi__Volume = maxPosDelta_Pi.Volume;
                //----
                PriceVolumeInfo maxNegDelta_Pi = candle.MaxNegativeDeltaPriceInfo;
                bXmaxNegDelta_Pi__Price  = maxNegDelta_Pi.Price;
                bXmaxNegDelta_Pi__Volume = maxNegDelta_Pi.Volume;
                //----
                ValueArea valArea = candle.ValueArea;
                bXvalArea__High = valArea.ValueAreaHigh;
                bXvaLArea__Low  = valArea.ValueAreaLow;
            }
        }
    }
}

// add.Info @ https://docs.atas.net/en/md_DataFeedsCore_2Docs_2en_20070__Graphics.html
// ----