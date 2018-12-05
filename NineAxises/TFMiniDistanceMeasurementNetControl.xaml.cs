using System.Net.Sockets;
using System.Windows.Controls;
using System.Windows.Media;

namespace Probes
{
    /// <summary>
    /// LongDistanceMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class TFMiniDistanceMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceivePartLength => 9;
        protected override int LinesGroupLength => 2;
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox; protected const int DefaultMeasurementInterval = 200;
        protected const int MaxDistance = 80000; //mm
        protected const int InvalidDistance = 0xffff;//-1 for short value
        protected Socket Client = null;

        public TFMiniDistanceMeasurementNetControl()
        {
            this.LinesGroup[0].Description = "Distance in cm";
            this.LinesGroup[1].Description = "Strength(20-3000)";
            this.LinesGroup[0].Stroke = Brushes.Blue;
            this.LinesGroup[1].Stroke = Brushes.Red;
        }


        protected override void CallInitializeComponent()
        {
            this.InitializeComponent();
        }

        public override bool OnConnectClient(Socket Client)
        {
            return true;
        }

        protected override void OnReceivedInternal(byte[] data, int offset, int count)
        {
            if (data != null && data.Length==this.ReceivePartLength
                && data[offset+0] == 0x59 && data[offset + 1] == 0x59 
                && data[offset+7] == 0x00)
            {
                int sum = 0;
                for(int i = offset; i < offset + count-1; i++)
                {
                    sum += data[i];
                }
                if ((byte)(sum & 0xff) == data[offset + 8])
                {
                    ushort dist = (ushort)(data[offset + 2] | data[offset + 3] << 8);
                    if (dist != 0xffff)
                    {
                        ushort strength = (ushort)(data[offset + 4] | data[offset + 5] << 8);
                        byte mode = data[offset + 6];
                        switch (mode)
                        {
                            case 2:
                                this.ModeText.Text = "[Near]";
                                break;
                            case 7:
                                this.ModeText.Text = "[Far]";
                                break;
                        }
                        this.AddData(dist, 0);
                        this.AddData(strength, 1);
                    }
                }
            }
        }
    }
}
