using System.Windows.Controls;

namespace Probes
{
    /// <summary>
    /// WeightMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class WeightMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceivePartLength => 52;
        public override string Header => "WEIGHT:";
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;
        protected override TextBlock ValueTextBox => this._ValueTextBox;

        public const double _500g = 500.0;
        public const double _100g = 100.0;
        public const double WeightGap = _500g - _100g;

        public WeightMeasurementNetControl()
        {
            this.LinesGroup[0].Description = "Weight in Gram";
        }
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }
        protected override void OnReceivedInternal(string input)
        {
            if (input!=null)
            {
                var parts = input.Substring(7,52-8).Split(',');
                if (parts.Length == 5)
                {
                    bool good = true;
                    if (!int.TryParse(parts[0], System.Globalization.NumberStyles.HexNumber, null, out int value))
                    {
                        good = false;
                    }
                    if (!int.TryParse(parts[1], System.Globalization.NumberStyles.HexNumber, null, out int middle))
                    {
                        good = false;
                    }
                    if (!int.TryParse(parts[2], System.Globalization.NumberStyles.HexNumber, null, out int r2))
                    {
                        good = false;
                    }
                    if (!int.TryParse(parts[3], System.Globalization.NumberStyles.HexNumber, null, out int r1))
                    {
                        good = false;
                    }
                    if (!int.TryParse(parts[4], System.Globalization.NumberStyles.HexNumber, null, out int r0))
                    {
                        good = false;
                    }
                    if (good)
                    {
                        this.Input(value, middle, r2, r1, r0);
                    }
                    else
                    {

                    }
                }
            }

        }
        public virtual void Input(int value, int middle, int r2, int r1, int r0) 
            => this.AddData((r2 != r1) ? (value - r0) / (double)(r2 - r1) * WeightGap : 0.0);


    }
}
