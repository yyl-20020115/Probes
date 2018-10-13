using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Probes
{
    public abstract class NineAxesMeasurementNetControl : MeasurementBaseNetControl
    {
        public NineAxesMeasurementNetControl()
        {
            Grid.SetColumnSpan(this, 2);
        }
        protected abstract AxisDisplayControl Display { get; }
        protected override int LineGroupLength => 3;
        protected override void OnReceivedInternal(byte[] data, int offset, int count)
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
                        this.OnGravityDataReceived(
                            new Vector3D(
                                result[0] / 32768.0 * 16.0,
                                result[1] / 32768.0 * 16.0,
                                result[2] / 32768.0 * 16.0
                                )
                            );
                        break;
                    case 0x52:
                        //AngleSpeed
                        this.OnAngleSpeedDataReceived(
                            new Vector3D(
                                result[0] / 32768.0 * 2000.0,
                                result[1] / 32768.0 * 2000.0,
                                result[2] / 32768.0 * 2000.0
                                )
                            );
                        break;
                    case 0x53:
                        //AngleValue
                        this.OnAngleValueDataReceived(
                            new Vector3D(
                                result[0] / 32768.0 * 180.0,
                                result[1] / 32768.0 * 180.0,
                                result[2] / 32768.0 * 180.0
                                )
                            );
                        break;
                    case 0x54:
                        //Magnet
                        this.OnMagnetDataReceived(
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

        protected virtual void OnGravityDataReceived(Vector3D data)
        {

        }
        protected virtual void OnAngleSpeedDataReceived(Vector3D data)
        {

        }
        protected virtual void OnAngleValueDataReceived(Vector3D data)
        {

        }
        protected virtual void OnMagnetDataReceived(Vector3D data)
        {

        }
        protected virtual void AddData(Vector3D data)
        {

        }
        protected virtual void EnableDisplay_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Display.Visibility = System.Windows.Visibility.Visible;
        }

        protected virtual void EnableDisplay_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Display.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
