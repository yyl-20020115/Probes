using System.Windows.Controls;

namespace Probes
{
    /// <summary>
    /// TorsionMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class TorsionMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceivePartLength => 6;
        public override string[] Headers => new string[] { };
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;

        public double ScaleFactor { get; set; } = 1.0;
        public TorsionMeasurementNetControl()
        {
            this.LinesGroup[0].Description = "Torsion in N.m";
        }
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }
        protected override void OnReceivedInternal(byte[] data, int offset, int count)
        {
            if(data!=null && data.Length == ReceivePartLength 
                && data[offset+0] == 0x01
                && ((data[offset+1] & 0xf0) == 0x50))
            {
                if(data[offset+5]==(data[offset+0] ^ data[offset + 1] ^ data[offset + 2] ^ data[offset + 3] ^ data[offset + 4]))
                {
                    int range = (data[offset + 1] & 0x0f);
                    int value = data[offset + 2] << 16 | data[offset + 3] << 8 | data[offset + 4];
                    if((value & 0x800000) != 0)
                    {
                        int t = 0xff;
                        t <<= 16;
                        value |= t;
                    }
                    this.AddData(value * ScaleFactor);
                }
            }
        }
    }
}
