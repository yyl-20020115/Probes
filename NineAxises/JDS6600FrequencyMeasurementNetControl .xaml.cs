using System;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Probes
{
    /// <summary>
    /// JDS6600FrequencyMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class JDS6600FrequencyMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceivePartLength => 13;
        public override string[] Headers => new string[] { ":r81=" };
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;
        protected DispatcherTimer CommandTimer = new DispatcherTimer();
        protected TimeSpan DefaultCommandInterval = TimeSpan.FromMilliseconds(100);
        public JDS6600FrequencyMeasurementNetControl()
        {
            this.LinesGroup[0].Description = "Frequency in Hz";
            this.CommandTimer.Interval = DefaultCommandInterval;
            this.CommandTimer.Tick += Timer_Tick;
        }
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }
        public override bool OnConnectClient(Socket Client)
        {
            this.Send(":r81=0.\r\n");
            return base.OnConnectClient(Client);
        }
        protected virtual void Timer_Tick(object sender, System.EventArgs e)
        {
            //this.Send(":r38=0.\r\n");
        }
        protected override void OnReceivedInternal(string input)
        {
            if (input != null)
            {
                input = input.TrimEnd('\0','\r','\n','.');
                var data = input.Substring(5);
                if (!string.IsNullOrEmpty(data))
                {
                    if (int.TryParse(data, System.Globalization.NumberStyles.Number, null, out var frequency))
                    {
                        this.AddData(frequency);
                    }
                }
            }
        }

        protected override void SetRemoteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            base.SetRemoteCheckBox_Checked(sender, e);
            this.CommandTimer.Start();

        }
        protected override void SetRemoteCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            base.SetRemoteCheckBox_Unchecked(sender, e);
            this.CommandTimer.Stop();
        }
    }
}
