using InteractiveDataDisplay.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Probes
{
    public abstract class NineAxesMeasurementNetControl : MeasurementBaseNetControl
    {
        protected abstract AxisDisplayControl Display { get; }
        protected abstract Grid LinesAuxGrid { get; }

        protected override int LinesGroupLength => 4;
        protected virtual int LinesAuxGroupLength => 2;
        protected LineGraph[] LinesAuxGroup = null;
        protected double[] BaseZeroAuxYGroup = null;
        protected double[] LastYAuxGroup = null;
        protected List<Point>[] PointsAuxGroup = null;
        public override int ReceivePartLength { get; set; } = 11;
        public override int ReceiveBufferLength => ReceivePartLength <<2; //4 packet per frame
        protected List<byte> ByteBuffer = new List<byte>();
        public NineAxesMeasurementNetControl()
        {
            if (this.LinesAuxGrid != null && this.LinesAuxGroupLength>0)
            {
                this.LinesAuxGroup = new LineGraph[this.LinesAuxGroupLength];
                this.PointsAuxGroup = new List<Point>[this.LinesAuxGroupLength];

                for (int i = 0; i < this.LinesAuxGroup.Length; i++)
                {
                    var lg = this.CreateLineGraphInstance();
                    this.LinesAuxGrid.Children.Add(this.LinesAuxGroup[i] = lg);
                    this.LinePointsDict.Add(lg, this.PointsAuxGroup[i] = new List<Point>());
                }
                this.BaseZeroAuxYGroup = new double[this.LinesAuxGroup.Length];
                this.LastYAuxGroup = new double[this.LinesAuxGroup.Length];

            }
            Grid.SetColumnSpan(this, 2);
            if (this.LinesGroup != null && this.LinesGroup.Length == 4)
            {
                this.LinesGroup[0].Stroke = Brushes.Red;
                this.LinesGroup[1].Stroke = Brushes.Green;
                this.LinesGroup[2].Stroke = Brushes.Blue;
                this.LinesGroup[3].Stroke = Brushes.Black;
            }
            if (this.LinesAuxGroup != null && this.LinesAuxGroup.Length==2)
            {
                this.LinesAuxGroup[0].Stroke = Brushes.Violet;
                this.LinesAuxGroup[1].Stroke = Brushes.Cyan;
            }
            this.Display.InputMode = AxisDisplayControl.Modes.Vector;
        }

        protected override void OnReceivedInternal(byte[] data, int offset, int count)
        {
            if (data != null && count == this.ReceiveBufferLength && offset>=0 && offset<count )
            {
                for (int i = 0; i < count; i++)
                {
                    ByteBuffer.Add(data[offset + i]);
                }

                for (int i = 0; i < ByteBuffer.Count - this.ReceiveBufferLength + 1; i++)
                {
                    if ((ByteBuffer[i] == 0x55)
                    && ((ByteBuffer[i + 1] & 0x50) == 0x50)
                    && ((ByteBuffer[i + 0] + ByteBuffer[i + 1] + ByteBuffer[i + 2] + ByteBuffer[i + 3] + ByteBuffer[i + 4] + ByteBuffer[i + 5] + ByteBuffer[i + 6] + ByteBuffer[i + 7] + ByteBuffer[i + 8] + ByteBuffer[i + 9]) & 0xff) == ByteBuffer[i + 10])
                    {
                        this.OnReceivedInternalPart(ByteBuffer.Skip(i).Take(ReceivePartLength).ToArray(), 0, ReceivePartLength);
                        this.ByteBuffer = this.ByteBuffer.Skip(i + ReceivePartLength).ToList();
                        i = 0;
                    }
                }
             }
        }
        protected virtual void OnReceivedInternalPart(byte[] data, int offset, int count)
        {
            if (data != null)
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
        protected virtual void AddData(Vector3D data, bool std)
        {
            this.AddData(data.X, LineIndex: 0, Update: false);
            this.AddData(data.Y, LineIndex: 1, Update: false);
            this.AddData(data.Z, LineIndex: 2, Update: false);
            if (std)
            {
                this.AddDataSTD(data);
            }
            this.UpdateLines();
            this.Display.AddData(data);
        }
        protected virtual void AddDataSTD(Vector3D data)
        {
            double S = 0.0, T = 0.0, D = 0.0;

            if ((S = data.Length) > 0.0)
            {
                S *= Math.Sign(data.Z);
                T = Math.Acos(data.Z / S);
                D = Math.Atan2(data.Y, data.X);
            }
            this.AddData(S, 3,false);
            this.AddData(T, 4,false);
            this.AddData(D, 5,false);
        }

        protected override void BaseZeroYButton_Checked(object sender, RoutedEventArgs e)
        {
            if (this.BaseZeroAuxYGroup != null)
            {
                Array.Copy(this.LastYAuxGroup, this.BaseZeroAuxYGroup, this.LastYAuxGroup.Length);
            }
            base.BaseZeroYButton_Checked(sender, e);

        }
        protected override void BaseZeroYButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.BaseZeroAuxYGroup != null)
            {
                Array.Clear(this.BaseZeroAuxYGroup, 0, this.BaseZeroAuxYGroup.Length);
            }
            base.BaseZeroYButton_Unchecked(sender, e);
        }
        protected override void UpdateLines()
        {
            for (int i = 0; i < this.LinesGroupLength + 
                (this.LinesAuxGroup!=null?this.LinesAuxGroup.Length:0); i++)
            {
                this.UpdateLine(i);
            }
        }
        protected override void AddData(Point p, int LineIndex = 0, bool Update = true)
        {
            if (!this.IsPausing)
            {
                if (LineIndex >= 0 && LineIndex < this.LinesGroup.Length)
                {
                    base.AddData(p, LineIndex,Update);
                }
                else
                {
                    this.LastYAuxGroup[LineIndex - this.LinesGroup.Length] = p.Y;
                    this.PointsAuxGroup[LineIndex - this.LinesGroup.Length].Add(p);
                    if (Update)
                    {
                        this.UpdateLine(LineIndex);
                    }
                }
            }
        }
        protected override void UpdateLine(int LineIndex)
        {
            if (LineIndex >= 0 && LineIndex < this.LinesGroup.Length)
            {
                base.UpdateLine(LineIndex);
            }
            else
            {
                if (LineIndex < this.LinesGroup.Length + (this.LinesAuxGroup != null ? this.LinesAuxGroup.Length : 0))
                {
                    LineIndex -= this.LinesGroup.Length;

                    this.LinesAuxGroup[LineIndex].Points = new PointCollection(this.PointsAuxGroup[LineIndex].Select(
                        pt => new Point(pt.X, pt.Y - this.BaseZeroAuxYGroup[LineIndex])));

                    if (this.PointsAuxGroup[LineIndex].Count > 0)
                    {
                        //plot width in seconds
                        double CurrentPlotWidth = this.PointsAuxGroup[LineIndex][this.PointsAuxGroup[LineIndex].Count - 1].X - this.PointsAuxGroup[LineIndex][0].X;

                        if (CurrentPlotWidth > this.PlotWidth)
                        {
                            this.LinesAuxGroup[LineIndex].PlotOriginX = CurrentPlotWidth - this.PlotWidth;
                        }
                    }
                }
            }
        }
    }
}
