using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Probes
{
    public interface IMeasurementHub
    {
        void ConnectComPort(IMeasurementControl Control,string PortName);
        void DisconnectComPort(IMeasurementControl Control, string PortName);
        void CloseControl(IMeasurementControl Control);
    }
    public interface IMeasurementControl
    {
        string CurrentComPortName { get; set; }
        bool IsPausing { get; }
        bool IsConnected { get; }
        void ConnectHub(IMeasurementHub hub);
        void SetComPortsList(List<string> ComPortsList);
        void Dispose();
    }
}
