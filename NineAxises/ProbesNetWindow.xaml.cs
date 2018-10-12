using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Probes
{
    public interface IMeasurementNetWindow
    {
        List<IMeasurementNetControl> Controls { get; }
        void Send(IMeasurementNetControl control, byte[] data);
        void PostReceiveBuffer(IMeasurementNetControl Control, int BufferLength, bool AutoReuse = false);

        void MoveUp(IMeasurementNetControl Control);

        void MoveDown(IMeasurementNetControl Control);

        void Remove(IMeasurementNetControl Control);
    }
    public interface IMeasurementNetControl
    {
        int ReceiveBufferLength { get; }
        IPAddress RemoteAddress { get; }
        void OnConnectWindow(IMeasurementNetWindow window);
        bool OnConnectClient(Socket Client);
        void OnReceived(byte[] data, int offset, int count);
        void OnSendComplete(byte[] data, int offset, int count);
    }

    public class ClientReceiveSocketAsyncEventArgs : SocketAsyncEventArgs
    {
        public Socket Client { get; protected set; }
        public bool AutoReuse { get; protected set; }
        public ClientReceiveSocketAsyncEventArgs(Socket Client, bool AutoReuse, params IMeasurementNetControl[] controls)
        {
            this.Client = Client ?? throw new ArgumentNullException(nameof(Client));
            this.AutoReuse = AutoReuse;
            this.Controls.AddRange(controls);
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
                    this.Client?.ReceiveAsync(this);
                }
                catch (ObjectDisposedException)
                {

                }
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
            this.SetupMenu();

            this.ServerLoop();
        }
        protected virtual void SetupMenu()
        {
            foreach (var t in this.GetControlTypes())
            {
                var mi = new MenuItem() { Header = t };
                mi.Click += Mi_Click;
                this.AddMeasurementMenuItem.Items.Add(mi);
            }
        }

        protected virtual void Mi_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi && mi.Header is Type mt)
            {
                this.AddControl(Assembly.GetAssembly(mt).CreateInstance(mt.FullName) as IMeasurementNetControl);
            }
        }

        protected virtual void CollectControls()
        {
            foreach(var c in this.ControlsContainer.Children)
            {
                if(c is IMeasurementNetControl mc && !this.Controls.Contains(mc))
                {
                    this.Controls.Add(mc);
                    mc.OnConnectWindow(this);
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
            this.ClientArgs.Values.ToList().ForEach(Args => Args.Dispose());
            foreach (var c in this.Clients)
            {
                try
                {
                    c.Disconnect(false);
                    c.Close();
                    c.Dispose();
                }
                catch
                {

                }
            }
            this.Clients.Clear();
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
            try
            {
                this.Listener?.Server?.Dispose();
            }
            catch
            {

            }
            finally
            {
                this.Listener = null;
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
                if (Control is UIElement u && !this.ControlsContainer.Children.Contains(u))
                {
                    this.ControlsContainer.Children.Add(u);
                }
                Control.OnConnectWindow(this);
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

                if(Control.OnConnectClient(Client))
                {
                    this.PostReceiveBuffer(Control,Control.ReceiveBufferLength,true);
                }
            }
        }

        public virtual void PostReceiveBuffer(IMeasurementNetControl Control, int BufferLength, bool AutoReuse = false)
        {
            this.PostReceiveBuffer(Control, ControlClients[Control], BufferLength, AutoReuse);
        }
        protected virtual void PostReceiveBuffer(IMeasurementNetControl Control, Socket Client,int BufferLength, bool AutoReuse = false)
        {
            if (Control != null && Client != null)
            {
                if (!this.ClientArgs.TryGetValue(Client, out var Args))
                {
                    Args = new ClientReceiveSocketAsyncEventArgs(Client,AutoReuse, Control);
                    Args.SetBuffer(new byte[BufferLength], 0, BufferLength);
                }
                else
                {
                    Args.Controls.Add(Control);
                }

                Client.ReceiveAsync(Args);
            }
        }

        public virtual void MoveUp(IMeasurementNetControl Control)
        {
            if(Control is UIElement u)
            {
                int i = this.ControlsContainer.Children.IndexOf(u);
                if (i >= 1)
                {
                    this.ControlsContainer.Children.RemoveAt(i);
                    this.ControlsContainer.Children.Insert(i - 1, u);
                    this.ControlsContainer.UpdateLayout();
                }
            }
        }

        public virtual void MoveDown(IMeasurementNetControl Control)
        {
            if (Control is UIElement u)
            {
                int i = this.ControlsContainer.Children.IndexOf(u);
                if (i >= 0 && i < this.ControlsContainer.Children.Count - 1)
                {
                    this.ControlsContainer.Children.RemoveAt(i);
                    this.ControlsContainer.Children.Insert(i + 1, u);
                    this.ControlsContainer.UpdateLayout();
                }
            }
        }

        public virtual void Remove(IMeasurementNetControl Control)
        {
            if (Control != null)
            {
                if(this.ControlClients.TryGetValue(Control,out var Client))
                {
                    if(this.ClientArgs.TryGetValue(Client,out var Args))
                    {
                        Args.Controls.Remove(Control);
                        if(Args.Controls.Count == 0)
                        {
                            this.ClientArgs.Remove(Client);
                            try
                            {
                                Args.Dispose();
                            }
                            catch (ObjectDisposedException) { }
                        }
                    }
                    this.ControlClients.Remove(Control);
                    this.Controls.Remove(Control);
                }
                if(Control is UIElement u)
                {
                    this.ControlsContainer.Children.Remove(u);
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

        protected List<Type> GetControlTypes()
        {
            List<Type> types = new List<Type>();
            foreach(var t in this.GetType().Assembly.GetTypes())
            {
                if (t != null)
                {
                    var i = t.GetInterface(typeof(IMeasurementNetControl).Name);
                    if (i != null)
                    {
                        types.Add(t);
                    }
                }
            }
            return types;
        }
        protected virtual void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
