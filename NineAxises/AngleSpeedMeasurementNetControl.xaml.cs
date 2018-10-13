using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Probes
{
    /// <summary>
    /// AngleSpeedMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class AngleSpeedMeasurementNetControl : MeasurementBaseNetControl, INineAxesMeasurementNetControl
    {
        public AngleSpeedMeasurementNetControl()
        {
            this.Line.Description = "Angle Speed in Degree/s";
        }

        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;

        public AxisType AxisType => AxisType.AngleSpeed;

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
