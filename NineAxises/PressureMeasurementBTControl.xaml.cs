using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Probes
{
    /// <summary>
    /// PressureMeasurementControl.xaml 的交互逻辑
    /// </summary>
    public partial class PressureMeasurementBTControl : MeasurementBaseBTControl, IMeasurementBTControl
    {
        protected delegate void InputDelegate(int Data);
        protected InputDelegate InputMethod = null;
        protected override double SampleInterval => 0.020;//20ms
        protected override ComboBox ComPortsComboBox => this._ComPortsComboBox;
        protected override CheckBox ConnectCheckBox => this._ConnectCheckBox;
        protected override CheckBox PauseCheckBox => this._PauseCheckBox;
        protected override int ReadBufferSize { get; } = 18;

        protected double RelativeZeroY = double.NaN;
        protected bool ResetRelativeZeroY = true; //set first value to be zero value
        public virtual bool IsReciprocal => this.ReciprocalCheckBox.IsChecked.HasValue && this.ReciprocalCheckBox.IsChecked.Value;
        public PressureMeasurementBTControl()
        {
            InitializeComponent();
            this.InputMethod = new InputDelegate(this.Input);
            this.Line.Stroke = Brushes.Blue;
            this.Line.Description = "Pressure ADC value";
            this.Line.StrokeThickness = 1;
            this.Lines.Children.Add(Line);
        }

        public virtual void Input(int Data)
        {
            if (!this.IsPausing)
            {
                bool LED = (Data >> 16) == 0;

                this.LED.Fill = LED ? Brushes.Green : Brushes.Gray;

                int Value = Data & 0x0FFF;

                double Y = Value / 4096.0;
                double X = (DateTime.Now - this.StartTime).TotalSeconds;

                if (this.IsReciprocal)
                {
                    Y = 1.0 / Y;
                }
                if (this.ResetRelativeZeroY)
                {
                    this.RelativeZeroY = Y;
                    this.ResetRelativeZeroY = false;
                }

                if (!double.IsNaN(this.RelativeZeroY))
                {
                    Y -= this.RelativeZeroY;
                }

                this.SyncPlot(X, Y);
            }
        }
  
        protected override void ComPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
        }
        protected override void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == SerialData.Chars)
            {
                var line = this.ComPort?.ReadLine() ?? string.Empty;
                if (line.StartsWith("PRESSURE:"))
                {
                    if (int.TryParse(line.Substring(9, 8), System.Globalization.NumberStyles.HexNumber, null, out int Data))
                    {
                        Dispatcher.BeginInvoke(this.InputMethod, Data);
                    }
                }
            }
            else if (e.EventType == SerialData.Eof)
            {

            }
        }

        protected virtual void ReciprocalCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.UpdateLayout();
        }
        protected virtual void ReciprocalCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.UpdateLayout();
        }

        private void ResetRelativeZeroYButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
