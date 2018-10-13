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

        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }
        public AngleSpeedMeasurementNetControl()
        {
            this.LineGroup[0].Description = "Angle Speed in Degree/s";
        }
        protected override void OnAngleSpeedDataReceived(Vector3D data) => this.AddData(data);
    }
}
