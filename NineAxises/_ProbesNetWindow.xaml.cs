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
        DateTime StartTime { get; }
        List<IMeasurementNetControl> Controls { get; }
        void Send(IMeasurementNetControl control, byte[] data);
        void PostReceiveBuffer(IMeasurementNetControl Control, int BufferLength, bool AutoReuse = false);

        void Remove(IMeasurementNetControl Control);

        void ConnectClient(IMeasurementNetControl Control);
        void DisconnectClient(IMeasurementNetControl Control);

        void ResetAll();
        void ReportStatus(string status);
    }
    public interface IMeasurementNetControl
    {
        int ReceiveBufferLength { get; }
        IPAddress RemoteAddress { get; }
        string RemoteAddressText { get; }
        void OnConnectWindow(IMeasurementNetWindow window);
        bool OnConnectClient(Socket Client);
        void OnReceived(byte[] data, int offset, int count);
        void OnSendComplete(byte[] data, int offset, int count);

        void Dispose();
        void Reset();
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
        }
        protected  override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.CollectControls();
            this.SetupMenu();
            this.ServerPort = DefaultServerPort;

            this.ServerPortCheckBox.IsChecked = true;
        }

        protected virtual void SetupMenu()
        {
            //this.PlaceMeasurementMenuItem.
            var types = this.GetControlTypes();
            foreach(var t in types)
            {
                var mt = new MenuItem() { Header = t };
                
                var span = t.BaseType == typeof(NineAxesMeasurementNetControl) ? this.ControlsContainer.ColumnDefinitions.Count - 1 : this.ControlsContainer.ColumnDefinitions.Count;

                for (int row = 0; row < this.ControlsContainer.RowDefinitions.Count; row++)
                {
                    for (int col = 0; col < span; col++)
                    {
                        var pos = new MenuItem { Header = string.Format("({0},{1})", row, col), Tag = (row << 16 | col) };
                        pos.Click += Mi_Click;
                        mt.Items.Add(pos);
                    }
                }
                this.PlaceMeasurementMenuItem.Items.Add(mt);
            }

        }

        protected virtual void Mi_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi)
            {
                var mp = mi.Parent as MenuItem;
                var mt = mp.Header as Type;
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
            this.Controls.ForEach(c => c.Dispose());
            this.Controls.Clear();
        }
        protected virtual void CloseServer()
        {
            try
            {
                this.IsClosing = true;
                this.Listener.Stop();
                this.ControlClientArgs.Values.ToList().ForEach(Args => Args.Dispose());
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

        public virtual DateTime StartTime { get; protected set; } = DateTime.Now;

        protected Dictionary<IMeasurementNetControl,Socket> ControlClients = new Dictionary<IMeasurementNetControl, Socket>();
        protected Dictionary<(IMeasurementNetControl, Socket), ClientReceiveSocketAsyncEventArgs> ControlClientArgs = new Dictionary<(IMeasurementNetControl, Socket), ClientReceiveSocketAsyncEventArgs>();
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
                this.ConnectClient(Control);
                Control.OnConnectWindow(this);
                if (!this.Controls.Contains(Control))
                {
                    this.Controls.Add(Control);
                }
                if (Control is UIElement u && !this.ControlsContainer.Children.Contains(u))
                {
                    this.ControlsContainer.Children.Add(u);
                    Grid.SetRow(u, row);
                    Grid.SetColumn(u,col);
                    this.EnableMenuItem(Control.GetType(),row,col, false);
                }
            }
        }

        protected virtual void AddClient(Socket Client)
        {
            if (Client != null && !this.Clients.Contains(Client) && Client.RemoteEndPoint is IPEndPoint ipe)
            {
                var ipav6 = ipe.Address.MapToIPv6();

                var Control = this.Controls.Find(c => c.RemoteAddress!=null && ipav6.Equals(c.RemoteAddress.MapToIPv6()));
                if (Control != null)
                {
                    this.AddControlAndClient(Control, Client);
                } 
                else if(!this.Clients.Contains(Client))
                {
                    this.Clients.Add(Client);
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
        public virtual void ReportStatus(string status)
        {
            this.StatusTextBlock.Text = status ?? string.Empty;
        }

        public virtual void PostReceiveBuffer(IMeasurementNetControl Control, int BufferLength, bool AutoReuse = false)
        {
            this.PostReceiveBuffer(Control, ControlClients[Control], BufferLength, AutoReuse);
        }
        protected virtual void PostReceiveBuffer(IMeasurementNetControl Control, Socket Client,int BufferLength, bool AutoReuse = false)
        {
            if (Control != null && Client != null)
            {
                if (!this.ControlClientArgs.TryGetValue((Control,Client), out var Args))
                {
                    Args = new ClientReceiveSocketAsyncEventArgs(Client, AutoReuse, Control);
                    Args.SetBuffer(new byte[BufferLength], 0, BufferLength);
                    this.ControlClientArgs.Add((Control, Client), Args);
                    Client.ReceiveAsync(Args);
                }
            }
        }

        public virtual void Remove(IMeasurementNetControl Control)
        {
            if (Control != null)
            {
                this.DisconnectClient(Control);
                this.Controls.Remove(Control);
                if(Control is UIElement u)
                {
                    int row = Grid.GetRow(u);
                    int col = Grid.GetColumn(u);
                    this.ControlsContainer.Children.Remove(u);
                    this.EnableMenuItem(Control.GetType(),Grid.GetRow(u),Grid.GetColumn(u), true);
                    Control.Dispose();
                }
            }
        }
        protected virtual void EnableMenuItem(Type type, int row,int col,bool Enable)
        {
            bool extra = type.BaseType == typeof(NineAxesMeasurementNetControl);

            for (int i = 0; i < this.PlaceMeasurementMenuItem.Items.Count; i++)
            {
                var mt = this.PlaceMeasurementMenuItem.Items[i] as MenuItem;

                for (int j = 0; j < mt.Items.Count; j++)
                {
                    if (mt.Items[j] is MenuItem nt)
                    {
                        if ((int)nt.Tag == (row << 16 | col) || extra && (int)nt.Tag == (row << 16 | col + 1))
                        {
                            nt.IsEnabled = Enable;
                        }
                    }
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
            if (control != null&& this.ControlClients.TryGetValue(control, out var client))
            {
                this.SendData(control, client, data);
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

        public virtual void ConnectClient(IMeasurementNetControl Control)
        {
            if (Control != null && Control.RemoteAddress!=null)
            {
                var ipcv6 = Control.RemoteAddress.MapToIPv6();
                var Client = this.Clients.Find(c => c.RemoteEndPoint is IPEndPoint ipe && ipe.Address.MapToIPv6().Equals(ipcv6));
                if (Client != null)
                {
                    this.AddControlAndClient(Control, Client);
                }
            }
        }

        public virtual void DisconnectClient(IMeasurementNetControl Control)
        {
            if (Control != null)
            {
                if (this.ControlClients.TryGetValue(Control, out var Client))
                {
                    if (this.ControlClientArgs.TryGetValue((Control, Client), out var Args))
                    {
                        Args.Controls.Remove(Control);
                        if (Args.Controls.Count == 0)
                        {
                            this.ControlClientArgs.Remove((Control, Client));
                            try
                            {
                                Args.Dispose();
                            }
                            catch (ObjectDisposedException) { }
                        }
                    }
                    this.ControlClients.Remove(Control);
                }
            }
        }

        private void ResetAll_Click(object sender, RoutedEventArgs e)
        {
            this.StartTime = DateTime.Now;
            this.ResetAll();
        }

        public virtual void ResetAll()
        {
            this.Controls.ForEach(c => c.Reset());
        }
    }
}
