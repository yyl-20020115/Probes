using System;
using System.Windows.Controls;

namespace Probes
{
    /// <summary>
    /// ADCMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class ADCMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceivePartLength => 11;
        public override string[] Headers => new string[] { "A:" };
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;

        public ADCMeasurementNetControl()
        {
            this.LinesGroup[0].Description = "ADC value";
        }
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }
        protected const int DefaultWeightGap = 400;
        protected override void OnReceivedInternal(string input)
        {
            if (!string.IsNullOrEmpty(input) && input.StartsWith("A:") 
                && int.TryParse(input.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out int value))
            {
                this.AddData(value - (1<<22));
            }
        }
    }

}
