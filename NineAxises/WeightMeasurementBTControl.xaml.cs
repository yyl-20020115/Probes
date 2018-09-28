using System;
using System.IO.Ports;
using System.Windows.Controls;
using System.Windows.Media;

namespace Probes
{
    /// <summary>
    /// WeightMeasurementControl.xaml 的交互逻辑
    /// </summary>
    public partial class WeightMeasurementBTControl :MeasurementBaseBTControl
    {
        protected delegate void InputDelegate(int value, int middle, int r2, int r1, int r0);
        protected InputDelegate InputMethod = null;
        protected override int ReadBufferSize { get; } = 52;
        protected override ComboBox ComPortsComboBox => this._ComPortsComboBox;
        protected override CheckBox ConnectCheckBox => this._ConnectCheckBox;
        protected override CheckBox PauseCheckBox => this._PauseCheckBox;
        protected override double SampleInterval => 0.068;//68ms
        public WeightMeasurementBTControl()
        {
            InitializeComponent();
            this.InputMethod = new InputDelegate(this.Input);
            this.Line.Stroke = Brushes.Blue;
            this.Line.Description = "Weight in Gram";
            this.Line.StrokeThickness = 1;
            this.Lines.Children.Add(this.Line);
        }


        protected override void ComPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {

        }

        protected override void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == SerialData.Chars)
            {
                var line = this.ComPort?.ReadLine() ?? string.Empty;
                if (line.StartsWith("WEIGHT:"))
                {
                    var parts = line.Substring(7).TrimEnd().Split(',');
                    if (parts.Length == 5)
                    {
                        if (!int.TryParse(parts[0], System.Globalization.NumberStyles.HexNumber, null, out int value))
                        {
                            value = 0;
                        }
                        if (!int.TryParse(parts[1], System.Globalization.NumberStyles.HexNumber, null, out int middle))
                        {
                            middle = 0;
                        }
                        if (!int.TryParse(parts[2], System.Globalization.NumberStyles.HexNumber, null, out int r2))
                        {
                            r2 = 200;
                        }
                        if (!int.TryParse(parts[3], System.Globalization.NumberStyles.HexNumber, null, out int r1))
                        {
                            r1 = 100;
                        }
                        if (!int.TryParse(parts[4], System.Globalization.NumberStyles.HexNumber, null, out int r0))
                        {
                            r0 = 0;
                        }
                        Dispatcher.BeginInvoke(this.InputMethod, value, middle, r2, r1, r0);
                    }
                }
            }
            else if (e.EventType == SerialData.Eof)
            {

            }
        }

        public virtual void Input(int value,int middle,int r2,int r1, int r0)
        {
            if (!this.IsPausing)
            {
                double Weight = (r2 != r1) ? (value - r0) / (double)(r2 - r1) * 100.0 : 0.0;

                var dt = DateTime.Now - this.StartTime;

                this.SyncPlot(dt.TotalSeconds,Weight);
            }
        }
    }
}
