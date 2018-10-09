using System;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Probes
{
    /// <summary>
    /// TimeMeasurementControl.xaml 的交互逻辑
    /// </summary>
    public partial class TimeMeasurementBTControl : MeasurementBaseBTControl
    {
        public virtual double ScaleFactor { get; set; } = 1e6;
        public const int DefaultTimeSliceValue = 100;
        public const int DefaultTimeDelayValue = 20;
        protected delegate void InputDelegate(int tc2, int tb2, int tc1, int tb1);
        protected InputDelegate InputMethod = null;
        protected bool StartState = false;
        protected DispatcherTimer CommandTimer = new DispatcherTimer();
        protected int _TimeSliceValue = DefaultTimeSliceValue;
        protected int _TimeDelayValue = DefaultTimeDelayValue;
        public int TimeSliceValue => this._TimeSliceValue;
        public int TimeDelayValue => this._TimeDelayValue;

        protected override int ReadBufferSize { get; } = 42;
        protected override double SampleInterval => (this.TimeSliceValue + this.TimeDelayValue) /1000.0;
        protected override ComboBox ComPortsComboBox => this._ComPortsComboBox;

        protected override CheckBox PauseCheckBox => this._PauseCheckBox;

        protected override CheckBox ConnectCheckBox => this._ConnectCheckBox;

        public TimeMeasurementBTControl()
        {
            InitializeComponent();
            this.InputMethod = new InputDelegate(this.Input);
            this.Line.Stroke = Brushes.Blue;
            this.Line.Description = "dt=Probe Time - Master Time";
            this.Line.StrokeThickness = 1;
            this.Lines.Children.Add(this.Line);
            this.CommandTimer.Tick += Timer_Tick;
            this.CommandTimer.Interval = TimeSpan.FromMilliseconds(this._TimeSliceValue);
            this.CommandTimer.Start();
        }
        public override void Dispose()
        {
            this.CommandTimer.Stop();
            base.Dispose();
        }
        protected virtual void Timer_Tick(object sender, EventArgs e)
        {
            if (this._TimeSliceValue != this.CommandTimer.Interval.Milliseconds)
            {
                this.CommandTimer.Interval = TimeSpan.FromMilliseconds(this._TimeSliceValue);
            }
            if (this.ComPort!=null && this.ComPort.IsOpen)
            {
                if (!this.StartState)
                {
                    this.SendCommand('B'); 
                    this.StartState = true;
                }
                else
                {
                    this.SendCommand('E');
                    this.StartState = false;

                    if (this._TimeDelayValue > 0)
                    {
                        this.CommandTimer.Stop();
                        Task.Delay(this._TimeDelayValue);
                        if (!this.IsPausing)
                        {
                            this.CommandTimer.Start();
                        }
                    }
                    else if (this.IsPausing)
                    {
                        this.CommandTimer.Stop();
                    }
                }

            }
        }
        protected virtual void SendCommand(char c)
        {
            if (this.ComPort != null && this.ComPort.IsOpen)
            {
                try
                {
                    this.ComPort?.BaseStream.WriteByte((byte)c);
                }
                catch
                {
                }
            }
        }
        protected virtual void SetParametersButton_Click(object sender, RoutedEventArgs e)
        {
            if(!int.TryParse(this.TimeSliceTextBox.Text,out this._TimeSliceValue) )
            {
                this._TimeSliceValue = DefaultTimeSliceValue;
                this.TimeSliceTextBox.Text = this._TimeSliceValue.ToString();
            }
            if (!int.TryParse(this.TimeDelayTextBox.Text, out this._TimeDelayValue) || this._TimeDelayValue < DefaultTimeDelayValue)
            {
                this._TimeDelayValue = DefaultTimeDelayValue;
                this.TimeDelayTextBox.Text = this._TimeDelayValue.ToString();
            }
        }

        public void Input(int tc2, int tb2, int tc1,int tb1)
        {
            tb2 = tb2 > 0 ? tb2 : 1;
            tb1 = tb1 > 0 ? tb1 : 1;
            double t2 = tc2 / (double)tb2;
            double t1 = tc1 / (double)tb1;

            this.Input(t2, t1);
        }
        public void Input(double t2, double t1) => this.SyncPlot((t2 - t1) * this.ScaleFactor);

        protected override void PauseCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            base.PauseCheckBox_Unchecked(sender, e);
            if (!this.CommandTimer.IsEnabled)
            {
                this.CommandTimer.Start();
            }
        }

        protected override void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == SerialData.Chars)
            {
                var line = this.ComPort?.ReadLine() ?? string.Empty;

                if (line.StartsWith("_TIME:"))
                {
                    var parts = line.Substring(6).TrimEnd().Split(',');

                    if (parts.Length == 4)
                    {
                        if (!int.TryParse(parts[0], System.Globalization.NumberStyles.HexNumber, null, out int t2))
                        {
                            t2 = 0;
                        }
                        if (!int.TryParse(parts[1], System.Globalization.NumberStyles.HexNumber, null, out int b2))
                        {
                            b2 = 1;
                        }
                        if (!int.TryParse(parts[2], System.Globalization.NumberStyles.HexNumber, null, out int t1))
                        {
                            t1 = 0;
                        }
                        if (!int.TryParse(parts[3], System.Globalization.NumberStyles.HexNumber, null, out int b1))
                        {
                            b1 = 1;
                        }
                        Dispatcher.BeginInvoke(this.InputMethod, t2, b2, t1, b1);
                    }
                }
            }
            else if (e.EventType == SerialData.Eof)
            {

            }
        }
    }
}
