using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Probes
{
    /// <summary>
    /// AngleSpeedMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class AngleSpeedMeasurementNetControl : NineAxesMeasurementNetControl
    {
        protected override AxisDisplayControl Display => this._Display;

        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;

        public override AxisType AxisType => AxisType.AngleSpeed;

        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }
        public AngleSpeedMeasurementNetControl()
        {
            this.LineGroup[0].Description = "Angle Speed in Degree/s";
        }
    }
}
