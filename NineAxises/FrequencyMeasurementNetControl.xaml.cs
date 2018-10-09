using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Probes
{
    /// <summary>
    /// FrequencyMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class FrequencyMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceiveBufferLength => 21;
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;

        public const int DefaultSysFrequency = 168000000;
        protected override string RemoteAddressText => "192.168.1.70";
        public FrequencyMeasurementNetControl()
        {
            this.Line.Description = "Frequency in Hz";
        }
        protected override void CallInitializeComponent()
        {
            this.InitializeComponent();
        }
        protected override void OnReceivedInternal(string input)
        {
            if(input!=null&& input.StartsWith("FM:") && input.EndsWith("\n"))
            {
                var parts = input.Substring(3, 9).Split(',');
                if (parts.Length == 2)
                {
                    if(!int.TryParse(parts[0],System.Globalization.NumberStyles.HexNumber, null, out var period))
                    {
                        period = -1;
                    }
                    if(!int.TryParse(parts[1],System.Globalization.NumberStyles.HexNumber,null,out var sysfrequency))
                    {
                        sysfrequency = DefaultSysFrequency;
                    }

                    this.SyncPlot(period > 0 ? sysfrequency / (double)period : 0);
                }
            }
        }
    }
}
