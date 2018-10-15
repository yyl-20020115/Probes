using System.Windows.Controls;
using System.Windows.Media;

namespace Probes
{
    /// <summary>
    /// TemperatureMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class TemperatureMeasurementNetControl : MeasurementBaseNetControl
    {
        /*
            模块输出格式，每帧包含10个字节（十六进制）：

            ①.Byte0: 0x5A帧头标志

            ②.Byte1: 0x5A帧头标志

            ③.Byte2: 0X45本帧数据类型（0X45：温度数据）

            ④.Byte3: 0x04数据量（以下4个数据2组为例）

            ⑤.Byte4: 0x00~0xFF数据1高8位

            ⑥.Byte5: 0x00~0xFF数据1低8位

            ⑦.Byte6: 0x00~0xFF数据2高8位

            ⑧.Byte7: 0x00~0xFF数据2低8位

            ⑨.Byte8: 0x00~0xFF校验和（前面数据累加和，仅留低8位）
            10.Byte9: 0x14
         */

        public override int ReceivePartLength => 10;
        public override string Header => string.Empty;
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;
        protected override int LinesGroupLength => 2;

        public TemperatureMeasurementNetControl()
        {
            InitializeComponent();
            this.LinesGroup[0].Description = "Target Temperature in C-Degree";
            this.LinesGroup[1].Description = "Environment Temperature in C-Degree";
            this.LinesGroup[0].Stroke = Brushes.Blue;
            this.LinesGroup[1].Stroke = Brushes.Green;

        }
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }
        protected override void OnReceivedInternal(byte[] data, int offset, int count)
        {
            if(data!=null && count == this.ReceivePartLength)
            {
                if (data[offset + 0] == 0x5A && data[offset + 1] == 0x5A && data[offset + 2] == 0x45 && data[offset + 3] == 0x04 &&
                    data[offset + 9] == 0x14 
                    && (byte)(data[offset + 0] + data[offset + 1] + data[offset + 2] + data[offset + 3] + data[offset + 4] + data[offset + 5] + data[offset + 6] + data[offset + 7]) == data[offset + 8])
                {
                    ushort t0 = (ushort)((ushort)data[offset + 4] << 8 | (ushort)data[offset + 5]);
                    ushort t1 = (ushort)((ushort)data[offset + 6] << 8 | (ushort)data[offset + 7]);

                    this.Input(t0, t1);
                }
            }
        }
        public virtual void Input(ushort t0,ushort t1)
        {
            this.AddData(t0 / 100.0, 0, false);
            this.AddData(t1 / 100.0, 1, false);
            this.UpdateLines();
        }
    }
}
