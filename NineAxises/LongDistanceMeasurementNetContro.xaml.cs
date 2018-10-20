using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Probes
{
    /// <summary>
    /// LongDistanceMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class LongDistanceMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceivePartLength => 11;
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox; protected const int DefaultMeasurementInterval = 200;
        protected const int MaxDistance = 80000; //mm
        protected const int InvalidDistance = 0xffff;//-1 for short value
        protected Socket Client = null;
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
        public LongDistanceMeasurementNetControl()
        {
            this.LinesGroup[0].Description = "Distance in Meter";
        }


        protected override void CallInitializeComponent()
        {
            this.InitializeComponent();
        }

        public override bool OnConnectClient(Socket Client)
        {
            //this.PostReceiveBuffer(10); //60 d0 30 30 31 2e 38 32 30 9e
            this.Send(0x80, 0x06, 0x03, 0x77); //Continuing measurement 80 06 03 77
            return true;
        }

        protected override void OnReceivedInternal(byte[] data, int offset, int count)
        {
            if (data != null && data.Length==this.ReceivePartLength && data[offset+0]== 0x80 && data[offset + 1] == 0x06 && data[offset + 2] == 0x83)
            {
                byte sum = 0;
                for(int i = offset; i < offset + count-1; i++)
                {
                    sum += data[i];
                }
                sum &= 0xff;
                sum = (byte)~sum;
                sum++;
                if(sum == data[offset+count-1])
                {
                    //80 06 83 30 30 31 2e 38 32 37 97
                    string text = Encoding.ASCII.GetString(data, 3, 7);
                    if(double.TryParse(text,out var distance))
                    {
                        this.AddData(distance);
                    }
                }


            }
        }
    }
}
