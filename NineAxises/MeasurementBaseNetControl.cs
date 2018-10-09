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

        public abstract int ReceiveBufferLength { get; }
        protected abstract string RemoteAddressText { get; }
        public virtual IPAddress RemoteAddress => IPAddress.TryParse(this.RemoteAddressText, out var add) ? add : IPAddress.None;
        protected IMeasurementNetWindow window = null;
        protected abstract CheckBox PauseCheckBox { get; }
        protected abstract Grid LinesGrid { get; }

        protected PointCollection Points = new PointCollection();
        protected DateTime StartTime = DateTime.MinValue;
        protected LineGraph Line = new LineGraph();

        protected virtual double SampleInterval=> 0.01;//10ms
        protected virtual int SamplePointsPerWindow => 256;
        public virtual double PlotWidth => this.SampleInterval * this.SamplePointsPerWindow;
        public virtual bool IsPausing => this.PauseCheckBox.IsChecked.HasValue && this.PauseCheckBox.IsChecked.Value;

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
           
        public virtual void InitWith(IMeasurementNetWindow window)
        {
            this.window = window;
        }
        public virtual void OnConnect(Socket Client)
        {

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

        protected virtual void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            this.StartTime = DateTime.Now;
            this.Points.Clear();
            this.Line.Points = this.Points;
            this.Line.PlotOriginX = 0.0;
            this.Line.PlotOriginY = 0.0;
        }
        protected virtual void PauseCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.PauseCheckBox.Content = "Resume";
        }
        protected virtual void PauseCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.PauseCheckBox.Content = "Pause";
        }
        protected virtual void SyncPlot(double Y) => this.SyncPlot((DateTime.Now - this.StartTime).TotalSeconds, Y);
        protected virtual void SyncPlot(double X, double Y) => this.SyncPlot(new Point(X, Y));
        protected virtual void SyncPlot(Point p)
        {
            if (!this.IsPausing)
            {
                this.Points.Add(p);

                this.Line.Points = this.Points;

                double CurrentPlotWidth = this.Line.Points.Count * this.SampleInterval;

                if (CurrentPlotWidth > this.PlotWidth)
                {
                    this.Line.PlotOriginX = CurrentPlotWidth - this.PlotWidth;
                }
            }
        }
    }
}
