using System.Windows.Controls;

namespace Probes
{
    /// <summary>
    /// FrequencyMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class FrequencyMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceiveBufferLength => 21;
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;

        public const int DefaultSysFrequency = 168000000;
        public override string RemoteAddressText => "192.168.1.71";

        protected const double ScaleFactor = 1.0;
        protected bool ShowingFrequency => !this.FrequencyOrTime.IsChecked.GetValueOrDefault();
        public FrequencyMeasurementNetControl()
        {
            this.LineGroup[0].Description = "Frequency in Hz";
        }
        protected override void CallInitializeComponent()
        {
            this.InitializeComponent();
        }
        protected override void OnReceivedInternal(string input)
        {
            if(input!=null&& input.StartsWith("FM:") && input.EndsWith("\n"))
            {
                var parts = input.Substring(3, 17).Split(',');
                if (parts.Length == 2)
                {
                    if(!int.TryParse(parts[0],System.Globalization.NumberStyles.HexNumber, null, out var period))
                    {
                        period = -1;
                    }
                    if(!int.TryParse(parts[1],System.Globalization.NumberStyles.HexNumber,null,out var sysfrequency))
                    {
                        sysfrequency = DefaultSysFrequency;
                    }

                    double Y = this.ShowingFrequency
                        ? (period > 0 ? sysfrequency / (period * ScaleFactor) : 0)
                        : (period * ScaleFactor / sysfrequency);
                    this.AddData(Y);
                }
            }
        }

    }
}
