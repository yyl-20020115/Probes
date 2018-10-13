using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace Probes
{
    public enum AxisType
    {
        Gravity,
        Magnetic,
        AngleSpeed,
        AngleValue,
    }
    public interface INineAxesMeasurementNetControl : IMeasurementNetControl
    {
        AxisType AxisType { get; }
        void OnReceiveData(Vector3D data);
    }
    public class NineAxesDataDecoder : IMeasurementNetControl
    {
        public IPAddress RemoteAddress  => IPAddress.TryParse(this.RemoteAddressText, out var add) ? add : IPAddress.None;
        public string RemoteAddressText => "192.168.1.66";

        public int ReceiveBufferLength => 11;

        public delegate void OnReceiveDataDelegate(Vector3D data);

        public event OnReceiveDataDelegate GravityDataReceivedEvent;
        public event OnReceiveDataDelegate MagnetDataReceivedEvent;
        public event OnReceiveDataDelegate AngleSpeedDataReceivedEvent;
        public event OnReceiveDataDelegate AngleValueDataReceivedEvent;

        protected MeasurementBaseNetControl.OnReceiveDataDelegate OnReceivedCallback = null;
        public NineAxesDataDecoder()
        {
            this.OnReceivedCallback = new MeasurementBaseNetControl.OnReceiveDataDelegate(OnReceivedInternal);
        }
        public virtual void OnReceived(byte[] data, int offset, int count)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(this.OnReceivedCallback, data, offset, count);
        }
        protected virtual void OnReceivedInternal(byte[] data, int offset, int count)
        {
            if (data != null && count >= ReceiveBufferLength)
            {
                double[] result = new double[4];

                result[0] = BitConverter.ToInt16(data, offset + 2);
                result[1] = BitConverter.ToInt16(data, offset + 4);
                result[2] = BitConverter.ToInt16(data, offset + 6);
                result[3] = BitConverter.ToInt16(data, offset + 8);

                switch (data[1])
                {
                    case 0x50:
                        //ChipTime
                        break;
                    case 0x51:
                        //Gravity
                        this.GravityDataReceivedEvent(
                            new Vector3D(
                                result[0] / 32768.0 * 16.0,
                                result[1] / 32768.0 * 16.0,
                                result[2] / 32768.0 * 16.0
                                )
                            );
                        break;
                    case 0x52:
                        //AngleSpeed
                        this.AngleSpeedDataReceivedEvent(
                            new Vector3D(
                                result[0] / 32768.0 * 2000.0,
                                result[1] / 32768.0 * 2000.0,
                                result[2] / 32768.0 * 2000.0
                                )
                            );
                        break;
                    case 0x53:
                        //AngleValue
                        this.AngleValueDataReceivedEvent(
                            new Vector3D(
                                result[0] / 32768.0 * 180.0,
                                result[1] / 32768.0 * 180.0,
                                result[2] / 32768.0 * 180.0
                                )
                            );
                        break;
                    case 0x54:
                        //Magnet
                        this.MagnetDataReceivedEvent(
                            new Vector3D(
                                result[0] / 32768.0 * 1200.0 * 2.0,
                                result[1] / 32768.0 * 1200.0 * 2.0,
                                result[2] / 32768.0 * 1200.0 * 2.0
                                )
                            );
                        break;
                    case 0x55:
                        //PortVoltage
                        //PortVoltage[0] = Data[0];
                        //PortVoltage[1] = Data[1];
                        //PortVoltage[2] = Data[2];
                        //PortVoltage[3] = Data[3];
                        break;
                    case 0x56:
                        //Pressure = BitConverter.ToInt32(byteTemp, 2);
                        //Altitude = (double)BitConverter.ToInt32(byteTemp, 6) / 100.0;
                        break;
                    case 0x57:
                        //Longitude = BitConverter.ToInt32(byteTemp, 2);
                        //Latitude = BitConverter.ToInt32(byteTemp, 6);
                        break;
                    case 0x58:
                        //GPSHeight = (double)BitConverter.ToInt16(byteTemp, 2) / 10.0;
                        //GPSYaw = (double)BitConverter.ToInt16(byteTemp, 4) / 10.0;
                        //GroundVelocity = BitConverter.ToInt16(byteTemp, 6) / 1e3;
                        break;
                    default:
                        break;
                }
            }
        }

        public void OnConnectWindow(IMeasurementNetWindow window)
        {
            
        }

        public bool OnConnectClient(Socket Client)
        {
            return true;
        }

        public void OnSendComplete(byte[] data, int offset, int count)
        {
            
        }
    }
}
