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

        void Remove(IMeasurementNetControl Control);
    }
    public interface IMeasurementNetControl
    {
        int ReceiveBufferLength { get; }
        IPAddress RemoteAddress { get; }
        string RemoteAddressText { get; set; }
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
        protected const int DefaultServerPort = 6000;
        protected TcpListener Listener = null;
        protected bool IsClosing = false;
        public int ServerPort
        {
            get => int.TryParse(this.ServerPortTextBox.Text, out var p) ? p : 0;
            set => this.ServerPortTextBox.Text = value.ToString();
        }
        public ProbesNetWindow()
        {
            InitializeComponent();
            this.ServerPort = DefaultServerPort;
        }
        protected  override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.CollectControls();
            this.SetupMenu();

            this.ServerPortCheckBox.IsChecked = true;
        }

        protected virtual void SetupMenu()
        {
            //this.PlaceMeasurementMenuItem.
            var types = this.GetControlTypes();
            for (int row = 0; row < 2; row++)
            {
                for (int col = 0; col < 2; col++)
                {
                    var pos = new MenuItem { Header = string.Format("({0},{1})", row, col) };
                    this.PlaceMeasurementMenuItem.Items.Add(pos);
                    foreach (var t in types)
                    {
                        var mi = new MenuItem() { Header = t,Tag=(row<<16|col) };
                        mi.Click += Mi_Click;
                        pos.Items.Add(mi);
                    }
                }
            }
        }

        protected virtual void Mi_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi && mi.Header is Type mt)
            {
                int v =(int) mi.Tag;
                this.AddControl(
                    Assembly.GetAssembly(mt).CreateInstance(mt.FullName) as IMeasurementNetControl,(v>>16),(v&0xffff));
                
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
            this.CloseServer();
            this.Controls.Clear();
        }
        protected virtual void CloseServer()
        {
            try
            {
                this.IsClosing = true;
                this.Listener.Stop();
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
            }
            catch { }
            finally
            {
                this.Listener = null;
                this.ControlClients.Clear();
                this.Clients.Clear();
            }
        }
        public virtual List<IMeasurementNetControl> Controls { get; } = new List<IMeasurementNetControl>();
        public virtual List<Socket> Clients { get; } = new List<Socket>();

        protected Dictionary<IMeasurementNetControl,Socket> ControlClients = new Dictionary<IMeasurementNetControl, Socket>();
        protected Dictionary<Socket, ClientReceiveSocketAsyncEventArgs> ClientArgs = new Dictionary<Socket, ClientReceiveSocketAsyncEventArgs>();
        protected async virtual void ServerLoop()
        {
            this.Listener = TcpListener.Create(this.ServerPort);
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
        protected virtual void AddControl(IMeasurementNetControl Control, int row,int col)
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
                    Grid.SetRow(u, row);
                    Grid.SetColumn(u,col);
                    var mi = this.PlaceMeasurementMenuItem.Items[row * 2 + col] as MenuItem;
                    if (mi != null)
                    {
                        mi.IsEnabled = false;
                    }

                }
                Control.OnConnectWindow(this);

            }
        }

        protected virtual void AddClient(Socket Client)
        {
            if (Client != null && !this.Clients.Contains(Client) && Client.RemoteEndPoint is IPEndPoint ipe)
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
                    int row = Grid.GetRow(u);
                    int col = Grid.GetColumn(u);
                    this.ControlsContainer.Children.Remove(u);
                    var mi = this.PlaceMeasurementMenuItem.Items[row * 2 + col] as MenuItem;
                    if (mi != null)
                    {
                        mi.IsEnabled = true;
                    }
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
            foreach (var t in this.GetType().Assembly.GetTypes())
            {
                //therefore the decoder is not included
                if (t!=null && !t.IsInterface &&!t.IsAbstract&& t.BaseType.GetInterface(typeof(IMeasurementNetControl).FullName)!=null)
                {
                    types.Add(t);
                }
            }
            return types;
        }
        protected virtual void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected virtual void ServerPortCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.ServerPortTextBox.IsEnabled = false;
            this.ServerLoop();
        }

        protected virtual void ServerPortCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.CloseServer();
            this.ServerPortTextBox.IsEnabled = true;
        }
    }
}
