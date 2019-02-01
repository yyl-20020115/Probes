using System;
using System.IO.Ports;
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
    public partial class LCRVC4090MeasurementNetControl : MeasurementBaseSerialControl
    {
        protected const string VC4090LCR_COMMAND_IDN = "*IDN?";

        protected const string VC4090LCR_COMMAND_SET_REMOTE_MODE = "SYST:REM";
        protected const string VC4090LCR_COMMAND_SET_LOCAL_MODE = "SYST:LOC";
        protected const string VC4090LCR_COMMAND_FETCH_DATA = "FETCh?";

        protected const string VC4090LCR_COMMAND_SET_L_MODE = "FUNC:IMP:A L";
        protected const string VC4090LCR_COMMAND_SET_C_MODE = "FUNC:IMP:A C";
        protected const string VC4090LCR_COMMAND_SET_R_MODE = "FUNC:IMP:A R";
        protected const string VC4090LCR_COMMAND_SET_AUTO_MODE = "FUNC:IMP:A AUTO";
        protected const string VC4090LCR_COMMAND_SET_Z_MODE = "FUNC:IMP:A Z";
        protected const string VC4090LCR_COMMAND_SET_DCR_MODE = "FUNC:IMP:A DCR";
        protected const string VC4090LCR_COMMAND_SET_ECAP_MODE = "FUNC:IMP:A ECAP";

        protected const string VC4090LCR_COMMAND_SET_AUX_X_MODE = "FUNC:IMP:B X";
        protected const string VC4090LCR_COMMAND_SET_AUX_D_MODE = "FUNC:IMP:B D";
        protected const string VC4090LCR_COMMAND_SET_AUX_Q_MODE = "FUNC:IMP:B Q";
        protected const string VC4090LCR_COMMAND_SET_AUX_THR_MODE = "FUNC:IMP:B THR";
        protected const string VC4090LCR_COMMAND_SET_AUX_ESR_MODE = "FUNC:IMP:B ESR";
        protected override int LinesGroupLength => 2;
        public override int ReceivePartLength => 20;
        public override string[] Headers => null;
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;
        protected DispatcherTimer CommandTimer = new DispatcherTimer();
        protected TimeSpan DefaultCommandInterval = TimeSpan.FromMilliseconds(10);
        protected double CapacityFactor = 1.0e12;
        protected double InductorFactor = 1.0e6;
        protected double ResisiterFactor = 1.0e-3;

        public enum MeterMode
        {
            Auto = 0,
            Cap = 1,
            Ind = 2,
            Res = 3,
        }
        protected MeterMode Mode = MeterMode.Cap;
        public LCRVC4090MeasurementNetControl()
            :base()
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
            this.SendCommandWithNewLine(VC4090LCR_COMMAND_SET_LOCAL_MODE);

            base.Dispose();
        }
        protected virtual string SendCommandWithSpace(string command, bool read = true)
             => this.SendCommandWithSuffix(command, ' ', read);
        protected virtual string SendCommandWithNewLine(string command, bool read = true)
         => this.SendCommandWithSuffix(command, '\n', read);
        protected virtual string SendCommandWithSuffix(string command, char suffix='\n', bool read=true)
        {
            string ret = string.Empty;
            if (command != null)
            {
                ret = this.Send(command + suffix, read);
            }
            return ret;
        }
        protected override void OnConnectPort(SerialPort port)
        {
            string ret = null;

            ret = this.SendCommandWithSpace(VC4090LCR_COMMAND_IDN);
            ret = this.SendCommandWithNewLine(VC4090LCR_COMMAND_SET_REMOTE_MODE);
            //ret = this.SendCommandWithNewLine(VC4090LCR_COMMAND_SET_AUTO_MODE);

            this.CommandTimer.Start();

        }

        protected virtual void Timer_Tick(object sender, System.EventArgs e)
        {
            this.SendCommandWithNewLine(VC4090LCR_COMMAND_FETCH_DATA,false);
        }
        protected override void OnReceivedInternal(string input)
        {
            if (input != null && !input.StartsWith("ZC,"))
            {
                var parts = input.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach(var part in parts)
                {
                    this.OnReceiveLine(part);
                }
            }
        }
        protected virtual void OnReceiveLine(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                var parts = input.Trim().Split(',');
                if(parts!=null &&parts.Length == 1)
                {
                    parts = new string[] { parts[0], "0" };
                }
                if (parts != null && parts.Length == 2)
                {
                    if (double.TryParse(parts[0], System.Globalization.NumberStyles.Float, null, out var MainValue))
                    {
                        double f = 1.0;
                        switch (this.Mode)
                        {
                            case MeterMode.Cap:
                                f = this.CapacityFactor;
                                break;
                            case MeterMode.Ind:
                                f = this.InductorFactor;
                                break;
                            case MeterMode.Res:
                                f = this.ResisiterFactor;
                                break;
                            case MeterMode.Auto:
                                break;
                            default:
                                break;

                        }
                        this.AddData(MainValue * f, 0);
                    }
                    if (double.TryParse(parts[1], System.Globalization.NumberStyles.Float, null, out var AuxValue))
                    {
                        //this.AddData(AuxValue, 1);
                    }
                }
            }
        }
        protected override void SetRemoteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            base.SetRemoteCheckBox_Checked(sender, e);
        }
        protected override void SetRemoteCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            base.SetRemoteCheckBox_Unchecked(sender, e);
            this.CommandTimer.Stop();
        }

        protected virtual void ACheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.Mode = MeterMode.Auto;
            if (this.LinesGroup != null)
            {
                this.LinesGroup[0].Description = "Main";
                this.LinesGroup[0].Stroke = Brushes.Blue;
                this.LinesGroup[1].Description = "Aux";
                this.LinesGroup[0].Stroke = Brushes.Green;
            }
            this.Reset();
        }

        protected virtual void LCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.Mode = MeterMode.Ind;
            if (this.LinesGroup != null)
            {
                this.LinesGroup[0].Description = "Ind in uH";
            }
            this.Reset();
            this.SendCommandWithSuffix(VC4090LCR_COMMAND_SET_L_MODE);
        }

        protected virtual void CCheckBox_Checked(object sender, RoutedEventArgs e)
        {
             this.Mode = MeterMode.Cap;
            if (this.LinesGroup != null)
            {
                this.LinesGroup[0].Description = "Cap in pF";
            }
            this.Reset();
            this.SendCommandWithSuffix(VC4090LCR_COMMAND_SET_C_MODE);
        }

        protected virtual void RCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.Mode = MeterMode.Res;
            if (this.LinesGroup != null)
            {
                this.LinesGroup[0].Description = "Res in kOhm";
            }
            this.Reset();
            this.SendCommandWithSuffix(VC4090LCR_COMMAND_SET_R_MODE);
        }

        private void LCRControl_Initialized(object sender, EventArgs e)
        {
            this.UpdatePortNames();
            if (this.RemoteAddressComboBox.Items.Count > 0)
            {
                this.RemoteAddressComboBox.SelectedIndex = 0;
            }
        }
    }
}
