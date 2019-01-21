using InteractiveDataDisplay.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Probes
{
    public abstract class BNOMeasurementNetControl : MeasurementBaseNetControl
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
        public BNOMeasurementNetControl()
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
        //192.168.1.85
        //BNO:C212,0959,F2DA,00C8,3244,03F2,FF28,F6CE,0002,0000,0000,0000,0002,0002,0001,0000,FEC9,FE6D,018B,0000
        //int16_t rotationVector_Q1 = 14;
        //int16_t accelerometer_Q1 = 8;
        //int16_t linear_accelerometer_Q1 = 8;
        //int16_t gyro_Q1 = 9;
        //int16_t magnetometer_Q1 = 4;

        //uint16_t quatI = myIMU.getRawQuatI();
        //uint16_t quatJ = myIMU.getRawQuatJ();
        //uint16_t quatK = myIMU.getRawQuatK();
        //uint16_t quatR = myIMU.getRawQuatReal();
        //uint16_t quatA = myIMU.getRawQuatRadianAccuracy();
        //uint16_t ax = myIMU.getRawAccelX();
        //uint16_t ay = myIMU.getRawAccelY();
        //uint16_t az = myIMU.getRawAccelZ();
        //uint16_t aa = myIMU.getAccelAccuracy();

        //uint16_t lx = myIMU.getRawLinAccelX();
        //uint16_t ly = myIMU.getRawLinAccelY();
        //uint16_t lz = myIMU.getRawLinAccelZ();

        //uint16_t gx = myIMU.getRawGyroX();
        //uint16_t gy = myIMU.getRawGyroY();
        //uint16_t gz = myIMU.getRawGyroZ();
        //uint16_t ga = myIMU.getGyroAccuracy();

        //uint16_t mx = myIMU.getRawMagX();
        //uint16_t my = myIMU.getRawMagY();
        //uint16_t mz = myIMU.getRawMagZ();
        //uint16_t ma = myIMU.getMagAccuracy();
        protected override void OnReceivedInternal(string input)
        {
            if(input!=null && input.StartsWith("BNO:"))
            {
                var parts = input.Substring(4).Split(',');
                if(parts.Length == 15)
                {
                    //float quatI = myIMU.getQuatI();
                    //float quatJ = myIMU.getQuatJ();
                    //float quatK = myIMU.getQuatK();
                    //float quatR = myIMU.getQuatReal();
                    //float quatA = myIMU.getQuatRadianAccuracy();
                    //float ax = myIMU.getAccelX();
                    //float ay = myIMU.getAccelY();
                    //float az = myIMU.getAccelZ();
                    //float gx = myIMU.getGyroX();
                    //float gy = myIMU.getGyroY();
                    //float gz = myIMU.getGyroZ();
                    //float mx = myIMU.getMagX();
                    //float my = myIMU.getMagY();
                    //float mz = myIMU.getMagZ();
                    //int ma = myIMU.getMagAccuracy();
                    var all = true;
                    if(!float.TryParse(parts[0],out var quatI))
                    {
                        all = false;
                        quatI = 0.0f;
                    }
                    if (!float.TryParse(parts[1], out var quatJ))
                    {
                        all = false;
                        quatJ = 0.0f;
                    }
                    if (!float.TryParse(parts[2], out var quatK))
                    {
                        all = false;
                        quatK = 0.0f;
                    }
                    if (!float.TryParse(parts[3], out var quatR))
                    {
                        all = false;
                        quatR = 0.0f;
                    }
                    if (!float.TryParse(parts[4], out var quatA))
                    {
                        all = false;
                        quatA = 0.0f;
                    }
                    if (!float.TryParse(parts[5], out var ax))
                    {
                        all = false;
                        ax = 0.0f;
                    }
                    if (!float.TryParse(parts[6], out var ay))
                    {
                        all = false;
                        ay = 0.0f;
                    }
                    if (!float.TryParse(parts[7], out var az))
                    {
                        all = false;
                        az = 0.0f;
                    }
                    if (!float.TryParse(parts[8], out var gx))
                    {
                        all = false;
                        gx = 0.0f;
                    }
                    if (!float.TryParse(parts[9], out var gy))
                    {
                        all = false;
                        gy = 0.0f;
                    }
                    if (!float.TryParse(parts[10], out var gz))
                    {
                        all = false;
                        gz = 0.0f;
                    }
                    if (!float.TryParse(parts[11], out var mx))
                    {
                        all = false;
                        mx = 0.0f;
                    }
                    if (!float.TryParse(parts[12], out var my))
                    {
                        all = false;
                        my = 0.0f;
                    }
                    if (!float.TryParse(parts[13], out var mz))
                    {
                        all = false;
                        mz = 0.0f;
                    }
                    if (!int.TryParse(parts[0], out var ma))
                    {
                        all = false;
                        ma = 0;
                    }

                    if (all)
                    {

                    }
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
        public override void Reset()
        {
            base.Reset();
            for (int i = 0; i < this.LinesAuxGroup.Length; i++)
            {
                this.PointsAuxGroup[i].Clear();
                this.LinesAuxGroup[i].Points = new PointCollection();
                this.LinesAuxGroup[i].PlotOriginX = 0.0;
                this.LinesAuxGroup[i].PlotOriginY = 0.0;
            }
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
