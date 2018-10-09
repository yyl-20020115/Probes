﻿using System.Net.Sockets;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Probes
{
    /// <summary>
    /// DistanceMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class DistanceMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceiveBufferLength => 2;
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;

        protected override string RemoteAddressText => "192.168.1.67";
        public virtual int InitRetries { get; set; } = 10;

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

        protected DispatcherTimer Timer = new DispatcherTimer();
        protected const int DefaultMeasurementInterval = 200;
        public DistanceMeasurementNetControl()
        {
            this.Line.Description = "Distance in Millimeter";
            this.Timer.Interval = System.TimeSpan.FromMilliseconds(DefaultMeasurementInterval);
            this.Timer.Tick += Timer_Tick;
        }

        protected void Timer_Tick(object sender, System.EventArgs e)
        {
            this.Send(COMMAND_MEASURE_DISTANCE);
        }

        protected override void CallInitializeComponent()
        {
            this.InitializeComponent();
        }

        public override void OnConnect(Socket Client)
        {
            if (Client != null)
            {
                var SendBuffer = new byte[1];
                var ReceiveBuffer = new byte[2];
                var st = Client.SendTimeout;
                var rt = Client.ReceiveTimeout;
                Client.SendTimeout = 500;
                Client.ReceiveTimeout = 500;
                for (int i = 0; i<this.InitRetries; i++)
                {
                    //SendBuffer[0] = COMMAND_INITIALIZE;
                    //Client.Send(SendBuffer, 0, 1, SocketFlags.None);
                    ////get 2 return bytes.
                    //Client.Receive(ReceiveBuffer, 0, 2, SocketFlags.None);

                    SendBuffer[0] = COMMAND_SET_ACCURATE_MODE;
                    Client.Send(SendBuffer,0,1, SocketFlags.None);
                    Client.Receive(ReceiveBuffer,0,1, SocketFlags.None);
                    if (ReceiveBuffer[0] == SendBuffer[0])
                    {
                        Client.SendTimeout = st;
                        Client.ReceiveTimeout = rt;
                        this.Timer.Start();
                        break;
                    }
                }

            }
        }

        protected override void OnReceivedInternal(byte[] data, int offset, int count)
        {
            if(data.Length == 2)
            {
                this.SyncPlot(((((int)data[offset]) << 8) | (int)data[offset+1]));
            }
        }
    }
}
