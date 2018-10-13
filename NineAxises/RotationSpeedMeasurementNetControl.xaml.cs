﻿using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Probes
{
    /// <summary>
    /// RotationSpeedMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class RotationSpeedMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceiveBufferLength => 21;
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;

        public override string RemoteAddressText => "192.168.1.68";

        protected const int DefaultSysFrequency = 168000000;
        protected const double ScaleFactor = 1.0;
        public RotationSpeedMeasurementNetControl()
        {
            this.LineGroup[0].Description = "Rotation Speed in RPM";
            this.LineGroup[0].Stroke = Brushes.Green;

        }
        protected override void CallInitializeComponent()
        {
            this.InitializeComponent();
        }
        protected override void OnReceivedInternal(string input)
        {
            if (input != null && input.StartsWith("RS:") && input.EndsWith("\n"))
            {
                var parts = input.Substring(3, 17).Split(',');

                if(parts.Length == 2)
                {
                    if (!int.TryParse(parts[0], System.Globalization.NumberStyles.HexNumber, null, out var period))
                    {
                        period = 1;
                    }
                    if (!int.TryParse(parts[1], System.Globalization.NumberStyles.HexNumber, null, out var sysfrequency))
                    {
                        sysfrequency = DefaultSysFrequency;
                    }
                    double f = period>0 ? sysfrequency / (period * ScaleFactor): 0.0;
                    
                    this.AddData(f*60.0);
                }
            }
        }
    }
}
