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
        protected override Grid LinesAuxGrid => this.LinesAux;
        protected override CheckBox PauseCheckBox => this.Pause;

        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }
        public AngleSpeedMeasurementNetControl()
        {
            this.LinesGroup[0].Description = "X(Deg/s)";
            this.LinesGroup[1].Description = "Y(Deg/s)";
            this.LinesGroup[2].Description = "Z(Deg/s)";
            this.LinesGroup[3].Description = "D(Deg/s)";

            this.LinesAuxGroup[0].Description = "PA(Deg)";
            this.LinesAuxGroup[1].Description = "VA(Deg)";
        }
        protected override void OnAngleSpeedDataReceived(Vector3D data) => this.AddData(data, true);
    }
}
