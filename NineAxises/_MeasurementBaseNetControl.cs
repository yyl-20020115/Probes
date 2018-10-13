using InteractiveDataDisplay.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public virtual string RemoteAddressText { get => this.RemoteAddressComboBox.SelectedItem.ToString(); set => this.RemoteAddressComboBox.SelectedItem = value; } 
        public virtual IPAddress RemoteAddress => IPAddress.TryParse(this.RemoteAddressText, out var add) ? add : IPAddress.None;
        protected IMeasurementNetWindow window = null;
        protected abstract CheckBox PauseCheckBox { get; }
        protected abstract CheckBox SetRemoteCheckBox { get; }
        protected abstract ComboBox RemoteAddressComboBox { get; }

        protected abstract Grid LinesGrid { get; }

        protected List<Point>[] PointsGroup = null;
        protected DateTime StartTime = DateTime.MinValue;
        protected virtual int LineGroupLength => 1;
        protected LineGraph[] LineGroup = null;
        protected double[] BaseZeroYGroup = null;
        protected double[] LastYGroup = null;
        public virtual double PlotWidth => 60.0; //60 seconds
        public virtual bool IsPausing => this.PauseCheckBox.IsChecked.GetValueOrDefault();
        protected OnReceiveDataDelegate OnReceivedCallback = null;
        public MeasurementBaseNetControl()
        {
            this.CallInitializeComponent();

            this.LineGroup = new LineGraph[this.LineGroupLength];

            for(int i = 0; i < this.LineGroup.Length; i++)
            {
                this.LinesGrid.Children.Add(this.LineGroup[i] = new LineGraph() { Stroke = Brushes.Blue, StrokeThickness = 1, IsAutoFitEnabled = true });
            }
            this.BaseZeroYGroup = new double[this.LineGroup.Length];
            this.LastYGroup = new double[this.LineGroup.Length];
            this.PointsGroup = new List<Point>[this.LineGroup.Length];
            for(int i = 0; i < this.PointsGroup.Length; i++)
            {
                this.PointsGroup[i] = new List<Point>();
            }
            this.OnReceivedCallback = new OnReceiveDataDelegate(this.OnReceivedInternal);

            this.SetRemoteCheckBox.IsChecked = true;
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
            for(int i = 0;i<this.LineGroup.Length;i++)
            {
                this.PointsGroup[i].Clear();
                this.LineGroup[i].Points = new PointCollection();
                this.LineGroup[i].PlotOriginX = 0.0;
                this.LineGroup[i].PlotOriginY = 0.0;
            }
        }
        protected virtual void AddData(double Y, int LineIndex = 0) => this.AddData((DateTime.Now - this.StartTime).TotalSeconds, Y,LineIndex);
        protected virtual void AddData(double X, double Y, int LineIndex = 0) => this.AddData(new Point(X, Y),LineIndex);
        protected virtual void AddData(Point p, int LineIndex = 0)
        {
            if (!this.IsPausing)
            {
                this.LastYGroup[LineIndex] = p.Y;
                this.PointsGroup[LineIndex].Add(p);
                this.UpdateLine(LineIndex);
            }
        }

        protected virtual void UpdateLines()
        {
            for(int i = 0; i < this.LineGroup.Length; i++)
            {
                this.UpdateLine(i);
            }
        }

        protected virtual void UpdateLine(int LineIndex)
        {
            this.LineGroup[LineIndex].Points = new PointCollection(this.PointsGroup[LineIndex].Select(
                pt => new Point(pt.X, pt.Y - this.BaseZeroYGroup[LineIndex])));

            if (this.PointsGroup[LineIndex].Count > 0)
            {
                //plot width in seconds
                double CurrentPlotWidth = this.PointsGroup[LineIndex][this.PointsGroup[LineIndex].Count - 1].X - this.PointsGroup[LineIndex][0].X;

                if (CurrentPlotWidth > this.PlotWidth)
                {
                    this.LineGroup[LineIndex].PlotOriginX = CurrentPlotWidth - this.PlotWidth;
                }
            }
        }
        protected virtual void BaseZeroYButton_Checked(object sender, RoutedEventArgs e)
        {
            Array.Copy(this.LastYGroup, this.BaseZeroYGroup, this.LastYGroup.Length);
            this.UpdateLines();
        }

        protected virtual void BaseZeroYButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Array.Clear(this.BaseZeroYGroup, 0, this.BaseZeroYGroup.Length);
            this.UpdateLines();
        }
        protected virtual void SetRemoteCheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.RemoteAddressComboBox.IsEnabled = false;
        }

        protected virtual void SetRemoteCheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.RemoteAddressComboBox.IsEnabled = true;
        }

    }
}
