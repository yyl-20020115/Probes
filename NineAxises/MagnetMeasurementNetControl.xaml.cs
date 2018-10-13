using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Probes
{
    /// <summary>
    /// MagnetMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class MagnetMeasurementNetControl : NineAxesMeasurementNetControl
    {
        protected override AxisDisplayControl Display => this._Display;

        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;

        public override AxisType AxisType => AxisType.Magnetic;
        public MagnetMeasurementNetControl()
        {
            this.LineGroup[0].Description = "Gravity in G";
        }
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }


    }
}
