using System.Windows.Controls;

namespace Probes
{
    /// <summary>
    /// WeightMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class WeightMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceiveBufferLength => 52;
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;

        public override string RemoteAddressText => "192.168.1.70";

        public const double _500g = 500.0;
        public const double _100g = 100.0;
        public const double WeightGap = _500g - _100g;

        public WeightMeasurementNetControl()
        {
            this.LineGroup[0].Description = "Weight in Gram(Range:3000g)";
        }
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }
        protected override void OnReceivedInternal(string input)
        {
            if (input!=null&& input.StartsWith("WEIGHT:"))
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

 
    }
}
