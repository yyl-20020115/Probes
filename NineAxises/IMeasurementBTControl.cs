using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Probes
{
    public interface IMeasurementBTHub
    {
        void ConnectComPort(IMeasurementBTControl Control,string PortName);
        void DisconnectComPort(IMeasurementBTControl Control, string PortName);
        void CloseControl(IMeasurementBTControl Control);
    }
    public interface IMeasurementBTControl
    {
        string CurrentComPortName { get; set; }
        bool IsPausing { get; }
        bool IsConnected { get; }
        void ConnectHub(IMeasurementBTHub hub);
        void SetComPortsList(List<string> ComPortsList);
        void Dispose();
    }
}
