using System;
using System.Windows.Controls;

namespace Probes
{
    /// <summary>
    /// RotationSpeedMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class RotationSpeedMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceiveBufferLength => 12;
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;

        public override string RemoteAddressText => "192.168.1.68";
        public RotationSpeedMeasurementNetControl()
        {
            this.Line.Description = "Rotation Speed in RPM";
        }
        protected override void CallInitializeComponent()
        {
            this.InitializeComponent();
        }
        protected override void OnReceivedInternal(string input)
        {
            if (input != null && input.StartsWith("RS:") && input.EndsWith("\n"))
            {
                var parts = input.Substring(3, 8);

                if (!int.TryParse(parts, System.Globalization.NumberStyles.HexNumber, null, out var rpm))
                {
                    rpm = -1;
                }

                this.SyncPlot(rpm >= 0 ? rpm : 0);
            }
        }
    }
}
