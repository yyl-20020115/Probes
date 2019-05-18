using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Probes
{
    /// <summary>
    /// HF50MeasurementSerialControl.xaml 的交互逻辑
    /// </summary>
    public partial class HF50MeasurementSerialControl : MeasurementBaseSerialControl
    {
        public override int BaudRate => 2400;
        public override int ReceivePartLength => 8;
        public override string[] Headers => new string[] { };
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;
        public HF50MeasurementSerialControl()
        {
            this.LinesGroup[0].Description = "Force in N";
        }

        
        protected override void CallInitializeComponent()
        {
            this.InitializeComponent();

            this.UpdatePortNames();
        }


        protected override void OnConnectPort(SerialPort port)
        {
            port?.Write("e");
        }
        protected List<byte> Buffer = new List<byte>();


        protected override void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == SerialData.Chars)
            {
                int length = this.Port.BytesToRead;

                var readbuffer = new byte[length];
                var localbuffer = new byte[8];

                this.Port.Read(readbuffer, 0, length);

                this.Buffer.AddRange(readbuffer);

                int i = this.Buffer.IndexOf(0x55);

                while (i >= 0 && this.Buffer.Count - i >= 8)
                {
                    this.Buffer.CopyTo(i, localbuffer, 0, localbuffer.Length);

                    var rx = this.ProcessBuffer(localbuffer, out var value);

                    this.Buffer.RemoveRange(0, i + rx);
                    if (!double.IsNaN(value))
                    {
                        Dispatcher.Invoke(() => this.AddData(value)); ;
                    }
                }
            }
        }
        protected override void SetRemoteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            base.SetRemoteCheckBox_Checked(sender, e);
        }
        protected override void SetRemoteCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            base.SetRemoteCheckBox_Unchecked(sender, e);
        }

        protected double g = 0.0;
        protected int u = 0;
        protected float factor = 0.00999999978f;

        protected virtual int ProcessBuffer(byte[] sdata, out double value)
        {
            var rx = 8;
            var unit = string.Empty;
            value = double.NaN;
            switch (sdata[1])
            {
                case 1:                                     // GOT VALUE
                    value = (sdata[7] & 0xF)
                       + ((sdata[6] & 0xF) << 4)
                       + ((sdata[5] & 0xF) << 8)
                       + ((sdata[4] & 0xF) << 12)
                       + ((sdata[3] & 0xF) << 16);
                    if (sdata[2] == 45)
                        value = -value;
                    value *= factor;
                    rx = 8;
                    break;
                case 2:                                     // GOT UNIT:N
                    var v6 = sdata[2];
                    switch (v6)
                    {
                        case 48:
                            unit = "kg";
                            break;
                        case 49:
                            unit = "lb";
                            break;
                        case 50:
                            unit = "N";
                            break;
                    }
                    u = v6 - 48;
                    //factor = 54 - sdata[3];
                    rx = 4;
                    break;
                case 3:                                 //Load:Value=50   
                    value = (sdata[5] & 0xF)
                       + ((sdata[4] & 0xF) << 4)
                       + ((sdata[3] & 0xF) << 8)
                       + ((sdata[2] & 0xF) << 12);

                    if (sdata[6] == 49)
                    {
                        unit = "K";
                    }
                    rx = 7;
                    break;
                case 4:                                 //Work Mode:Value = 1          
                    value = sdata[2] - 48;
                    rx = 3;
                    break;
                case 5:                                 //Lower Limit: Value= 0.0f   
                    value = (sdata[5] & 0xF)
                       + ((sdata[4] & 0xF) << 4)
                       + ((sdata[3] & 0xF) << 8)
                       + ((sdata[2] & 0xF) << 12);
                    value *= factor;
                    rx = 6;
                    break;
                case 6:                                  //Induction: Value= 0.0f  
                    value = (sdata[5] & 0xF)
                       + ((sdata[4] & 0xF) << 4)
                       + ((sdata[3] & 0xF) << 8)
                       + ((sdata[2] & 0xF) << 12);
                    value *= factor;
                    rx = 6;
                    break;
                case 7:                                 //Upper Limit: Value= 50.0f
                    value = (sdata[5] & 0xF)
                       + ((sdata[4] & 0xF) << 4)
                       + ((sdata[3] & 0xF) << 8)
                       + ((sdata[2] & 0xF) << 12);
                    value *= factor;
                    rx = 6;
                    break;
                case 8:                                   //Comparision: Value=50.0f            
                    value = (sdata[5] & 0xF)
                       + ((sdata[4] & 0xF) << 4)
                       + ((sdata[3] & 0xF) << 8)
                       + ((sdata[2] & 0xF) << 12);
                    value *= factor;
                    rx = 6;
                    break;
                case 9:                                  // Acceleration Value=9.80000019f    
                    value = (sdata[5] & 0xF)
                       + ((sdata[4] & 0xF) << 4)
                       + ((sdata[3] & 0xF) << 8)
                       + ((sdata[2] & 0xF) << 12);
                    g = value * 0.001;
                    rx = 6;
                    break;
            }
            return rx;
        }

    }
}
