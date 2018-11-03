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
    public abstract partial class MeasurementBaseNetControl : UserControl, IMeasurementNetControl
    {
        public delegate void OnReceiveDataDelegate(byte[] data, int offset, int count);

        public virtual int ReceiveBufferLength { get { return this.ReceivePartLength * this.ReceivePartCount; } set { } }
        public virtual int ReceivePartCount { get; set; } = 1;
        public virtual int ReceivePartLength { get; set; } = 1;
        public virtual string[] Headers => new string[0];
        public virtual string RemoteAddressText => this.RemoteAddressComboBox.Text;
        public virtual IPAddress RemoteAddress => IPAddress.TryParse(this.RemoteAddressText, out var add) ? add : IPAddress.None;
        protected IMeasurementNetWindow window = null;
        protected abstract CheckBox PauseCheckBox { get; }
        protected abstract CheckBox SetRemoteCheckBox { get; }
        protected abstract ComboBox RemoteAddressComboBox { get; }
        protected abstract Grid LinesGrid { get; }
        protected DateTime StartTime => this.window.StartTime;
        protected virtual int LinesGroupLength => 1;
        protected LineGraph[] LinesGroup = null;
        protected double[] BaseZeroYGroup = null;
        protected double[] LastYGroup = null;
        protected List<Point>[] PointsGroup = null;
        protected Dictionary<LineGraph,List<Point> > LinePointsDict = new Dictionary<LineGraph,List<Point>>();
        public virtual double PlotWidth => 60.0; //60 seconds
        public virtual bool IsPausing => this.PauseCheckBox.IsChecked.GetValueOrDefault();
        protected string TextBuffer = string.Empty;
        protected OnReceiveDataDelegate OnReceivedCallback = null;
        public MeasurementBaseNetControl()
        {
            this.CallInitializeComponent();

            this.LinesGroup = new LineGraph[this.LinesGroupLength];
            this.PointsGroup = new List<Point>[this.LinesGroup.Length];

            for(int i = 0; i < this.LinesGroup.Length; i++)
            {
                var lg = this.CreateLineGraphInstance();
                this.LinesGrid.Children.Add(this.LinesGroup[i] =lg);
                this.LinePointsDict.Add(lg, this.PointsGroup[i] = new List<Point>());
            }
            this.BaseZeroYGroup = new double[this.LinesGroup.Length];
            this.LastYGroup = new double[this.LinesGroup.Length];

            this.OnReceivedCallback = new OnReceiveDataDelegate(this.OnReceivedInternal);

            this.SetRemoteCheckBox.IsChecked = true;
        }
        protected virtual LineGraph CreateLineGraphInstance()
        {
            var lg = new LineGraph()
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 1,
                IsAutoFitEnabled = true,
            };
            lg.MouseMove += Lg_MouseMove;
            return lg;
        }
        protected virtual void Lg_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(sender is LineGraph lg)
            {
                var p = e.GetPosition(lg);
                var x = lg.XFromLeft(p.X);
                var lp = this.LinePointsDict[lg];
                Point? cp = null;
                if (lp != null)
                {
                    for(int i = 0; i < lp.Count-1; i++)
                    {
                        if(x>=lp[i].X && x < lp[i+1].X)
                        {
                            cp = lp[i];
                            break;
                        }
                    }
                }
                if(cp.HasValue)
                {
                    this.window?.ReportStatus(string.Format("Time:{0}, Value:{1}",
                         (cp.Value.X.ToString().PadRight(12, '0')),
                         (cp.Value.Y.ToString().PadRight(18, '0'))));
                }
            }
        }

        protected abstract void CallInitializeComponent();
  
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
            Dispatcher.BeginInvoke(this.OnReceivedCallback, data, offset, count);
        }
        protected virtual void OnReceivedInternal(byte[] data, int offset, int count)
        {
            this.OnReceivedInternal(Encoding.ASCII.GetString(data, offset, count),this.Headers,this.ReceivePartLength);
        }
        protected virtual void OnReceivedInternal(string text, string[] headers, int length)
        {
            if (!string.IsNullOrEmpty(text) && length > 0)
            {
                TextBuffer += text;
                var i = 0;
                while ((i = this.FindFirstIndexInside(TextBuffer,headers)) >= 0)
                {
                    if ((TextBuffer.Length - i) >= length && (TextBuffer[i + length - 1] == '\n' || TextBuffer[i + length - 1] == '\0'))
                    {
                        this.OnReceivedInternal(TextBuffer.Substring(i, length));
                        TextBuffer = TextBuffer.Substring(i + length);
                    }
                    else
                    {
                        TextBuffer = string.Empty;
                        break;
                    }
                }
            }
        }
        protected virtual int FindFirstIndexInside(string text,string[] headers)
        {
            if(!string.IsNullOrEmpty(text) && headers != null)
            {
                foreach(var h in headers)
                {
                    if (!string.IsNullOrEmpty(h))
                    {
                        var t = text.IndexOf(h);
                        if (t >= 0)
                        {
                            return t;
                        }
                    }
                }
            }
            return -1;
        }
        protected virtual void OnReceivedInternal(string input)
        {

        }
        public virtual void OnSendComplete(byte[] data,int offset,int count)
        {

        }
        protected virtual void Send(string command) => this.Send(Encoding.ASCII.GetBytes(command));

        protected virtual void Send(params byte[] data) => this.window?.Send(this, data);
        protected virtual void RemoveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(MessageBox.Show($"Remve {this.GetType().Name}?","Confirm", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                this.window?.Remove(this);
            }
        }

        protected virtual void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            this.Reset();
        }
        public virtual void Reset()
        {
            for (int i = 0; i < this.LinesGroup.Length; i++)
            {
                this.PointsGroup[i].Clear();
                this.LinesGroup[i].Points = new PointCollection();
                this.LinesGroup[i].PlotOriginX = 0.0;
                this.LinesGroup[i].PlotOriginY = 0.0;
            }
        }
        protected virtual void AddData(double Y, int LineIndex = 0, bool Update = true) => this.AddData((DateTime.Now - this.StartTime).TotalSeconds, Y,LineIndex,Update);
        protected virtual void AddData(double X, double Y, int LineIndex = 0, bool Update = true) => this.AddData(new Point(X, Y),LineIndex,Update);
        protected virtual void AddData(Point p, int LineIndex = 0, bool Update = true)
        {
            if (!this.IsPausing)
            {
                this.LastYGroup[LineIndex] = p.Y;
                this.PointsGroup[LineIndex].Add(p);
                if (Update)
                {
                    this.UpdateLine(LineIndex);
                }
            }
        }

        protected virtual void UpdateLines()
        {
            for(int i = 0; i < this.LinesGroup.Length; i++)
            {
                this.UpdateLine(i);
            }
        }

        protected virtual void UpdateLine(int LineIndex)
        {
            this.LinesGroup[LineIndex].Points = new PointCollection(this.PointsGroup[LineIndex].Select(
                pt => new Point(pt.X, pt.Y - this.BaseZeroYGroup[LineIndex])));

            if (this.PointsGroup[LineIndex].Count > 0)
            {
                var lastX = this.PointsGroup[LineIndex][this.PointsGroup[LineIndex].Count - 1].X;
                var firstX = this.PointsGroup[LineIndex][0].X; 
                //plot width in seconds
                var CurrentPlotWidth = lastX - firstX;

                if (CurrentPlotWidth > this.PlotWidth)
                {
                    this.LinesGroup[LineIndex].PlotOriginX = lastX - this.PlotWidth;
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
            this.window?.ConnectClient(this);
        }

        protected virtual void SetRemoteCheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.window?.DisconnectClient(this);
            this.RemoteAddressComboBox.IsEnabled = true;
        }

        public virtual void Dispose()
        {
        }
    }
}
