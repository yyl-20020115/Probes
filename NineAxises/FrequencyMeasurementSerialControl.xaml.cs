﻿using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Probes
{
    /// <summary>
    /// FrequencyMeasurementSerialControl.xaml 的交互逻辑
    /// </summary>
    public partial class FrequencyMeasurementSerialControl : MeasurementBaseSerialControl
    {
        public override int BaudRate => 57600;
        public override int ReceivePartLength => 13;
        public override string[] Headers => new string[] { "ce" };
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;
        protected DispatcherTimer CommandTimer = new DispatcherTimer();
        protected TimeSpan DefaultCommandInterval = TimeSpan.FromMilliseconds(10);
        public FrequencyMeasurementSerialControl()
        {
            this.LinesGroup[0].Description = "Frequency in Hz";
            this.CommandTimer.Interval = DefaultCommandInterval;
            this.CommandTimer.Tick += Timer_Tick;
        }

        protected virtual void Timer_Tick(object sender, System.EventArgs e)
        {
            this.Send("ce\r\n");
        }

        protected override void CallInitializeComponent()
        {
            this.InitializeComponent();
        }
        protected override void Port_DataReceivedInternal(SerialData EventType,string input)
        {
            if(EventType == SerialData.Chars && input != null && input.StartsWith("ce"))
            {
                var data = input.Substring(2).Trim();
                if (!string.IsNullOrEmpty(data))
                {
                    if(!int.TryParse(data,System.Globalization.NumberStyles.Number, null, out var frequency))
                    {
                        this.AddData(frequency);
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
