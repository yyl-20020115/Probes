using System;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Probes
{
    /// <summary>
    /// LCRVC4090MeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class LCRVC4090MeasurementNetControl : MeasurementBaseNetControl
    {
        protected const string VC4090LCR_COMMAND_IDN = "*IDN? ";
        protected const string VC4090LCR_COMMAND_BAD_IDN = "*IDN?\r\n";

        protected const string VC4090LCR_COMMAND_SET_REMOTE_MODE = "SYST:REM ";
        protected const string VC4090LCR_COMMAND_SET_LOCAL_MODE = "SYST:LOC ";
        protected const string VC4090LCR_COMMAND_FETCH_DATA = "FETCh? ";

        protected const string VC4090LCR_COMMAND_SET_L_MODE = "FUNC:IMP:A L ";
        protected const string VC4090LCR_COMMAND_SET_C_MODE = "FUNC:IMP:A C ";
        protected const string VC4090LCR_COMMAND_SET_R_MODE = "FUNC:IMP:A R ";
        protected const string VC4090LCR_COMMAND_SET_AUTO_MODE = "FUNC:IMP:A AUTO ";
        protected const string VC4090LCR_COMMAND_SET_Z_MODE = "FUNC:IMP:A Z ";
        protected const string VC4090LCR_COMMAND_SET_DCR_MODE = "FUNC:IMP:A DCR ";
        protected const string VC4090LCR_COMMAND_SET_ECAP_MODE = "FUNC:IMP:A ECAP ";

        protected const string VC4090LCR_COMMAND_SET_AUX_X_MODE = "FUNC:IMP:B X ";
        protected const string VC4090LCR_COMMAND_SET_AUX_D_MODE = "FUNC:IMP:B D ";
        protected const string VC4090LCR_COMMAND_SET_AUX_Q_MODE = "FUNC:IMP:B Q ";
        protected const string VC4090LCR_COMMAND_SET_AUX_THR_MODE = "FUNC:IMP:B THR ";
        protected const string VC4090LCR_COMMAND_SET_AUX_ESR_MODE = "FUNC:IMP:B ESR ";
        protected override int LinesGroupLength => 2;
        public override int ReceivePartLength => 20;
        public override string[] Headers => null;
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;
        protected DispatcherTimer CommandTimer = new DispatcherTimer();
        protected TimeSpan DefaultCommandInterval = TimeSpan.FromMilliseconds(100);
        public LCRVC4090MeasurementNetControl()
        {
            this.LinesGroup[0].Description = "Main";
            this.LinesGroup[0].Stroke = Brushes.Blue;
            this.LinesGroup[1].Description = "Aux";
            this.LinesGroup[0].Stroke = Brushes.Green;
            this.CommandTimer.Interval = DefaultCommandInterval;
            this.CommandTimer.Tick += Timer_Tick;
        }
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }
        public override void Dispose()
        {
            this.Send(VC4090LCR_COMMAND_SET_LOCAL_MODE);

            base.Dispose();
        }
        public override bool OnConnectClient(Socket Client)
        {
            //BAD command is used to fix hardware error on my LCR meter
            this.Send(VC4090LCR_COMMAND_BAD_IDN);

            this.Send(VC4090LCR_COMMAND_IDN);
            this.Send(VC4090LCR_COMMAND_SET_REMOTE_MODE);
            this.Send(VC4090LCR_COMMAND_SET_AUTO_MODE);

            return base.OnConnectClient(Client);
        }
        protected virtual void Timer_Tick(object sender, System.EventArgs e)
        {
            this.Send(VC4090LCR_COMMAND_FETCH_DATA);
        }
        protected override void OnReceivedInternal(string input)
        {
            if (input != null)
            {
                var parts = input.TrimEnd().Split(',');
                if(parts!=null && parts.Length == 2)
                {
                    if (double.TryParse(parts[0], System.Globalization.NumberStyles.Number, null, out var MainValue))
                    {
                        this.AddData(MainValue, 0);
                    }
                    if (double.TryParse(parts[1], System.Globalization.NumberStyles.Number, null, out var AuxValue))
                    {
                        this.AddData(AuxValue, 1);
                    }
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

        protected virtual void ACheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.Send(VC4090LCR_COMMAND_SET_AUTO_MODE);
        }

        protected virtual void LCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.Send(VC4090LCR_COMMAND_SET_L_MODE);
        }

        protected virtual void CCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.Send(VC4090LCR_COMMAND_SET_C_MODE);
        }

        protected virtual void RCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.Send(VC4090LCR_COMMAND_SET_R_MODE);
        }
    }
}
