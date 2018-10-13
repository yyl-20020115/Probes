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
        protected override Grid LinesAuxGrid => null;
        protected override int LinesGroupLength => 3;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;

        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }
        public AngleValueMeasurementNetControl()
        {
            this.Display.Title = "Angle Value";
            this.Display.ValueUnit = "Degree";
            this.LinesGroup[0].Description = "Roll  (Degree)";
            this.LinesGroup[1].Description = "Pitch (Degree)";
            this.LinesGroup[2].Description = "Yaw   (Degree)";
        }
        protected override void OnAngleValueDataReceived(Vector3D data) => this.AddData(data,false);
    }
}
