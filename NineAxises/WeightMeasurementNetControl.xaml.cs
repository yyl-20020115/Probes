using System;
using System.Windows.Controls;

namespace Probes
{
    /// <summary>
    /// WeightMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class WeightMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceivePartLength => 52;
        public override string[] Headers => new string[] { "W:", "WEIGHT:" };
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;

        public WeightMeasurementNetControl()
        {
            this.LinesGroup[0].Description = "Weight in Gram";
        }
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }
        protected const int DefaultWeightGap = 400;
        protected override void OnReceivedInternal(string input)
        {
            if (input != null && input.IndexOf('\n') == input.Length - 1)
            {
                int r2 = 0, r1 = 0, r0 = 0, rg = DefaultWeightGap, middle = 0;
                string[] parts = null;
                if (input.StartsWith("WEIGHT:"))
                {
                    parts = input.Substring(7).TrimEnd().Split(',');
                }
                else if (input.StartsWith("W:"))
                {
                    parts = input.Substring(2).TrimEnd().Split(',');
                }
                if (parts != null && parts.Length >= 5)
                {
                    bool good = true;
                    if (!int.TryParse(parts[0], System.Globalization.NumberStyles.HexNumber, null, out int value))
                    {
                        good = false;
                    }
                    else if (!int.TryParse(parts[1], System.Globalization.NumberStyles.HexNumber, null, out middle))
                    {
                        good = false;
                    }
                    else if (!int.TryParse(parts[2], System.Globalization.NumberStyles.HexNumber, null, out r2))
                    {
                        good = false;
                    }
                    else if (!int.TryParse(parts[3], System.Globalization.NumberStyles.HexNumber, null, out r1))
                    {
                        good = false;
                    }
                    else if (!int.TryParse(parts[4], System.Globalization.NumberStyles.HexNumber, null, out r0))
                    {
                        good = false;
                    }
                    else if (parts.Length == 6 && !int.TryParse(parts[5], System.Globalization.NumberStyles.HexNumber, null, out rg))
                    {
                        good = false;
                    }
                    if (good && r2 != r1)
                    {
                        if (value < r0)
                        {

                        }
                        this.Input(value, middle, r2, r1, r0, rg);
                    }
                    else
                    {

                    }
                }
            }

        }

        public virtual void Input(int value, int middle, int r2, int r1, int r0, int rg)
        {
            double Y = (r2 != r1) ? (value - r0) / (double)(r2 - r1) * rg : 0.0;

            this.AddData(Y);

        }

    }
}
