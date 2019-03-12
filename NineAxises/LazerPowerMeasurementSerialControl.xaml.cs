using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Probes
{
    /// <summary>
    /// LazerPowerMeasurementSerialControl.xaml 的交互逻辑
    /// </summary>
    public partial class LazerPowerMeasurementSerialControl : MeasurementBaseSerialControl
    {
        //?pw%
        //20 2D 30 2E 30 38 00
        public override int BaudRate => 9600;
        public override int ReceivePartLength => 7;
        public override string[] Headers => new string[] {" "};
        public override char EndOfLineChar => '\0';
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;
        protected DispatcherTimer CommandTimer = new DispatcherTimer();
        protected TimeSpan DefaultCommandInterval = TimeSpan.FromMilliseconds(100);
        public LazerPowerMeasurementSerialControl()
        {
            this.LinesGroup[0].Description = "Power in mW";
            this.CommandTimer.Interval = DefaultCommandInterval;
            this.CommandTimer.Tick += Timer_Tick;
        }

        protected virtual void Timer_Tick(object sender, System.EventArgs e)
        {
            this.Send("?pw%",false);
        }
         
        protected override void CallInitializeComponent()
        {
            this.InitializeComponent();
        }
        protected override void Port_DataReceivedInternal(SerialData EventType,string input)
        {
            if(EventType == SerialData.Chars && !string.IsNullOrEmpty(input))
            {
                int p = input.IndexOf(this.EndOfLineChar);
                if (p > 0)
                {
                    input = input.Substring(0, p);
                }
                if(double.TryParse(input.Trim(), out var data))
                {
                    this.AddData(data);
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
