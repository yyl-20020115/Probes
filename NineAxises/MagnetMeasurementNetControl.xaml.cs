using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Probes
{
    /// <summary>
    /// MagnetMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class MagnetMeasurementNetControl : MeasurementBaseNetControl, INineAxesMeasurementNetControl
    {
        public MagnetMeasurementNetControl()
        {
            this.Line.Description = "Gravity in G";
        }

        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;

        public AxisType AxisType => AxisType.Magnetic;

        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }

        public void OnReceiveData(Vector3D data)
        {
            //TODO:
        }

    }
}
