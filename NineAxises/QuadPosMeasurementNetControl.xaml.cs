using InteractiveDataDisplay.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Probes
{
    /// <summary>
    /// QuadPosMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class QuadPosMeasurementNetControl : MeasurementBaseSerialControl
    {
        public override int ReceivePartLength => 24;
        protected override int LinesGroupLength => 2;
        public override string[] Headers => new string[] { };
        public override char EndOfLineChar => '\0';
        protected override int WriteTimeout => 100;
        protected override Grid LinesGrid => this.Lines0;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;

        protected DispatcherTimer CommandTimer = new DispatcherTimer();
        protected TimeSpan DefaultCommandInterval = TimeSpan.FromMilliseconds(40);
         
        ////115200,n,8,1
        protected const string GetTypeCommand = "#?type%";
        protected const string GetDataCommand = "#?data%";
        protected const string GetPosCommand = "#?pos%";
        protected const string SetUpdownCommand0 = "#SI:%02d%";
        protected const string SetUpdownCommand1 = "#SS:%03d%";

        public QuadPosMeasurementNetControl()
        {
            this.LinesGroup[0].Description = "X";
            this.LinesGroup[1].Description = "Y";

            this.LinesGroup[0].Stroke = Brushes.Red;
            this.LinesGroup[1].Stroke = Brushes.Blue;

            this.CommandTimer.Interval = DefaultCommandInterval;
            this.CommandTimer.Tick += Timer_Tick;
        }
        private void DrawLine(Canvas canvas, Point Sp, Point Ep)
        {
            if (canvas != null)
            {
                var linegemetry = new LineGeometry
                {
                    StartPoint = Sp,
                    EndPoint = Ep
                };
                var path = new Path
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Data = linegemetry
                };
                canvas.Children.Add(path);
            }
        }

        private void DrawCircle(Canvas canvas, Point center,double radius)
        {
            if (canvas != null)
            {
                var ellipseGeometry = new EllipseGeometry
                {
                    Center = center,
                    RadiusX = radius,
                    RadiusY = radius
                };
                var path = new Path
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Data = ellipseGeometry
                };
                canvas.Children.Add(path);
            }
        }
        protected override void CallInitializeComponent()
        {
            this.InitializeComponent();

            this.DrawLine(this.PositionCanvas, new Point(100.0, 0.0), new Point(100.0, 200.0));
            this.DrawLine(this.PositionCanvas, new Point(100.0, 0.0), new Point(90.0, 10.0));
            this.DrawLine(this.PositionCanvas, new Point(100.0, 0.0), new Point(110.0, 10.0));

            this.DrawLine(this.PositionCanvas, new Point(0.0, 100.0), new Point(200.0, 100.0));
            this.DrawLine(this.PositionCanvas, new Point(200.0, 100.0), new Point(190.0, 90.0));
            this.DrawLine(this.PositionCanvas, new Point(200.0, 100.0), new Point(190.0, 110.0));

            var cp = new Point(100.0, 100.0);

            for (int radius = 10; radius <= 100; radius += 10)
            {
                this.DrawCircle(this.PositionCanvas, cp, radius);
            }
            this.UpdatePortNames();
        }
        protected virtual void Timer_Tick(object sender, System.EventArgs e)
        {
            this.Send(GetDataCommand);
        }
        protected int Translate(byte[] data, uint start)
        {
            return data != null && data.Length >= start + 6
                ? (data[start + 0] << 20) | (data[start + 1] << 16) | (data[start + 2] << 12) | (data[start + 3] << 8) | (data[start + 4] << 4) | (data[start + 5] << 0)
                : 0;
        }
        
        protected override void OnReceivedInternal(string input)
        {
            if(input!=null && input.Length == 24)
            {
                var data = Encoding.ASCII.GetBytes(input);
                var valid = true;
                for(int i = 0; i < data.Length; i++)
                {
                    if (data[i] >= 48 && data[i] <= 48 + 15)
                    {
                        data[i] -= 48; //'0'
                    }
                    else
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    var D00 = this.Translate(data, 0);
                    var D01 = this.Translate(data, 6);
                    var D10 = this.Translate(data, 12);
                    var D11 = this.Translate(data, 18);
                    var DXX = (D00 + D01 + D10 + D11);

                    var x = DXX < 1000 ? 0.0 : (D01 + D11 - D00 - D10) * 1.0 / DXX;
                    var y = DXX < 1000 ? 0.0 : (D10 + D11 - D00 - D01) * 1.0 / DXX;

                    this.AddData(x,0,true);
                    this.AddData(y,1,true);

             
                    var px = 100.0 + x * 100.0; 
                    var py = 100.0 - y * 100.0;
                    
                    Canvas.SetLeft(EL, px - EL.Width / 2.0);
                    Canvas.SetTop(EL, py - EL.Height / 2.0);
                }
            }
        }


        protected override void SetRemoteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            base.SetRemoteCheckBox_Checked(sender, e);
            this.CommandTimer.Start();
        }
        protected override void SetRemoteCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            base.SetRemoteCheckBox_Unchecked(sender, e);
            this.CommandTimer.Stop();
        }
    }
}
