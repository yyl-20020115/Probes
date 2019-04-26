using System;
using System.Net.Sockets;
using System.Windows.Controls;

namespace Probes
{
    /// <summary>
    /// MicroMetertMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class MicroMetertMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceivePartLength => 52;
        public override string[] Headers => new string[] { "%" };
        public override char EndOfLineChar => '\r';
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;
        protected const double Factor = 1.0 / 10000.0;

        public MicroMetertMeasurementNetControl()
        {
            this.LinesGroup[0].Description = "Length in MilliMeter";
        }
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }
        public override bool OnConnectClient(Socket Client)
        {
            this.Send("%01#RMD**\r");
            return true;
        }
        protected override void OnReceivedInternal(string input)
        {
            if (input != null && input.Length>=14)
            {
                //%01$+0000118**
                //01234567890ABC

                string vt = input.Substring(4, 8);
                if(double.TryParse(vt,out var data))
                {
                    this.AddData(data * Factor);
                }
            }

        }

    }
}
