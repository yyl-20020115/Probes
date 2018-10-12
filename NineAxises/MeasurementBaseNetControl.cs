using InteractiveDataDisplay.WPF;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Probes
{
    public abstract partial class MeasurementBaseNetControl : UserControl, IMeasurementNetControl
    {
        public delegate void OnReceiveDataDelegate(byte[] data, int offset, int count);

        public virtual int ReceiveBufferLength { get; set; } = 1;
        public virtual string RemoteAddressText { get; set; } = string.Empty;
        public virtual IPAddress RemoteAddress => IPAddress.TryParse(this.RemoteAddressText, out var add) ? add : IPAddress.None;
        protected IMeasurementNetWindow window = null;
        protected abstract CheckBox PauseCheckBox { get; }
        protected abstract Grid LinesGrid { get; }

        protected PointCollection Points = new PointCollection();
        protected DateTime StartTime = DateTime.MinValue;
        protected LineGraph Line = new LineGraph();

        public virtual double PlotWidth => 60.0; //60 seconds
        public virtual bool IsPausing => this.PauseCheckBox.IsChecked.GetValueOrDefault();

        protected OnReceiveDataDelegate OnReceivedCallback = null;
        public MeasurementBaseNetControl()
        {
            this.CallInitializeComponent();
            this.Line.Stroke = Brushes.Blue;
            this.Line.StrokeThickness = 1;
            this.LinesGrid.Children.Add(this.Line);

            this.Line.IsAutoFitEnabled = true;
            this.OnReceivedCallback = new OnReceiveDataDelegate(this.OnReceivedInternal);
        }
        protected virtual void CallInitializeComponent()
        {

        }
           
        public virtual void OnConnectWindow(IMeasurementNetWindow window)
        {
            this.window = window;
        }
        public virtual bool OnConnectClient(Socket Client)
        {
            return true;
        }
        protected virtual void PostReceiveBuffer(int BufferLength, bool AutoReuse = false)
        {
            this.window?.PostReceiveBuffer(this, BufferLength,AutoReuse);
        }
        public virtual void OnReceived(byte[] data, int offset, int count)
        {
            if (this.StartTime.Equals(DateTime.MinValue))
            {
                this.StartTime = DateTime.Now;
            }
            Dispatcher.BeginInvoke(this.OnReceivedCallback, data, offset, count);
        }
        protected virtual void OnReceivedInternal(byte[] data, int offset, int count)
        {
            this.OnReceivedInternal(Encoding.ASCII.GetString(data, offset, count));
        }
        protected virtual void OnReceivedInternal(string input)
        {

        }
        public virtual void OnSendComplete(byte[] data,int offset,int count)
        {

        }
        protected virtual void Send(string command) => this.Send(Encoding.ASCII.GetBytes(command));

        protected virtual void Send(params byte[] data) => this.window?.Send(this, data);
        protected virtual void MoveUpButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.window?.MoveUp(this);
        }
        protected virtual void MoveDownButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.window?.MoveDown(this);
        }
        protected virtual void RemoveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(MessageBox.Show($"Remve {this.GetType().Name}?","Confirm", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                this.window?.Remove(this);
            }
        }


        protected virtual void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            this.StartTime = DateTime.Now;
            this.Points.Clear();
            this.Line.Points = this.Points;
            this.Line.PlotOriginX = 0.0;
            this.Line.PlotOriginY = 0.0;
        }
        protected virtual void SyncPlot(double Y) => this.SyncPlot((DateTime.Now - this.StartTime).TotalSeconds, Y);
        protected virtual void SyncPlot(double X, double Y) => this.SyncPlot(new Point(X, Y));
        protected virtual void SyncPlot(Point p)
        {
            if (!this.IsPausing)
            {
                this.Points.Add(p);

                this.Line.Points = this.Points;

                if (this.Points.Count > 0)
                {
                    //plot width in seconds
                    double CurrentPlotWidth = this.Points[this.Points.Count - 1].X - this.Points[0].X;

                    if (CurrentPlotWidth > this.PlotWidth)
                    {
                        this.Line.PlotOriginX = CurrentPlotWidth - this.PlotWidth;
                    }
                }
            }
        }
    }
}
