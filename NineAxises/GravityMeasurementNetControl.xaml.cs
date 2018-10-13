using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Probes
{
    /// <summary>
    /// GravityMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class GravityMeasurementNetControl : MeasurementBaseNetControl, INineAxesMeasurementNetControl
    {
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;

        public AxisType AxisType => AxisType.Gravity;

        public GravityMeasurementNetControl()
        {
            this.Line.Description = "Magnetic Field Strength in mT";
        }
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
