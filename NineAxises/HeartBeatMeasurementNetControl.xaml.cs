using System.Windows.Controls;

namespace Probes
{
    /// <summary>
    /// FrequencyMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class HeartBeatMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceivePartLength => 21;
        public override string Header => "HEART:";
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;
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
            //HEART:N,A2,FC,1F5
            if (input != null)
            {
                var parts = input.Substring(6).TrimEnd().Split(',');
                if (parts.Length == 4)
                {
                    var beating = parts[0] == "B";
                
                    if(!int.TryParse(parts[1],System.Globalization.NumberStyles.HexNumber, null, out var bpm))
                    {
                        bpm = 0;
                    }
                    if (!int.TryParse(parts[2], System.Globalization.NumberStyles.HexNumber, null, out var ibi))
                    {
                        ibi = 0;
                    }
                    if (!int.TryParse(parts[3], System.Globalization.NumberStyles.HexNumber, null, out var signal))
                    {
                        signal = 0;
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
