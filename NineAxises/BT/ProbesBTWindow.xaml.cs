using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Windows;

namespace Probes
{
    /// <summary>
    /// ProbesWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProbesBTWindow : Window,IMeasurementBTHub
    {
        protected List<string> ComPortNames = new List<string>();
        protected List<IMeasurementBTControl> MeasurementControls = new List<IMeasurementBTControl>();
        protected List<IMeasurementBTControl> ConnectedControls = new List<IMeasurementBTControl>();
        public ProbesBTWindow()
        {
            InitializeComponent();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.MeasurementControls.ForEach(m => m.Dispose());
            this.MeasurementControls.Clear();
            this.ConnectedControls.Clear();
            this.ComPortNames.Clear();
        }
        public void CloseControl(IMeasurementBTControl Control)
        {
            if (Control != null)
            {
                this.ConnectedControls.Remove(Control);
                this.MainGrid.Children.Remove(Control as UIElement);
            }
        }

        public void ConnectComPort(IMeasurementBTControl Control, string PortName)
        {
            if (PortName != null)
            {
                this.ComPortNames.Remove(PortName);
            }
            if (Control != null)
            {
                this.ConnectedControls.Add(Control);
                foreach(var m in this.MeasurementControls.Where(c=>c!=Control))
                {
                    m.SetComPortsList(this.ComPortNames);
                }
            }
        }

        public void DisconnectComPort(IMeasurementBTControl Control, string PortName)
        {
            if (PortName != null)
            {
                this.ComPortNames.Add(PortName);
            }
            if (Control != null)
            {
                this.ConnectedControls.Add(Control);
                foreach (var m in this.MeasurementControls.Where(c => c != Control))
                {
                    m.SetComPortsList(this.ComPortNames);
                }
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.ComPortNames.AddRange(SerialPort.GetPortNames());

            this.ComPortNames.Sort(new ComNameComparer());

            foreach(var c in this.MainGrid.Children)
            {
                if(c is IMeasurementBTControl m)
                {
                    this.MeasurementControls.Add(m);
                    m.ConnectHub(this);
                    m.SetComPortsList(this.ComPortNames);
                }
            }
        }
    }
}
