using InteractiveDataDisplay.WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Probes
{
    public abstract partial class MeasurementBaseUDPControl : MeasurementBaseNetControl
    {
        public class UDPReceiveSocketAsyncEventArgs : SocketAsyncEventArgs
        {
            public Socket UDPSocket { get; protected set; }
            public bool AutoReuse { get; protected set; }
            public new byte[] Buffer = new byte[32];
            public UDPReceiveSocketAsyncEventArgs(Socket UDPSocket, bool AutoReuse, params IMeasurementNetControl[] controls)
            {
                this.UDPSocket = UDPSocket ?? throw new ArgumentNullException(nameof(UDPSocket));
                this.AutoReuse = AutoReuse;
                this.Controls.AddRange(controls);
                this.SetBuffer(this.Buffer, 0, this.Buffer.Length);
                this.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            }
            public virtual List<IMeasurementNetControl> Controls { get; } = new List<IMeasurementNetControl>();
            protected override void OnCompleted(SocketAsyncEventArgs e)
            {
                base.OnCompleted(e);

                this.Controls.ForEach(c => c.OnReceived(e.Buffer, e.Offset, e.Count));

                if (this.AutoReuse)
                {
                    try
                    {
                        this.UDPSocket?.ReceiveFromAsync(this);
                    }
                    catch (ObjectDisposedException)
                    {

                    }
                }
            }
        }
        protected virtual int Port { get; set; } = 8192;

        protected Socket UDPSocket = null;
        public MeasurementBaseUDPControl()
        {

        }
        ~MeasurementBaseUDPControl()
        {
            this.FreeUDPSocket();
        }
        protected override void SetRemoteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            base.SetRemoteCheckBox_Checked(sender, e);
            this.CreateUDPSocket();
        }
        protected override void SetRemoteCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.FreeUDPSocket();
            base.SetRemoteCheckBox_Unchecked(sender, e);
        }
        protected virtual void CreateUDPSocket()
        {
            try
            {
                if (this.UDPSocket != null)
                {
                    this.FreeUDPSocket();
                }
                this.UDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                this.UDPSocket.Bind(new IPEndPoint(IPAddress.Any, this.Port));

                this.UDPSocket.ReceiveFromAsync(new UDPReceiveSocketAsyncEventArgs(this.UDPSocket, true, this));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        protected virtual void FreeUDPSocket()
        {
            if (this.UDPSocket != null)
            {
                this.UDPSocket.Close();
                this.UDPSocket.Dispose();
                this.UDPSocket = null;
            }
        }
    }
}
