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
    using Utils.Common;
    using Utils.Common.Attributes;
    using Utils.Common.Logging;   //-- <HintPath> ..\..\..\..\Program Files(x86)\ATAS Platform\Utils.Common.dll


    [Category("TDE_indicators")]
    [DisplayName("TDE_Renko24__targets")]

    public class TDE_Renko24__targets : Indicator
    {
        #region Fields
        private int _barNumber = 1;
        private int _barLine_offset;
        private int _lviBar;  // LastVisibleBarNumber
        private int _xByBar;
        private int _yByBar;
        private int _barWidth;
        private int _barX1;
        private int _barX2;
        private int _barYn;  // n Norm
        private int _barYf;  // f Flip

        public decimal bXopen;
        public decimal bXhigh;
        public decimal bXlow;
        public decimal bXclose;

        public string bXtrend;

        public decimal RenkoNormBull;
        public decimal RenkoNormBear;
        public decimal RenkoFlipBull;
        public decimal RenkoFlipBear;

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

        [Display(Name = "BarLine visible", GroupName = "Settings", Order = 60)]
        public bool BarlineVisible { get; set; }

        [Display(Name = "BarLine offset", GroupName = "Settings", Order = 70)]
        [Range(-50, 50)]
        public int BarLine_offset
        {
            get => _barLine_offset;
            set
            {
                if (_barLine_offset == value)
                    return;

                _barLine_offset = value;

                RaisePropertyChanged(nameof(_barLine_offset));
                RecalculateValues();
            }
        }

        [Display(Name = "InfoBox visible", GroupName = "Settings", Order = 80)]
        public bool InfoBoxVisible { get; set; }

        [Display(Name = "Darkmode", GroupName = "Settings", Order = 90)]
        public bool Darkmode { get; set; }
        #endregion
        //--------
        public TDE_Renko24__targets()
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

                   var infoText = "InfoBox:   (" + "bar - " + BarNumber.ToString() + ")"                   + System.Environment.NewLine;
            infoText = infoText                                                                            + System.Environment.NewLine;
            infoText = infoText + "Instrument: " + InstrumentInfo.Instrument                               + System.Environment.NewLine;
            infoText = infoText + "TickSize  : " + InstrumentInfo.TickSize                                 + System.Environment.NewLine;
            infoText = infoText + "Chart.Time: " + ChartInfo.ChartType + "." + ChartInfo.TimeFrame         + System.Environment.NewLine;
            infoText = infoText                                                                            + System.Environment.NewLine;
            infoText = infoText + "Open:   " + bXopen.ToString()                                           + System.Environment.NewLine;
            infoText = infoText + "High:   " + bXhigh.ToString()                                           + System.Environment.NewLine;
            infoText = infoText + "Low:    " + bXlow.ToString()                                            + System.Environment.NewLine;
            infoText = infoText + "Close:  " + bXclose.ToString()                                          + System.Environment.NewLine;
            // infoText = infoText                                                                         + System.Environment.NewLine;
            // infoText = infoText + "xByBar:       " + _xByBar.ToString()                                 + System.Environment.NewLine;
            // infoText = infoText + "yByBar:       " + _yByBar.ToString()                                 + System.Environment.NewLine;
            // infoText = infoText + "lastVisBarNo: " + _lviBar.ToString()                                 + System.Environment.NewLine;
            infoText = infoText                                                                            + System.Environment.NewLine;

            if (bXtrend == "bull")
            {
                infoText = infoText + "Trend:        " + bXtrend                                           + System.Environment.NewLine;
                infoText = infoText + "RenkoNorm:    " + RenkoNormBull.ToString()                          + System.Environment.NewLine;
                infoText = infoText + "RenkoFlip:    " + RenkoFlipBull.ToString()                          + System.Environment.NewLine;
            }
            else if (bXtrend == "bear")
            {
                infoText = infoText + "Trend:        " + bXtrend                                           + System.Environment.NewLine;
                infoText = infoText + "RenkoFlip:    " + RenkoFlipBear.ToString()                          + System.Environment.NewLine;
                infoText = infoText + "RenkoNorm:    " + RenkoNormBear.ToString()                          + System.Environment.NewLine;
            }



            //------
            var textFont = new RenderFont("Courier New", 8);
            var textSize = context.MeasureString(infoText, textFont);
            var textRect = new Rectangle(ChartArea.Width / 2, 15, (int)textSize.Width, (int)textSize.Height);
            //--                         x                  ,  y,               Widht,               Height

         // var lineRect = new Rectangle(ChartArea.Width / 2 - 10, 5,                 300     ,                  120     );
            var lineRect = new Rectangle(ChartArea.Width / 2 - 10, 5, (int)textSize.Width + 20, (int)textSize.Height + 20);

            if (InfoBoxVisible)
            {
                context.DrawRectangle(colorLine, lineRect);
                context.DrawString(infoText, textFont, colorText, textRect);
            }

            if (BarlineVisible)
            {
                context.DrawLine(colorLine, _xByBar + _barLine_offset, 30                  // x1, y1
                                          , _xByBar + _barLine_offset, ChartArea.Height);  // x2, y2
            }

            //-- Bull
            if (bXtrend == "bull")
            {
                _barYn = ChartInfo.GetYByPrice(RenkoNormBull);
                _barYf = ChartInfo.GetYByPrice(RenkoFlipBull);

                var textRn = new Rectangle(_barX1, _barYn - 20, (int)textSize.Width + 20, (int)textSize.Height + 20);
                var textRf = new Rectangle(_barX1, _barYf +  5, (int)textSize.Width + 20, (int)textSize.Height - 20);

                context.DrawLine(colorLine, _barX1, _barYn, _barX2, _barYn); 
                context.DrawString("RenkoNorm", textFont, colorText, textRn);
                context.DrawLine(colorLine, _barX1, _barYf, _barX2, _barYf); 
                context.DrawString("RenkoFlip", textFont, colorText, textRf);
            }
            else if (bXtrend == "bear")
            {
                _barYf = ChartInfo.GetYByPrice(RenkoFlipBear);
                _barYn = ChartInfo.GetYByPrice(RenkoNormBear);

                var textRf = new Rectangle(_barX1, _barYf - 20, (int)textSize.Width + 20, (int)textSize.Height - 20);
                var textRn = new Rectangle(_barX1, _barYn +  5, (int)textSize.Width + 20, (int)textSize.Height + 20);

                context.DrawLine(colorLine, _barX1, _barYf, _barX2, _barYf); 
                context.DrawString("RenkoFlip", textFont, colorText, textRf);
                context.DrawLine(colorLine, _barX1, _barYn, _barX2, _barYn); 
                context.DrawString("RenkoNorm", textFont, colorText, textRn);
            }
        }
        protected override void OnCalculate(int bar, decimal value)
        {
            if (bar > 20)    //-- avoid "Index was out of range.Error"
            {
                IndicatorCandle candle = GetCandle(bar - BarNumber);
                _lviBar = LastVisibleBarNumber;
                _xByBar = ChartInfo.GetXByBar(_lviBar - BarNumber, false);
                _yByBar = ChartInfo.GetYByPrice(candle.Open);

                _barWidth = ChartInfo.GetXByBar(_lviBar - BarNumber, false) - ChartInfo.GetXByBar(_lviBar - BarNumber - 1, false);
                _barX1    = _xByBar + (_barWidth * 1 / 2);
                _barX2    = _xByBar + (_barWidth * 3 / 2);

                bXopen  = candle.Open; 
                bXhigh  = candle.High; 
                bXlow   = candle.Low;  
                bXclose = candle.Close;

                if      (bXopen < bXclose)
                {
                    bXtrend = "bull";
                    RenkoNormBull = bXclose + (decimal)ChartInfo.TimeFrame.ToDecimal() * InstrumentInfo.TickSize;
                    RenkoFlipBull = bXopen  - (decimal)ChartInfo.TimeFrame.ToDecimal() * InstrumentInfo.TickSize;
                }
                else if (bXopen > bXclose)
                {
                    bXtrend = "bear";
                    RenkoNormBear = bXclose - (decimal)ChartInfo.TimeFrame.ToDecimal() * InstrumentInfo.TickSize;
                    RenkoFlipBear = bXopen  + (decimal)ChartInfo.TimeFrame.ToDecimal() * InstrumentInfo.TickSize;
                }
                else
                {
                    bXtrend = " -- ";
                    RenkoNormBull = bXclose + (decimal)ChartInfo.TimeFrame.ToDecimal() * InstrumentInfo.TickSize;
                    RenkoFlipBull = bXopen  - (decimal)ChartInfo.TimeFrame.ToDecimal() * InstrumentInfo.TickSize;
                    RenkoNormBear = bXclose - (decimal)ChartInfo.TimeFrame.ToDecimal() * InstrumentInfo.TickSize;
                    RenkoFlipBear = bXopen  + (decimal)ChartInfo.TimeFrame.ToDecimal() * InstrumentInfo.TickSize;
                }
            }
        }
    }
}

// ----