using System.Collections.Generic;
using System.Net.Sockets;
using System.Windows.Controls;

namespace Probes
{
    /// <summary>
    /// AmpMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class AmpMeasurementNetControl : MeasurementBaseNetControl
    {
        #region Useful constants
        //AA 55 Length Code Data SumHigh SumLow

        protected const byte CMD_CONNECT        = 0xF1; //控制链接（连续方式）
        protected const byte  CMD_UNCONNECT   = 0xF2; //控制断开
        protected const byte  CMD_ACK         = 0xF3; //控制应答
        protected const byte  CMD_GET_INFO    = 0xF4; //读取量程信息
        protected const byte  CMD_RET_INFO    = 0xF5; //量程信息返回
        protected const byte  CMD_RET_DATA    = 0xF6; //测量数据返回
        protected const byte  CMD_SET_DP_SEL  = 0xF7; //小数点位置设置
        protected const byte  CMD_SET_SPS     = 0xF8; //采样速率设置
        protected const byte  CMD_SET_BAUD    = 0xF9; //更改通讯波特率（重新上电有效）
        protected const byte  CMD_GET_DATA    = 0xFE; //读取测量值（单次读取）
        protected const byte  CMD_SET_RANGE   = 0xA1; //更改换算量程

        protected const byte  DP_HARDWARD = 0x00;
        protected const byte  DP_D1       = 0x01;
        protected const byte  DP_D2       = 0x02;
        protected const byte  DP_D3       = 0x03;
        protected const byte  DP_NO_3H    = 0x04;
        protected const byte  DP_D4_4H    = 0x04;
        protected const byte  DP_NO_4H    = 0x05;
        
        protected const byte  SPS_03PS_4H = 0x01;
        protected const byte  SPS_06PS_4H = 0x02;
        protected const byte  SPS_12PS_4H = 0x03;
        
        protected const byte  SPS_04PS_3H = 0x01;
        protected const byte  SPS_10PS_3H = 0x02;
        protected const byte  SPS_20PS_3H = 0x03;
        
        protected const byte  BAUD_RATE_115200   = 0x01;
        protected const byte  BAUD_RATE_57600    = 0x02;
        protected const byte  BAUD_RATE_38400    = 0x03;
        protected const byte  BAUD_RATE_19200    = 0x04;
        protected const byte  BAUD_RATE_9600     = 0x05;
        
        protected const byte  RESISTOR_RATE_AUTO = 0x00;
        protected const byte  RESISTOR_RATE_2K   = 0x01;
        protected const byte  RESISTOR_RATE_20K  = 0x02;
        protected const byte  RESISTOR_RATE_200K = 0x03;
        protected const byte  RESISTOR_RATE_2M   = 0x04;
        

        protected const byte  RANGE_1KHZ = 0x7D;
        protected const byte  RANGE_10KHZ = 0x7E;
        protected const byte  RANGE_100KHZ = 0x7F;
        protected const byte  RANGE_2MOhm = 0xA8;
        protected const byte  RANGE_200KOhm = 0xA9;
        protected const byte  RANGE_20KOhm = 0xAA;
        protected const byte  RANGE_2KOhm = 0xAB;
        protected const byte  RANGE_200Ohm = 0xAC;
        protected const byte  RANGE_1000A = 0xAD;
        protected const byte  RANGE_900A = 0xAE;
        protected const byte  RANGE_800A = 0xAF;
        protected const byte  RANGE_750A = 0xB0;
        protected const byte  RANGE_600A = 0xB1;
        protected const byte  RANGE_500A = 0xB2;
        protected const byte  RANGE_400A = 0xB3;
        protected const byte  RANGE_300A = 0xB4;
        protected const byte  RANGE_100A = 0xB5;
        protected const byte  RANGE_10A = 0xB6;
        protected const byte  RANGE_30A = 0xB7;
        protected const byte  RANGE_40A = 0xB8;
        protected const byte  RANGE_50A = 0xB9;
        protected const byte  RANGE_60A = 0xBA;
        protected const byte  RANGE_75A = 0xBB;
        protected const byte  RANGE_80A = 0xBC;
        protected const byte  RANGE_90A = 0xBD;
        protected const byte  RANGE_20A = 0xBE;
        protected const byte  RANGE_200A = 0xBF;
        protected const byte  RANGE_2V = 0xC1;
        protected const byte  RANGE_20V = 0xC2;
        protected const byte  RANGE_20mV = 0xC3;
        protected const byte  RANGE_200V = 0xC4;
        protected const byte  RANGE_200mV = 0xC5;
        protected const byte  RANGE_4V = 0xC6;
        protected const byte  RANGE_40V = 0xC7;
        protected const byte  RANGE_40mV = 0xC8;
        protected const byte  RANGE_400V = 0xC9;
        protected const byte  RANGE_400mV = 0xCA;
        protected const byte  RANGE_5V = 0xCB;
        protected const byte  RANGE_50V = 0xCC;
        protected const byte  RANGE_50mV = 0xCD;
        protected const byte  RANGE_500V = 0xCE;
        protected const byte  RANGE_500mV = 0xCF;
        protected const byte  RANGE_6V = 0xD0;
        protected const byte  RANGE_60V = 0xD1;
        protected const byte  RANGE_60mV = 0xD2;
        protected const byte  RANGE_600V = 0xD3;
        protected const byte  RANGE_600mV = 0xD4;
        protected const byte  RANGE_2A = 0xD5;
        protected const byte  RANGE_2mA = 0xD6;
        protected const byte  RANGE_20mA = 0xD7;
        protected const byte  RANGE_200mA = 0xD8;
        protected const byte  RANGE_200uA = 0xD9;
        protected const byte  RANGE_4mA = 0xDA;
        protected const byte  RANGE_40mA = 0xDB;
        protected const byte  RANGE_400mA = 0xDC;
        protected const byte  RANGE_400uA = 0xDD;
        protected const byte  RANGE_5mA = 0xDE;
        protected const byte  RANGE_50mA = 0xDF;
        protected const byte  RANGE_500mA = 0xE0;
        protected const byte  RANGE_500uA = 0xE1;
        protected const byte  RANGE_6mA = 0xE2;
        protected const byte  RANGE_60mA = 0xE3;
        protected const byte  RANGE_600mA = 0xE4;
        protected const byte  RANGE_600uA = 0xE5;
        protected const byte  RANGE_4A = 0xE6;
        protected const byte  RANGE_5A = 0xE7;
        protected const byte  RANGE_6A = 0xE8;
        protected const byte  RANGE_2KV = 0xE9;
        protected const byte  RANGE_NKV = 0xEA;
        protected const byte  RANGE_2mV = 0xEB;
        protected const byte  RANGE_20uA = 0xEB;
        protected const byte  RANGE_2KA = 0xED;
        protected const byte  RANGE_NKA = 0xEE;
        protected const byte  RANGE_700V = 0xEF;
        
        protected const byte  CLASS_DC_FOUR_HALF   = 0x11;
        protected const byte  CLASS_DC_THREE_HALF  = 0x12;
        protected const byte  CLASS_AC_FOUR_HALF   = 0x21;
        protected const byte  CLASS_AC_THREE_HALF  = 0x22;
        
        protected const byte  HEAD_FIRST  = 0xAA;
        protected const byte  HEAD_SECOND = 0x55;
        #endregion

        protected byte[] FormatCommand(byte CommandChar, byte Parameter = 0)
        {
            var Data = new List<byte>();
            ushort CHECK_SUM = 0;
            if (CommandChar == CMD_CONNECT
                || CommandChar == CMD_UNCONNECT
              || CommandChar == CMD_GET_INFO
              || CommandChar == CMD_SET_DP_SEL
              || CommandChar == CMD_SET_SPS
              || CommandChar == CMD_SET_BAUD
              || CommandChar == CMD_GET_DATA
              || CommandChar == CMD_SET_RANGE)
            {
                Data.Add( HEAD_FIRST);
                Data.Add( HEAD_SECOND);

                switch (CommandChar)
                {
                    case CMD_CONNECT:
                        {
                            Data.Add( 0x02);
                            Data.Add( CMD_CONNECT);
                            CHECK_SUM = 0x02 + CMD_CONNECT;
                        }
                        break;
                    case CMD_UNCONNECT:
                        {
                            Data.Add( 0x02);
                            Data.Add( CMD_UNCONNECT);
                            CHECK_SUM = 0x02 + CMD_UNCONNECT;
                        }
                        break;
                    case CMD_GET_INFO:
                        {
                            Data.Add( 0x02);
                            Data.Add( CMD_GET_INFO);
                            CHECK_SUM = 0x02 + CMD_GET_INFO;
                        }
                        break;
                    case CMD_SET_DP_SEL:
                        {
                            Data.Add( 0x03);
                            Data.Add( CMD_SET_DP_SEL);
                            Data.Add( Parameter); //dp
                            CHECK_SUM = (ushort)(0x03 + CMD_SET_DP_SEL + Parameter);
                        }
                        break;
                    case CMD_SET_SPS:
                        {
                            Data.Add( 0x03);
                            Data.Add( CMD_SET_SPS);
                            Data.Add( Parameter); //sps
                            CHECK_SUM = (ushort)(0x03 + CMD_SET_SPS + Parameter);
                        }
                        break;
                    case CMD_SET_BAUD:
                        {
                            Data.Add( 0x03);
                            Data.Add( CMD_SET_BAUD);
                            Data.Add( Parameter); //baud
                            CHECK_SUM = (ushort)(0x03 + CMD_SET_BAUD + Parameter);
                        }
                        break;
                    case CMD_GET_DATA:
                        {
                            Data.Add( 0x02);
                            Data.Add( CMD_GET_DATA);
                            CHECK_SUM = (ushort)( 0x02 + CMD_GET_DATA);
                        }
                        break;
                    case CMD_SET_RANGE:
                        {
                            Data.Add( 0x03);
                            Data.Add( CMD_SET_RANGE);
                            Data.Add( Parameter); //rr
                            CHECK_SUM = (ushort)(0x03 + CMD_SET_RANGE + Parameter);
                        }
                        break;
                }

                //checksum
                Data.Add((byte)((CHECK_SUM & 0xFF00) >> 8));
                Data.Add((byte)(CHECK_SUM & 0xFF));
            }
            return Data.ToArray();
        }

        ushort? AMMeterParseGetDataReturnData(byte[] Buffer)
        {
            //AA 55 04 F6 v0 v1 XX XX
            if (Buffer != null && Buffer.Length >= 8)
            {
                if (Buffer[0] == HEAD_FIRST
                && Buffer[1] == HEAD_SECOND
                && Buffer[2] == 0x04
                && Buffer[3] == CMD_RET_DATA
                && (((ushort)Buffer[6] << 8) | Buffer[7]) 
                == (ushort)Buffer[2]+ (ushort)Buffer[3]+ (ushort)Buffer[4]+ (ushort)Buffer[5] )
		        {
                    return (ushort)(Buffer[4] | (((ushort)Buffer[5]) << 8));
                }
            }
            return null;
        }

        protected bool AMMeterIsAckReturnData(byte[] Buffer)
        {
            if (Buffer != null && Buffer.Length >= 6)
            {
                return Buffer[0] == HEAD_FIRST
                    && Buffer[1] == HEAD_SECOND
                    && Buffer[2] == 0x02
                    && Buffer[3] == CMD_ACK
                    && Buffer[4] == 0x00
                    && Buffer[5] == Buffer[2] + Buffer[3];
            }
            return false;
        }

        public override int ReceiveBufferLength => 8;
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;

        public override string RemoteAddressText => "192.168.1.72";

        public AmpMeasurementNetControl()
        {
            this.Line.Description = "Current in uA";
        }
        protected override void CallInitializeComponent()
        {
            this.InitializeComponent();
        }
        public override bool OnConnectClient(Socket Client)
        {
            base.OnConnectClient(Client);

            this.PostReceiveBuffer(6);
            this.Send(this.FormatCommand(CMD_SET_SPS, SPS_12PS_4H));

            this.PostReceiveBuffer(6);
            this.Send(this.FormatCommand(CMD_CONNECT));


            return true;
        }
        protected override void OnReceivedInternal(byte[] data, int offset, int count)
        {
            if (data !=null && count >=6)
            {
                if (data[offset] == HEAD_FIRST && data[offset + 1] == HEAD_SECOND)
                {
                    ushort? value = null;

                    if (AMMeterIsAckReturnData(data))
                    {
                        //GOT ACK           
                    }
                    else if ((value = AMMeterParseGetDataReturnData(data))!=null)
                    {
                        this.AddData(value.Value);
                    }
                }
            }
        }
    }
}
