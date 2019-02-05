using System;
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
        protected override Grid LinesAuxGrid => this.LinesAux;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override AxisDisplayControl Display => this._Display;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;

        public GravityMeasurementNetControl()
        {
            this.Display.Title = "Gravity";
            this.Display.ValueUnit = "g";
            this.LinesGroup[0].Description = "X(g)";
            this.LinesGroup[1].Description = "Y(g)";
            this.LinesGroup[2].Description = "Z(g)";
            this.LinesGroup[3].Description = "S(g)";

            this.LinesAuxGroup[0].Description = "T(Deg)";
            this.LinesAuxGroup[1].Description = "D(Deg)";

        }
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }
        protected override void OnGravityDataReceived(Vector3D data) => this.AddData(data, true);
        //protected override void OnGravityDataReceived(Vector3D data) => this.AddData(data.Length, 3, true);
    }
}
