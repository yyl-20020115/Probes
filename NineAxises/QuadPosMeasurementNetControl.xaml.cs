using System;
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
    public partial class QuadPosMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceivePartLength => 24;
        protected override int LinesGroupLength => 2;
        public override string[] Headers => new string[] { };
        public override char EndOfLineChar => '\0';
            
        protected override Grid LinesGrid => this.Lines;
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
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }
        protected virtual void Timer_Tick(object sender, System.EventArgs e)
        {
            this.Send(GetDataCommand);
        }
        protected int Translate(byte[] data, uint start)
        {
            return data!=null && data.Length>=start + 6  
                ? (data[start + 0] << 20) | (data[start + 1] << 16) | (data[start + 2] << 12) | (data[start + 3] << 8) | (data[start+4]<<4)|data[start + 5]<<0 
                : 0;
        }
        protected override void OnReceivedInternal(string input)
        {
            if(input!=null && input.Length == 24)
            {
                byte[] data = Encoding.ASCII.GetBytes(input);
                bool valid = true;
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
                    double D00 = this.Translate(data, 0);
                    double D01 = this.Translate(data, 6);
                    double D10 = this.Translate(data, 12);
                    double D11 = this.Translate(data, 18);
                    double DXX = D00 + D01 + D10 + D11;

                    double x = (D01 + D11 - D00 - D10) / DXX;
                    double y = (D10 + D11 - D00 - D01) / DXX;


                    this.AddData(x, LineIndex: 0, Update: true);
                    this.AddData(y, LineIndex: 1, Update: true);

                    double px = 100.0 + x * 100.0;
                    double py = 100.0 + y * 100.0;
                    

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
