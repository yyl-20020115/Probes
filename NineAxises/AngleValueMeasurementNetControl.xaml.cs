using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Probes
{
    /// <summary>
    /// AngleValueMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class AngleValueMeasurementNetControl : NineAxesMeasurementNetControl
    {
        protected override AxisDisplayControl Display => this._Display;

        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;
        public override AxisType AxisType => AxisType.AngleValue;

        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }
        public AngleValueMeasurementNetControl()
        {
            this.LineGroup[0].Description = "Angle Value in Degree";
        }
    }
}
