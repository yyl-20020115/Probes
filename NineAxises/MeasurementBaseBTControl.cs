using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using InteractiveDataDisplay.WPF;

namespace Probes
{
    /// <summary>
    /// MeasurementBaseControl.xaml 的交互逻辑
    /// </summary>
    public abstract partial class MeasurementBaseBTControl : UserControl, IMeasurementBTControl
    {
        protected PointCollection Points = new PointCollection();
        protected bool IsErrorReceiving = false;
        protected bool IsDataReceiving = false;
        protected bool IsClosing = false;
        protected SerialPort ComPort = null;
        public virtual string CurrentComPortName { get; set; } = string.Empty;
        protected abstract int ReadBufferSize { get; }
        protected virtual int WriteBufferSize { get; } = 2;
        protected abstract ComboBox ComPortsComboBox { get; }
        protected abstract CheckBox PauseCheckBox { get; }
        protected abstract CheckBox ConnectCheckBox { get; }

        protected DateTime StartTime = DateTime.Now;
        protected LineGraph Line = new LineGraph();
        protected IMeasurementBTHub Hub = null;
        protected virtual double SampleInterval => 1.0;
        protected virtual int SamplePointsPerWindow => 256;
        public virtual double PlotWidth => this.SampleInterval * this.SamplePointsPerWindow;
        public virtual bool IsPausing => this.PauseCheckBox.IsChecked.HasValue && this.PauseCheckBox.IsChecked.Value;
        public virtual bool IsConnected => this.ConnectCheckBox.IsChecked.HasValue && this.ConnectCheckBox.IsChecked.Value;

        public MeasurementBaseBTControl()
        {
            this.Line.IsAutoFitEnabled = true;
        }
        public virtual void ConnectHub(IMeasurementBTHub hub)
        {
            this.Hub = hub;
        }

        public virtual void Dispose()
        {
            try
            {
                this.IsClosing = true;
                while (this.IsErrorReceiving ||this.IsDataReceiving) this.DoEvent();
                this.ComPort?.Dispose();
            }
            catch { }
            finally
            {
                this.ComPort = null;
                this.Hub?.DisconnectComPort(this, this.CurrentComPortName);
                this.CurrentComPortName = string.Empty;
            }
        }

        public virtual void SetComPortsList(List<string> ComPortsList)
        {
            this.ComPortsComboBox.Items.Clear();
            if (ComPortsList != null && ComPortsList.Count > 0)
            {
                ComPortsList.ForEach(c => this.ComPortsComboBox.Items.Add(c));
                if (ComPortsList.Count > 0)
                {
                    if(!string.IsNullOrEmpty(this.CurrentComPortName) && ComPortsList.Contains(this.CurrentComPortName))
                    {
                        this.ComPortsComboBox.SelectedItem = this.CurrentComPortName;
                    }
                    else
                    {
                        this.ComPortsComboBox.SelectedIndex = 0;
                    }
                }
            }
        }
        protected virtual void ConnectCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.ComPortsComboBox.SelectedItem is string cp)
            {
                if (this.ComPort != null)
                {
                    try
                    {
                        this.IsClosing = true;
                        while (this.IsErrorReceiving || this.IsDataReceiving) this.DoEvent();
                        this.ComPort?.Dispose();
                    }
                    catch { }
                    finally
                    {
                        this.ComPort = null;
                        this.Hub?.ConnectComPort(this, this.CurrentComPortName);
                    }
                }

                try
                {
                    this.IsClosing = false;
                    this.ComPort = new SerialPort
                    {
                        PortName = cp,
                        Encoding = Encoding.ASCII,
                        BaudRate = 115200,
                        WriteBufferSize = this.WriteBufferSize,
                        ReadBufferSize = this.ReadBufferSize,
                        DataBits = 8,
                        NewLine = "\n",
                        Parity = Parity.None,
                        StopBits = StopBits.One
                    };
                    this.ComPort.DataReceived += DataReceived;
                    this.ComPort.ErrorReceived += ErrorReceived;
                    this.ComPort.Open();

                    this.CurrentComPortName = cp;
                    this.Hub?.ConnectComPort(this, this.CurrentComPortName);
                    this.ComPortsComboBox.IsEnabled = false;
                    this.StartTime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    try
                    {
                        this.ComPort.Dispose();
                    }
                    catch
                    {

                    }
                    finally
                    {
                        this.ComPort = null;
                    }
                }
            }

        }
        /// <summary>
        /// 模仿C#的Application.Doevent函数。可以适当添加try catch 模块
        /// </summary>
        protected virtual void DoEvent()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }
        protected virtual object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;
            return null;
        }

        protected virtual void ConnectCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsClosing = true;
                while (this.IsErrorReceiving || this.IsDataReceiving) this.DoEvent();
                this.ComPort?.Dispose();
            }
            catch { }
            finally
            {
                this.ComPort = null;
                this.Hub?.DisconnectComPort(this, this.CurrentComPortName);
                this.CurrentComPortName = string.Empty;
                this.ComPortsComboBox.IsEnabled = true;
            }
        }
        protected virtual void ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            if (this.IsClosing) return;
            try
            {
                this.IsErrorReceiving = true;
                this.ComPort_ErrorReceived(sender, e);
            }
            catch { }
            finally
            {
                this.IsErrorReceiving = false;
            }
        }
        protected virtual void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (this.IsClosing) return;
            try
            {
                this.IsDataReceiving = true;
                this.ComPort_DataReceived(sender, e);
            }
            catch { }
            finally
            {
                this.IsDataReceiving = false;
            }
        }


        protected abstract void ComPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e);
        protected abstract void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e);

        protected virtual void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            this.StartTime = DateTime.Now;
            this.Points.Clear();
            this.Line.Points = this.Points;
            this.Line.PlotOriginX = 0.0;
            this.Line.PlotOriginY = 0.0;

        }

        protected virtual void CenterYButton_Click(object sender, RoutedEventArgs e)
        {
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
        protected virtual void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hub?.CloseControl(this);
        }
        protected virtual void SyncPlot(double X, double Y) => this.SyncPlot(new Point(X, Y));
        protected virtual void SyncPlot(Point p)
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
