using System.Windows.Controls;

namespace Probes
{
    /// <summary>
    /// WeightMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class LightPowerMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceivePartLength => 31;
        public override string[] Headers => new string[] { "ADC:" };
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;

        public LightPowerMeasurementNetControl()
        {
            this.LinesGroup[0].Description = "Light Power in ADC value";
        }
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }
        protected override void OnReceivedInternal(string input)
        {
            if (input!=null &&input.StartsWith("ADC:"))
            {
                string[] parts = input.Substring(4).TrimEnd().Split(',');

                int value = 0, max = 0, min = 0;
                if (parts!=null && parts.Length >=3)
                {
                    bool good = true;
                    if (!int.TryParse(parts[0], System.Globalization.NumberStyles.HexNumber, null, out value))
                    {
                        good = false;
                    }
                    else if (!int.TryParse(parts[1], System.Globalization.NumberStyles.HexNumber, null, out max))
                    {
                        good = false;
                    }
                    else if (!int.TryParse(parts[2], System.Globalization.NumberStyles.HexNumber, null, out min))
                    {
                        good = false;
                    }

                    if (good)
                    {
                        this.Input(value,max,min);
                    }
                    else
                    {

                    }
                }
            }

        }
        public virtual void Input(int value, int max,int min) 
            => this.AddData(max == min ? 0 : value /(double) (max - min));
    }
}
