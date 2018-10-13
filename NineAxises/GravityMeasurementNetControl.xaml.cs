using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Probes
{
    /// <summary>
    /// GravityMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class GravityMeasurementNetControl : NineAxesMeasurementNetControl
    {
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override AxisDisplayControl Display => this._Display;

        public override AxisType AxisType => AxisType.Gravity;

        public GravityMeasurementNetControl()
        {
            this.LineGroup[0].Description = "Magnetic Field Strength in mT";
        }
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }


    }
}
