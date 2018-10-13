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
        void Connect(INineAxesMeasurementNetDecoderControl decoder);
    }
    public interface INineAxesMeasurementNetDecoderControl : IMeasurementNetControl
    {
        void AddNineAxesControl(INineAxesMeasurementNetControl control);
        void RemoveNineAxesControl(INineAxesMeasurementNetControl control);
        void Connect(string RemoteAddressText);
        void Disconnect();
    }
    public class NineAxesDataDecoder : INineAxesMeasurementNetDecoderControl
    {
        public virtual IPAddress RemoteAddress  => IPAddress.TryParse(this.RemoteAddressText, out var add) ? add : IPAddress.None;
        public virtual string RemoteAddressText { get; set; }= "192.168.1.66"; 

        protected IMeasurementNetWindow window = null;
        public int ReceiveBufferLength => 11;

        public delegate void OnReceiveDataDelegate(Vector3D data);

        public event OnReceiveDataDelegate GravityDataReceivedEvent;
        public event OnReceiveDataDelegate MagnetDataReceivedEvent;
        public event OnReceiveDataDelegate AngleSpeedDataReceivedEvent;
        public event OnReceiveDataDelegate AngleValueDataReceivedEvent;

        protected MeasurementBaseNetControl.OnReceiveDataDelegate OnReceivedCallback = null;

        protected List<INineAxesMeasurementNetControl> Controls = new List<INineAxesMeasurementNetControl>();
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

        public virtual void OnConnectWindow(IMeasurementNetWindow window)
        {
            this.window = window;
        }

        public virtual bool OnConnectClient(Socket Client)
        {
            return true;
        }

        public virtual void OnSendComplete(byte[] data, int offset, int count)
        {
            
        }

        protected string PreviousRemoteAddressText = null;

        public virtual void Connect(string RemoteAddressText)
        {
            this.UpdateRemoteAddressTexts(this.PreviousRemoteAddressText = this.RemoteAddressText);
        }

        public virtual void Disconnect()
        {
            this.UpdateRemoteAddressTexts( this.RemoteAddressText = this.PreviousRemoteAddressText);
        }
        protected virtual void UpdateRemoteAddressTexts(string remote)
        {
            foreach (var c in this.Controls)
            {
                c.RemoteAddressText = remote;
            }
        }
        public virtual void AddNineAxesControl(INineAxesMeasurementNetControl control)
        {
            if (control != null)
            {
                this.Controls.Add(control);
            }
        }

        public virtual void RemoveNineAxesControl(INineAxesMeasurementNetControl control)
        {
            if (control != null)
            {
                this.Controls.Remove(control);
            }
        }
    }
}
