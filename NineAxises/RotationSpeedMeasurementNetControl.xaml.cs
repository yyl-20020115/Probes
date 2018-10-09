using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        protected override string RemoteAddressText => "192.168.1.68";
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
