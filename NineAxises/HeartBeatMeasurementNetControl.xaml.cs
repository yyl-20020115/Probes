using System.Windows.Controls;
using System.Windows.Media;

namespace Probes
{
    /// <summary>
    /// FrequencyMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class HeartBeatMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceivePartLength => 23;//HEART:N,210,1604,1500\r\n
        public override string Header => "HEART:";
        public override double PlotWidth => 2;
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;
        protected const int BPM_BASE = 100;
        protected const int IBI_BASE = 1000;
        protected const int SIGNAL_BASE = 1000;
        public HeartBeatMeasurementNetControl()
        {
            this.LinesGroup[0].Description = "Heart Beat Strength";
        }
        protected override void CallInitializeComponent()
        {
            this.InitializeComponent();
        }
        protected override void OnReceivedInternal(string input)
        {
            if (input != null)
            {
                //HEART:N,210,1604,1500\r\n
                var parts = input.Substring(6).TrimEnd().Split(',');
                if (parts.Length == 4)
                {
                    this.LED.Fill = parts[0] == "B" ? Brushes.Green : Brushes.Gray;

                    if (!int.TryParse(parts[1],System.Globalization.NumberStyles.Integer, null, out var bpm))
                    {
                        bpm = 0;
                    }
                    else
                    {
                        bpm -= BPM_BASE;
                        this.BPMTextBlock.Text = $"BPM:{bpm}";
                    }
                    if (!int.TryParse(parts[2], System.Globalization.NumberStyles.Integer, null, out var ibi))
                    {
                        ibi = 0;
                    }
                    else
                    {
                        ibi -= IBI_BASE;
                    }
                    if (!int.TryParse(parts[3], System.Globalization.NumberStyles.Integer, null, out var signal))
                    {
                        signal = 0;
                    }
                    else
                    {
                        signal -= SIGNAL_BASE;
                    }
                    if (signal > 0)
                    {
                        this.AddData(signal);
                    }
                }
            }
        }

    }
}
