using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Probes
{
    /// <summary>
    /// FrequencyMeasurementSerialControl.xaml 的交互逻辑
    /// </summary>
    public partial class FastWeightSerialControl : MeasurementBaseSerialControl
    {
        public override int BaudRate => 115200;
        //=+000.01@
        public override int ReceivePartLength => 13;
        public override string[] Headers => new string[] { "=" };
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;
        protected DispatcherTimer CommandTimer = new DispatcherTimer();
        protected TimeSpan DefaultCommandInterval = TimeSpan.FromMilliseconds(10);
        public FastWeightSerialControl()
        {
            this.LinesGroup[0].Description = "Fast Weight in Gram";
            this.CommandTimer.Interval = DefaultCommandInterval;
            this.CommandTimer.Tick += Timer_Tick;
        }

        protected virtual void Timer_Tick(object sender, System.EventArgs e)
        {
            //23 30 31 0D
            this.Send("#01\n");
        }

        protected override void CallInitializeComponent()
        {
            this.InitializeComponent();
        }
        protected override void Port_DataReceivedInternal(SerialData EventType,string input)
        {
            if(EventType == SerialData.Chars && input != null && input.StartsWith("="))
            {
                //=+000.01@

                var data = input.Substring(1,7).Trim();
                if (!string.IsNullOrEmpty(data))
                {
                    if(!double.TryParse(data,System.Globalization.NumberStyles.Number, null, out var weight))
                    {
                        this.AddData(weight);
                    }
                }
            }
        }
        protected override void SetRemoteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            base.SetRemoteCheckBox_Checked(sender, e);
            if(this.Port!=null && this.Port.IsOpen)
            {
                this.CommandTimer.Start();
            }
        }
        protected override void SetRemoteCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            base.SetRemoteCheckBox_Unchecked(sender, e);
            this.CommandTimer.Stop();
        }
    }
}
