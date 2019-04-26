using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Probes
{
    /// <summary>
    /// FastWeightMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class FastWeightMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceivePartLength => 10;
        public override string[] Headers => new string[] { "=" };
        public override char EndOfLineChar => '\r';
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;
        protected DispatcherTimer CommandTimer = new DispatcherTimer();
        protected TimeSpan DefaultCommandInterval = TimeSpan.FromMilliseconds(10);
        public FastWeightMeasurementNetControl()
        {
            this.LinesGroup[0].Description = "Fast Weight in KG";
            this.CommandTimer.Interval = DefaultCommandInterval;
            this.CommandTimer.Tick += Timer_Tick;
        }
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }
        protected virtual void Timer_Tick(object sender, System.EventArgs e)
        {
            //23 30 31 0D
            //READ NET: 01
            this.Send("#0101\r");
        }
        protected override void OnReceivedInternal(string input)
        {
            if (input != null && input.StartsWith("="))
            {
                //=+000.01@

                var data = input.Substring(1, 7).Trim();
                if (!string.IsNullOrEmpty(data))
                {
                    if (double.TryParse(data, System.Globalization.NumberStyles.Number, null, out var weight))
                    {
                        this.AddData(weight);
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
