using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Probes
{
    /// <summary>
    /// PressureMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class PressureMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceivePartLength => 18;
        public override string Header => "PRESSURE:";
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;

        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;
        protected override TextBlock ValueTextBox => this._ValueTextBox;

        public PressureMeasurementNetControl()
        {
            this.LinesGroup[0].Description = "Pressure ADC value";
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
                                
                this.AddData(Y);
            }
        }

        protected override void OnReceivedInternal(string input)
        {
            if (input != null && int.TryParse(input.Substring(9, 8), System.Globalization.NumberStyles.HexNumber, null, out int Data))
            {
                this.Input(Data);
            }
        }
    }
}
