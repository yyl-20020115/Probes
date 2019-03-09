using System;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace Probes
{
    /// <summary>
    /// EPMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class EPMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceivePartLength => 76;
        public override string[] Headers => new string[] { "Voltage:"};
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;
        protected override int LinesGroupLength => 3;

        public EPMeasurementNetControl()
        {
            this.LinesGroup[0].Description = "Voltage in Volts";
            this.LinesGroup[1].Description = "Current in Amps";
            this.LinesGroup[2].Description = "Power in Watts";
            this.LinesGroup[0].Stroke = Brushes.Red;
            this.LinesGroup[1].Stroke = Brushes.Green;
            this.LinesGroup[2].Stroke = Brushes.Blue;
        }
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }

        protected string Buffer = string.Empty;
        protected override void  OnReceivedInternal(string text, string[] headers, int length)
        {
            this.Buffer += text;
            int p = 0;
            while ((p = this.Buffer.IndexOf("\r\n")) >= 0)
            {
                string line = this.Buffer.Substring(0,p);
                this.OnReceivedLine(line);
                this.Buffer = this.Buffer.Substring(p+2);
            }
        }
        protected virtual void OnReceivedLine(string input)
        {
            //Product ID:0B01200D
            //Voltage: 0.004V Current:0.000A NTC Temperature: -20.0C
            //9,5
            //24,5
            if (!string.IsNullOrEmpty(input))
            {
                if(input.StartsWith("Product ID:"))
                {

                }else
                {
                    var vp = input.IndexOf("Voltage:");
                    if (vp >= 0)
                    {
                        string vt = input.Substring(vp + 8, 5);

                        if(double.TryParse(vt, out var voltage))
                        {
                            var cp = input.IndexOf("Current:");

                            if (cp >= 0)
                            {
                                string at = input.Substring(cp + 8, 5);

                                if (double.TryParse(at, out var current))
                                {
                                    var power = voltage * current;
                                    this.AddData(voltage, 0);
                                    this.AddData(current, 1);
                                    this.AddData(power, 2);
                                    this.UpdateLines();
                                }
                            }
                        }
                    }
                    
                }
            }
        }

    }
}
