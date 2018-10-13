using System.Windows;
using System.Windows.Controls;

namespace Probes
{
    /// <summary>
    /// HighVoltageNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class HighVoltageNetControl : MeasurementBaseNetControl
    {
        public const double _10g = 10.0;
        public const double _5g = 5.0;
        public const double WeightGap = _10g - _5g;
        public override int ReceiveBufferLength => 52;
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;
        public HighVoltageNetControl()
        {
            this.LineGroup[0].Description = "Weight in Gram(Range:30g)";
        }
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }
        protected override void OnReceivedInternal(string input)
        {
            if (input != null && input.StartsWith("WEIGHT:"))
            {
                var parts = input.Substring(7).TrimEnd().Split(',');
                if (parts.Length == 5)
                {
                    if (!int.TryParse(parts[0], System.Globalization.NumberStyles.HexNumber, null, out int value))
                    {
                        value = 0;
                    }
                    if (!int.TryParse(parts[1], System.Globalization.NumberStyles.HexNumber, null, out int middle))
                    {
                        middle = 0;
                    }
                    if (!int.TryParse(parts[2], System.Globalization.NumberStyles.HexNumber, null, out int r2))
                    {
                        r2 = 0;
                    }
                    if (!int.TryParse(parts[3], System.Globalization.NumberStyles.HexNumber, null, out int r1))
                    {
                        r1 = 0;
                    }
                    if (!int.TryParse(parts[4], System.Globalization.NumberStyles.HexNumber, null, out int r0))
                    {
                        r0 = 0;
                    }
                    this.Input(value, middle, r2, r1, r0);
                }
            }

        }
        public virtual void Input(int value, int middle, int r2, int r1, int r0) 
            => this.AddData((r2 != r1) ? (value - r0) / (double)(r2 - r1) * WeightGap : 0.0);

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int value = ((int)e.NewValue & 0x3ff);
            //high byte first, then low byte
            this.Send((byte)((value&0xff00)>>8),(byte)(value &0xff));
        }

        private void ZVButton_Click(object sender, RoutedEventArgs e)
        {
            //Send 0 to DAC instantly!
            this.Send((byte)0,(byte)0);
        }
    }
}
