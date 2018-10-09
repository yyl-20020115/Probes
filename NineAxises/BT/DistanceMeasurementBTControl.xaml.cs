using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Probes
{
    /// <summary>
    /// DistanceMeasurementBTControl.xaml 的交互逻辑
    /// </summary>
    public partial class DistanceMeasurementBTControl : MeasurementBaseBTControl
    {
        protected override int BaudRate => 9600;
        protected override bool UseComEvents => false;
        protected override ComboBox ComPortsComboBox => this._ComPortsComboBox;
        protected override CheckBox ConnectCheckBox => this._ConnectCheckBox;
        protected override CheckBox PauseCheckBox => this._PauseCheckBox;

        protected DispatcherTimer Timer = new DispatcherTimer();

        protected const double DefaultSampleInterval = 0.2;
        protected override double SampleInterval { get; set; } = DefaultSampleInterval;
        /*
         * 3 WWW.WX-RCWL.COM
            接口定义:
            序号 接口定义 说明
            1 Vcc 供电电源
            2 TX 模块串口输出
            3 RX 模块串口输入
            4 Gnd 地
            测量命令:
            命令 返回值 说明
            0XA0 BYTE1 BYTE2 单次输出模式 输出距离为（BYTE1<<8）+ BYTE2 转成10 进制即为实际测试距离，单位mm
            0XA1 BYTE1 BYTE2 VL53L0X 初始化(初始化时间为500mS)
            0XB0 0xB0 模块波特率设置为9600，即时生效（默认）
            0XB1 0xB1 模块波特率设置为19200，即时生效
            0XB2 0xB2 模块波特率设置为115200，即时生效
            0xC0 0xC0 设置为长距离测量模式（默认）
            0xC1 0xC1 设置为高速测量模式
            0xC2 0xC2 设置为高精度测量模式（测量间隔需大于180ms）
            0xD0 0xD0 设置XSHUT 为高电平，模块正常工作并初始化模块
            0xD1 0xD1 设置XSHUT 为低电平，VL53L0X 关闭
            0xF0 BYTE1 BYTE2 BYTE3 BYTE4 BYTE5 BYTE6   BYTE1 - BYTE4:当前波特率(MSB）。 BYTE5：0X00 长距离模式 0X01 高速测量模式 0X02 高精度测量模式 BYTE6： 当前XSHUT 的状态 
            0xF1 公司及版本信息
         */
        protected const byte COMMAND_MEASURE_DISTANCE = 0xA0; //RET:2 BYTES
        protected const byte COMMAND_INITIALIZE = 0xA1; //RET:2 BYTES
        protected const byte COMMAND_SET_BAUD_9600 = 0xB0; //RET:1 BYTE
        protected const byte COMMAND_SET_BAUD_19200 = 0xB1; //RET:1 BYTE
        protected const byte COMMAND_SET_BAUD_115200 = 0xB2; //RET:1 BYTE
        protected const byte COMMAND_SET_LONG_MODE = 0xC0; //RET:1 BYTE
        protected const byte COMMAND_SET_FAST_MODE = 0xC1; //RET:1 BYTE
        protected const byte COMMAND_SET_ACCURATE_MODE = 0xC2; //RET:1 BYTE
        protected const byte COMMAND_XSHUT_HIGH = 0xD0; //RET:1 BYTE
        protected const byte COMMAND_XSHUT_LOW = 0xD1; //RET:1 BYTE
        protected const byte COMMAND_GET_CONFIG = 0xF0; //RET:6 BYTES
        protected const byte COMMAND_GET_VERSION_INFO = 0xF1; //RET:UNSURE BYTES WITH \n AS TERMINATER

        public DistanceMeasurementBTControl()
        {
            InitializeComponent();
            this.TimeDelayTextBox.Text = ((int)(this.SampleInterval * 1000.0)).ToString();
            this.Timer.Interval = TimeSpan.FromMilliseconds(this.SampleInterval);
            this.Timer.Tick += Timer_Tick;
        }

        protected virtual void Timer_Tick(object sender, EventArgs e)
        {
            if (this.IsClosing)
            {
                this.Timer.Stop();
                return;
            }
            if(this.ComPort!=null && this.ComPort.IsOpen)
            {
                this.SyncPlot(this.GetDistance(this.SendCommand(COMMAND_MEASURE_DISTANCE,2)));
            }
        }
        protected virtual byte[] SendCommand(byte c, int ExpectedReturnBytes = 1)
        {
            byte[] ReturnBytes = new byte[ExpectedReturnBytes>0?ExpectedReturnBytes:0];
            if(this.ComPort!=null && this.ComPort.IsOpen)
            {
                this.ComPort.BaseStream.WriteByte(c);

                for(int i = 0; i < ExpectedReturnBytes; i++)
                {
                    int _Byte = this.ComPort.ReadByte();
                    if (_Byte == -1) break;
                    ReturnBytes[i] = (byte)_Byte;
                }
            }
            return ReturnBytes;
        }
        protected virtual int GetDistance(byte[] bytes)
        {
            return bytes != null && bytes.Length == 2 ? (((int)bytes[0]) << 8 | (int)bytes[1]) : -1;
        }

        protected virtual void SetParametersButton_Click(object sender, RoutedEventArgs e)
        {
            int Interval = (int)DefaultSampleInterval * 1000;
            if(int.TryParse(this.TimeDelayTextBox.Text,out Interval))
            {
                this.SampleInterval = Interval / 1000.0;
                this.Timer.Interval = TimeSpan.FromSeconds(this.SampleInterval);
            }
            else
            {
                this.TimeDelayTextBox.Text = Interval.ToString();
            }
        }
        protected override void ConnectCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            base.ConnectCheckBox_Checked(sender, e);
            this.SendCommand(COMMAND_INITIALIZE, 2);
            Task.Delay(600);
            this.SendCommand(COMMAND_SET_ACCURATE_MODE, 1);
            this.Timer.Start();
        }
        protected override void ConnectCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Timer.Stop();
            base.ConnectCheckBox_Unchecked(sender, e);
        }
    }
}
