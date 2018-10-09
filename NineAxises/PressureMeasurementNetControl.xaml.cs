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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Probes
{
    /// <summary>
    /// PressureMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class PressureMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceiveBufferLength => 18;
        protected override string RemoteAddressText => "192.168.1.69";
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected double RelativeZeroY = double.NaN;
        protected bool ResetRelativeZeroY = true; //set first value to be zero value
        public PressureMeasurementNetControl()
        {
            this.Line.Description = "Pressure ADC value";
        }
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }
        public virtual void Input(int Data)
        {
            if (!this.IsPausing)
            {
                bool LED = (Data >> 16) == 0;

                this.LED.Fill = LED ? Brushes.Green : Brushes.Gray;

                int Value = Data & 0x0FFF;

                double Y = Value / 4096.0;

                //Reciprocal
                Y = Y>0.0 ? 1.0 / Y : 0.0;
                
                if (this.ResetRelativeZeroY)
                {
                    this.RelativeZeroY = Y;
                    this.ResetRelativeZeroY = false;
                }

                if (!double.IsNaN(this.RelativeZeroY))
                {
                    Y -= this.RelativeZeroY;
                }

                this.SyncPlot(Y);
            }
        }

        protected override void OnReceivedInternal(string input)
        {
            if (input.StartsWith("PRESSURE:"))
            {
                if (int.TryParse(input.Substring(9, 8), System.Globalization.NumberStyles.HexNumber, null, out int Data))
                {
                    this.Input(Data);
                }
            }
        }
        protected virtual void ResetRelativeZeroYButton_Click(object sender, RoutedEventArgs e)
        {
            this.ResetRelativeZeroY = true;
        }
    }
}
