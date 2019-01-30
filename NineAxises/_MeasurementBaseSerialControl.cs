using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace Probes
{
    public abstract class MeasurementBaseSerialControl : MeasurementBaseNetControl
    {
        public class ComNameComparer : StringComparer
        {
            public override int Compare(string x, string y)
            {
                return this.TryParseNumber(x) - this.TryParseNumber(y);
            }

            public override bool Equals(string x, string y)
            {
                return this.TryParseNumber(x) == this.TryParseNumber(y);
            }

            public override int GetHashCode(string obj)
            {
                return obj != null ? this.TryParseNumber(obj) : 0;
            }

            private int TryParseNumber(string name)
            {
                int n = -1;
                if (!string.IsNullOrEmpty(name) && name.Length > 3)
                {
                    if (int.TryParse(name.Substring(3), out n))
                    {

                    }
                }
                return n;
            }
        }
        public virtual int BaudRate => 115200;
        public delegate void OnSerialPortReceiveDataDelegate(SerialData EventType,byte[] data, int offset, int count);

        protected SerialPort Port = null;
        protected OnSerialPortReceiveDataDelegate OnSerialPortReceiveDataCallback = null;
        public MeasurementBaseSerialControl()
        {
            this.OnSerialPortReceiveDataCallback = Port_DataReceivedInternal;
        }
        public override void Dispose()
        {
            base.Dispose();
            this.DestroyPort();
        }
        protected virtual void UpdatePortNames()
        {
            var PortNames = new List<string>(SerialPort.GetPortNames());

            PortNames.Sort(new ComNameComparer());

            this.RemoteAddressComboBox.Items.Clear();

            foreach(var pn in PortNames)
            {
                this.RemoteAddressComboBox.Items.Add(pn);
            }
        }
        protected override void SetRemoteCheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {

            try
            {
                this.DestroyPort();
                this.Port = new SerialPort(this.RemoteAddressText, this.BaudRate, Parity.None, 8, StopBits.One)
                {
                    ReceivedBytesThreshold = this.ReceivePartLength
                };
                this.Port.DataReceived += Port_DataReceived;
                this.Port.Open();
                this.RemoteAddressComboBox.IsEnabled = false;
                this.OnConnectPort(this.Port);
            }
            catch
            {
                this.DestroyPort();
            }

        }
        protected virtual void OnConnectPort(SerialPort port)
        {

        }
        protected virtual void DestroyPort()
        {
            try
            {
                if (this.Port != null)
                {
                    if (this.Port.IsOpen)
                    {
                        this.Port.Close();
                    }
                    this.Port.Dispose();
                    this.Port = null;
                }
            }
            catch { }
        }
        protected override void SetRemoteCheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DestroyPort();
            this.RemoteAddressComboBox.IsEnabled = true;
        }
        protected virtual void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var data = new byte[this.Port.BytesToRead];

            this.Port.Read(data, 0, data.Length);

            Dispatcher.BeginInvoke(this.OnSerialPortReceiveDataCallback,e.EventType, data,0, data.Length);
        }

        protected virtual void Port_DataReceivedInternal(SerialData EventType, byte[] data, int offset, int count)
        {
            this.Port_DataReceivedInternal(EventType, Encoding.ASCII.GetString(data, offset, count));
        }
        protected virtual void Port_DataReceivedInternal(SerialData EventType, string text)
        {
            this.OnReceivedInternal(text);
        }
        protected virtual string Send(string text,bool read = true)
        {
            this.Send(Encoding.ASCII.GetBytes(text));

            return read ? this.Read() : string.Empty;
        }
        protected override void Send(params byte[] data)
        {
            if(this.Port!=null && this.Port.IsOpen)
            {
                this.Port.Write(data, 0, data.Length);
            }
        }
        protected virtual string Read()
        {
            string text = null;
            if (this.Port != null && this.Port.IsOpen)
            {
                text = this.Port.ReadExisting();
            }
            return text;
        }
    }
}
