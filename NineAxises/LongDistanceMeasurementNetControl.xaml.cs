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
