using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Probes
{
    /// <summary>
    /// FrequencyMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class DSOFrequencyMeasurementNetControl : MeasurementBaseUDPControl
    {
        public override int ReceivePartLength => 13;
        public override string[] Headers => new string[] { "F=" };
        public override char EndOfLineChar => '\n';
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;
        protected double LastFrequency = double.NaN;
        public double DeltaRangeRatio { get; set; } = 0.1;
        public DSOFrequencyMeasurementNetControl()
        {
            this.LinesGroup[0].Description = "Frequency in Hz";
        }
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }


        protected override void OnReceivedInternal(string input)
        {
            if (input != null && input.StartsWith("F="))
            {
                var data = input.Trim().Substring(2);
                if (!string.IsNullOrEmpty(data))
                {
                      
                    if (double.TryParse(data, System.Globalization.NumberStyles.Number, null, out var frequency))
                    {
                        if (double.IsNaN(this.LastFrequency))
                        {
                            this.LastFrequency = frequency;
                        }
                        else
                        {
                            double delta = Math.Abs(frequency - this.LastFrequency);
                            if (delta < Math.Abs(frequency) * DeltaRangeRatio)
                            {
                                this.AddData(frequency);
                                this.LastFrequency = frequency;
                            }
                        }
                    }
                }
            }
        }
    }
}
