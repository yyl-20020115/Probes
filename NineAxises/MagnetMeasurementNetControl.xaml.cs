using System;
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
        protected override Grid LinesAuxGrid => this.LinesAux;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;
        public MagnetMeasurementNetControl()
        {
            this.Display.Title = "Magnetic";
            this.Display.ValueUnit = "uT";
            this.LinesGroup[0].Description = "X(mT)";
            this.LinesGroup[1].Description = "Y(mT)";
            this.LinesGroup[2].Description = "Z(mT)";
            this.LinesGroup[3].Description = "S(mT)";

            this.LinesAuxGroup[0].Description = "T(Deg)";
            this.LinesAuxGroup[1].Description = "D(Deg)";
        }
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }

        protected override void OnMagnetDataReceived(Vector3D data) => this.AddData(data, true);
    }
}
