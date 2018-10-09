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
    /// AmpMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class AmpMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceiveBufferLength => 52;
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override double SampleInterval => 0.068;//68ms

        protected override string RemoteAddressText => "192.168.1.72";

        public AmpMeasurementNetControl()
        {
            this.Line.Description = "Current in Amp";
        }
        protected override void CallInitializeComponent()
        {
            this.InitializeComponent();
        }
        protected override void OnReceivedInternal(byte[] data, int offset, int count)
        {
            //TODO: parsing
        }

    }
}
