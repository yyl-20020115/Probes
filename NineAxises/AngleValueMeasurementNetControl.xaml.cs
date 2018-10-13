using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Probes
{
    /// <summary>
    /// AngleValueMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class AngleValueMeasurementNetControl : MeasurementBaseNetControl, INineAxesMeasurementNetControl
    {
        public AngleValueMeasurementNetControl()
        {
            this.Line.Description = "Angle Value in Degree";
        }

        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;

        public AxisType AxisType => AxisType.AngleValue;

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
