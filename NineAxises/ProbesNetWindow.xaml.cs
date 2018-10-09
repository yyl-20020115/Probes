using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace Probes
{
    public interface IMeasurementNetWindow
    {
        List<IMeasurementNetControl> Controls { get; }
        void Send(IMeasurementNetControl control, byte[] data);
    }
    public interface IMeasurementNetControl
    {
        int ReceiveBufferLength { get; }
        IPAddress RemoteAddress { get; }
        void InitWith(IMeasurementNetWindow window);
        void OnConnect(Socket Client);

        void OnReceived(byte[] data, int offset, int count);
        void OnSendComplete(byte[] data, int offset, int count);
    }

    public class ClientReceiveSocketAsyncEventArgs : SocketAsyncEventArgs
    {
        public Socket Client { get; protected set; }
        public ClientReceiveSocketAsyncEventArgs(Socket Client, params IMeasurementNetControl[] controls)
        {
            this.Client = Client ?? throw new ArgumentNullException(nameof(Client));
            this.Controls.AddRange(controls);
        }
        public virtual List<IMeasurementNetControl> Controls { get; } = new List<IMeasurementNetControl>();
        protected override void OnCompleted(SocketAsyncEventArgs e)
        {
            base.OnCompleted(e);

            this.Controls.ForEach(c => c.OnReceived(e.Buffer, e.Offset, e.Count));

            try
            {
                this.Client?.ReceiveAsync(this);
            }
            catch (ObjectDisposedException)
            {

            }
        }
    }
    public class ClientSendSocketAsyncEventArgs : SocketAsyncEventArgs
    {
        public IMeasurementNetControl Control { get; protected set; }
        public ClientSendSocketAsyncEventArgs(IMeasurementNetControl Control)
        {
            this.Control = Control ?? throw new ArgumentNullException(nameof(Control));
        }
        protected override void OnCompleted(SocketAsyncEventArgs e)
        {
            base.OnCompleted(e);

            this.Control.OnSendComplete(e.Buffer, e.Offset, e.Count);
        }
    }
    /// <summary>
    /// ProbesNetWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProbesNetWindow : Window, IMeasurementNetWindow
    {
        protected const int DefaultListeningPort = 6000;
        protected TcpListener Listener = TcpListener.Create(DefaultListeningPort);
        protected bool IsClosing = false;
        public ProbesNetWindow()
        {
            InitializeComponent();
        }
        protected  override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.CollectControls();
            this.ServerLoop();
        }
        protected virtual void CollectControls()
        {
            foreach(var c in this.ContainerGrid.Children)
            {
                if(c is IMeasurementNetControl mc && !this.Controls.Contains(mc))
                {
                    this.Controls.Add(mc);
                }
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.IsClosing = true;
            this.Listener.Stop();
            this.Controls.Clear();
            this.ControlClients.Clear();
            this.Clients.ForEach(c => c.Dispose());
            this.ClientArgs.Values.ToList().ForEach(Args => Args.Dispose());
        }
        public virtual List<IMeasurementNetControl> Controls { get; } = new List<IMeasurementNetControl>();
        public virtual List<Socket> Clients { get; } = new List<Socket>();

        protected Dictionary<IMeasurementNetControl,Socket> ControlClients = new Dictionary<IMeasurementNetControl, Socket>();
        protected Dictionary<Socket, ClientReceiveSocketAsyncEventArgs> ClientArgs = new Dictionary<Socket, ClientReceiveSocketAsyncEventArgs>();
        protected async virtual void ServerLoop()
        {
            this.Listener.Start();

            while (!this.IsClosing)
            {
                try
                {
                    this.AddClient(await this.Listener.AcceptSocketAsync());
                }
                catch
                {

                }
            }
            

        }
        protected virtual void AddControl(IMeasurementNetControl Control)
        {
            if (Control != null && !this.Controls.Contains(Control))
            {
                var ipcv6 = Control.RemoteAddress.MapToIPv6();
                var Client = this.Clients.Find(c => c.RemoteEndPoint is IPEndPoint ipe && ipe.Address.MapToIPv6().Equals(ipcv6));
                if (Client != null)
                {
                    this.AddControlAndClient(Control, Client);
                }
            }
        }

        protected virtual void AddClient(Socket Client)
        {
            if(Client!=null && !this.Clients.Contains(Client) && Client.RemoteEndPoint is IPEndPoint ipe)
            {
                var ipav6 = ipe.Address.MapToIPv6();
                var Control = this.Controls.Find(c => ipav6.Equals(c.RemoteAddress.MapToIPv6()));
                if (Control != null)
                {
                    this.AddControlAndClient(Control, Client);
                }
            }
        }
        protected virtual void AddControlAndClient(IMeasurementNetControl Control, Socket Client)
        {
            if(Control!=null && Client != null)
            {
                if (!this.Controls.Contains(Control))
                {
                    this.Controls.Add(Control);
                }
                if (!this.Clients.Contains(Client))
                {
                    this.Clients.Add(Client);
                }
                this.ControlClients[Control] = Client;

                Control.OnConnect(Client);

                if (!this.ClientArgs.TryGetValue(Client, out var Args))
                {
                    Args = new ClientReceiveSocketAsyncEventArgs(Client,Control);
                    Args.SetBuffer(new byte[Control.ReceiveBufferLength], 0, Control.ReceiveBufferLength);
                }
                else
                {
                    Args.Controls.Add(Control);
                }

                Client.ReceiveAsync(Args);
            }
        }

        protected virtual void RemoveControl(IMeasurementNetControl Control)
        {
            if (Control != null)
            {
                if(this.ControlClients.TryGetValue(Control,out var Client))
                {
                    if(this.ClientArgs.TryGetValue(Client,out var Args))
                    {
                        Args.Controls.Remove(Control);
                    }
                    this.ControlClients.Remove(Control);
                    this.Controls.Remove(Control);
                }
            }
        }

        protected virtual void RemoveClient(Socket Client, bool AlsoDispose = false)
        {
            if (Client != null)
            {
                this.ClientArgs.Remove(Client);
                var Controls = this.ControlClients.Keys.ToList();
                var ConnectedControls = Controls.Where(c => this.ControlClients[c] == Client).ToList();
                ConnectedControls.ForEach(c => this.ControlClients.Remove(c));

                if (AlsoDispose)
                {
                    Client.Dispose();
                }
            }
        }

        protected virtual void SendData(IMeasurementNetControl Control,Socket Client,byte[] buffer)
        {
            if (Client != null)
            {
                var Args = new ClientSendSocketAsyncEventArgs(Control);
                Args.SetBuffer(buffer, 0, buffer.Length);
                Client.SendAsync(Args);
            }
        }

        public virtual void Send(IMeasurementNetControl control, byte[] data)
        {
            if (control != null)
            {
                this.SendData(control,this.ControlClients[control], data);
            }
        }

    

    }
}
