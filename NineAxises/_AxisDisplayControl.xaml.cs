using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Probes
{
    /// <summary>
    /// AxisDisplayControl.xaml 的交互逻辑
    /// </summary>
    public partial class AxisDisplayControl : UserControl
    {
        public enum Modes : int
        {
            None = 0,
            Rotate = 1,
            Vector = 2,
        }

        private string unitValue = string.Empty;
        private string unitAngle = string.Empty;

        private double S = 0.0;
        private double T = 0.0;
        private double D = 0.0;

        private double scaleFactor = 0.5;
        private double bodyRadius = 1.0;
        private double bodyThickness = 0.01;
        private double stickRaidus = 0.02;
        private double stickLength = 1.0;
        private int segments = 64;

        private Vector3D lastVector = default(Vector3D);
        private Vector3D zeroVector = default(Vector3D);
        private Vector3D lastSTD = default(Vector3D);

        private string _sText = string.Empty;
        private string _tText = string.Empty;
        private string _dText = string.Empty;
        private string _xText = string.Empty;
        private string _yText = string.Empty;
        private string _zText = string.Empty;
        private Modes _inputMode = Modes.None;
        private Vector3D _maxVector = new Vector3D(1.0, 1.0, 1.0);
        private Vector3D _maxSTD = new Vector3D(1.0, 1.0, 1.0);

        public Modes InputMode { get => _inputMode; set { _inputMode = value;this.Update(); } }
        public string Title
        {
            get { return this.TitleText.Text; }
            set { this.TitleText.Text = value ?? string.Empty; }
        }

        public bool AutoUpdate { get; set; } = false;

        public string XText { get => _xText; set { _xText = value; this.Update(); } }
        public string YText { get => _yText; set { _yText = value; this.Update(); } }
        public string ZText { get => _zText; set { _zText = value; this.Update(); } }


        public string SText { get => _sText; set { _sText = value; this.Update(); } }
        public string TText { get => _tText; set { _tText = value; this.Update(); } }
        public string DText { get => _dText; set { _dText = value; this.Update(); } }

        public string ValueUnit { get => unitValue; set { unitValue = value; this.Update(); } }
        public string AngleUnit { get => unitAngle; set { unitAngle = value; this.Update(); } }
        public double ScaleFactor { get => scaleFactor; set { scaleFactor = value; this.BuildParts(); this.Update(); } }

        public double BodyRadius { get => bodyRadius; set { bodyRadius = value; this.BuildParts(); this.Update(); } }
        public double BodyThickness { get => bodyThickness; set { bodyThickness = value; this.BuildParts(); this.Update(); } }
        public double StickRaidus { get => stickRaidus; set { stickRaidus = value; this.BuildParts(); this.Update(); } }
        public double StickLength { get => stickLength; set { stickLength = value; this.BuildParts(); this.Update(); } }
        public int Segments { get => segments; set { segments = value; this.BuildParts(); this.Update(); } }

        public Vector3D ZeroVector { get => zeroVector; set { zeroVector = value; this.Update(); } }
        public AxisDisplayControl()
        {
            InitializeComponent();
            this.XText = "X";
            this.YText = "Y";
            this.ZText = "Z";
            this.SText = "S";
            this.TText = "T";
            this.DText = "D";
        }

        public virtual void Look(Point3D SelfLocation)
        {
            this.Look(SelfLocation, new Point3D());
        }

        public virtual void Look(Point3D SelfLocation, Point3D TargetLocation)
        {
            this.Camera.Position = SelfLocation;
            this.Camera.LookDirection = TargetLocation - SelfLocation;
        }

        private void BuildParts()
        {
            this.Head.Geometry = this.BuildCylinder(this.StickRaidus, this.StickLength, this.Segments);
            this.Body.Geometry = this.BuildCylinder(this.BodyRadius, this.BodyThickness, this.Segments);
            this.Tail.Geometry = this.BuildCylinder(this.StickRaidus, -this.StickLength, this.Segments);
            this._Arm.Geometry = this.BuildCylinder(this.StickRaidus, this.StickLength, this.Segments);
        }
        public void Update()
        {
            switch (this.InputMode)
            {
                case Modes.Vector:
                    this.UpdateVectorInfo();
                    break;
                case Modes.Rotate:
                    this.UpdateRotateInfo();
                    break;
            }
        }
        public virtual void AddData(Vector3D V)
        {
            switch (this.InputMode)
            {
                case Modes.Rotate:
                    this.AddRotateValue(V.X, V.Y, V.Z);
                    break;
                case Modes.Vector:
                    this.AddVectorValue(V);
                    break;
            }
        }
        protected virtual void AddRotateValue(Vector3D V)
        {
            this.AddRotateValue(V.X, V.Y, V.Z);
        }

        //滚转角（x轴）Roll
        //俯仰角（y轴）Pitch
        //偏航角（z轴）Yaw
        protected virtual void AddRotateValue(double Roll, double Pitch, double Yaw)
        {
            this.lastVector.X = Roll;
            this.lastVector.Y = Pitch;
            this.lastVector.Z = Yaw;

            this.lastSTD = this.lastVector;

            Vector3D N = this.lastVector - this.zeroVector;

            this.YAxisRotation.Angle = N.Y;
            this.ZAxisRotation.Angle = N.Z;
            this.XAxisRotation.Angle = N.X;
            if (this.AutoUpdate)
            {
                this.Update();
            }
        }

        protected virtual void AddVectorValue(Vector3D V)
        {
            Vector3D N = (this.lastVector = V) - this.zeroVector;

            if ((S = N.Length) > 0.0)
            {
                S *= Math.Sign(N.Z);

                this.InputMode = Modes.Vector;

                T = Math.Acos(N.Z / S);
                D = Math.Atan2(N.Y, N.X);

                var M = Matrix3D.Identity;

                M.Rotate(new Quaternion(new Vector3D(0.0, 1.0, 0.0), T * 180.0 / Math.PI));
                M.Rotate(new Quaternion(new Vector3D(0.0, 0.0, 1.0), D * 180.0 / Math.PI));

                this.Mat.Matrix = M;

                if (!double.IsNaN(this.scaleFactor))
                {
                    this.HeadLengthScale.ScaleZ
                    = this.TailLengthScale.ScaleZ
                    = S * this.scaleFactor;
                }
                this.lastSTD = new Vector3D(S, T, D);
            }
            else
            {
                this.S = 0.0;
                this.T = 0.0;
                this.D = 0.0;
            }
            if (this.AutoUpdate)
            {
                this.Update();
            }
        }


        private void UpdateVectorInfo()
        {
            this.XValueText.Text = this.XText + string.Format(_TextFormatTools.FormatText, _TextFormatTools.AlignDoubleValue(this.lastVector.X), this.ValueUnit);
            this.YValueText.Text = this.YText + string.Format(_TextFormatTools.FormatText, _TextFormatTools.AlignDoubleValue(this.lastVector.Y), this.ValueUnit);
            this.ZValueText.Text = this.ZText + string.Format(_TextFormatTools.FormatText, _TextFormatTools.AlignDoubleValue(this.lastVector.Z), this.ValueUnit);
            this.SValueText.Text = this.SText + string.Format(_TextFormatTools.FormatText, _TextFormatTools.AlignDoubleValue(S), this.ValueUnit);
            this.TValueText.Text = this.TText + string.Format(_TextFormatTools.FormatText, _TextFormatTools.AlignDoubleValue(T), this.AngleUnit);
            this.DValueText.Text = this.DText + string.Format(_TextFormatTools.FormatText, _TextFormatTools.AlignDoubleValue(D), this.AngleUnit);
        }


        protected void UpdateRotateInfo()
        {
            this.XValueText.Text = this.SText + string.Format(_TextFormatTools.FormatText, _TextFormatTools.AlignDoubleValue(this.lastVector.X), this.AngleUnit);
            this.YValueText.Text = this.TText + string.Format(_TextFormatTools.FormatText, _TextFormatTools.AlignDoubleValue(this.lastVector.Y), this.AngleUnit);
            this.ZValueText.Text = this.DText + string.Format(_TextFormatTools.FormatText, _TextFormatTools.AlignDoubleValue(this.lastVector.Z), this.AngleUnit);
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            this.BuildParts();
        }
        protected virtual MeshGeometry3D BuildDisk(double R, double Delta)
        {
            var pc = new Point3DCollection();
            var tc = new Int32Collection();

            double x1 = R * Math.Cos(0.0);
            double y1 = R * Math.Sin(0.0);

            int idx = 0;
            for (double d = Delta; d <= 2.0 * Math.PI; d += Delta)
            {
                double x2 = R * Math.Cos(d);
                double y2 = R * Math.Sin(d);
                pc.Add(new Point3D(0.0, 0.0, 0.0));
                pc.Add(new Point3D(x1, y1, 0.0));
                pc.Add(new Point3D(x2, y2, 0.0));
                tc.Add(idx++);
                tc.Add(idx++);
                tc.Add(idx++);
                x1 = x2;
                y1 = y2;
            }

            return new MeshGeometry3D
            {
                Positions = pc,
                TriangleIndices = tc
            };
        }
        protected virtual MeshGeometry3D BuildCube(double length, double width, double height, double x = 0, double y = 0, double z = 0)
        {
            List<Point3D> Points = new List<Point3D>();
            List<int> Indices = new List<int>();

            Points.Add(new Point3D(length / 2 + x, width / 2 + y, height / 2 + z));
            Points.Add(new Point3D(length / 2 + x, width / 2 + y, -height / 2 + z));
            Points.Add(new Point3D(-length / 2 + x, width / 2 + y, -height / 2 + z));
            Points.Add(new Point3D(-length / 2 + x, width / 2 + y, height / 2 + z));
            Points.Add(new Point3D(length / 2 + x, -width / 2 + y, height / 2 + z));
            Points.Add(new Point3D(length / 2 + x, -width / 2 + y, -height / 2 + z));
            Points.Add(new Point3D(-length / 2 + x, -width / 2 + y, -height / 2 + z));
            Points.Add(new Point3D(-length / 2 + x, -width / 2 + y, height / 2 + z));

            //f0
            Indices.Add(0);
            Indices.Add(2);
            Indices.Add(3);
            //
            Indices.Add(0);
            Indices.Add(1);
            Indices.Add(2);

            //f1
            Indices.Add(4);
            Indices.Add(3);
            Indices.Add(7);
            //
            Indices.Add(4);
            Indices.Add(0);
            Indices.Add(3);

            //f2
            Indices.Add(5);
            Indices.Add(0);
            Indices.Add(4);
            //
            Indices.Add(5);
            Indices.Add(1);
            Indices.Add(0);

            //f3
            Indices.Add(6);
            Indices.Add(1);
            Indices.Add(5);
            //
            Indices.Add(6);
            Indices.Add(2);
            Indices.Add(1);

            //f4
            Indices.Add(7);
            Indices.Add(2);
            Indices.Add(6);
            //
            Indices.Add(7);
            Indices.Add(3);
            Indices.Add(2);

            //f5
            Indices.Add(4);
            Indices.Add(6);
            Indices.Add(5);
            //
            Indices.Add(4);
            Indices.Add(7);
            Indices.Add(6);


            return new MeshGeometry3D
            {
                Positions = new Point3DCollection(Points),
                TriangleIndices = new Int32Collection(Indices)
            };
        }
        protected virtual MeshGeometry3D BuildCone(double radius, double height, int segments = 64, double x = 0, double y = 0, double z = 0, bool sideOnly = false)
        {
            List<Point3D> Points = new List<Point3D>();
            List<int> Indices = new List<int>();
            Points.Add(new Point3D(x, y, z));
            Points.Add(new Point3D(x, y, z + height));
            for (int i = 0; i < segments; i++)
            {
                double d = 2 * Math.PI / segments * i;
                Points.Add(new Point3D(Math.Cos(d) * radius + x, Math.Sin(d) * radius + y, z));
            }

            for (int i = 2; i < segments + 2; i++)
            {
                int j = i + 1;

                if (j >= segments + 2)
                {
                    j = 2;
                }

                if (!sideOnly)
                {
                    //bottom
                    Indices.Add(0);
                    Indices.Add(i);
                    Indices.Add(j);
                }
                //side
                Indices.Add(1);
                Indices.Add(j);
                Indices.Add(i);

            }
            return new MeshGeometry3D
            {
                Positions = new Point3DCollection(Points),
                TriangleIndices = new Int32Collection(Indices)
            };
        }
        protected virtual MeshGeometry3D BuildCylinder(double radius, double height, int segments = 64, double x = 0, double y = 0, double z = 0, bool sideOnly = false)
        {
            List<Point3D> Points = new List<Point3D>();
            List<int> Indices = new List<int>();
            Points.Add(new Point3D(x, y, z));
            Points.Add(new Point3D(x, y, z + height));
            for (int i = 0; i < segments; i++)
            {
                double d = 2 * Math.PI / segments * i;

                double lx = Math.Cos(d) * radius;
                double ly = Math.Sin(d) * radius;

                Points.Add(new Point3D(lx + x, ly + y, z));
                Points.Add(new Point3D(lx + x, ly + y, z + height));
            }

            for (int i = 0; i < segments; i++)
            {
                int j = i + 1;

                if (j >= segments)
                {
                    j = 0;
                }

                if (!sideOnly)
                {
                    //bottom
                    Indices.Add(0);
                    Indices.Add((i + 1) * 2);
                    Indices.Add((j + 1) * 2);

                    //top
                    Indices.Add(1);
                    Indices.Add((j + 1) * 2 + 1);
                    Indices.Add((i + 1) * 2 + 1);
                }
                //sides
                Indices.Add((j + 1) * 2);
                Indices.Add((i + 1) * 2);
                Indices.Add((i + 1) * 2 + 1);

                Indices.Add((i + 1) * 2 + 1);
                Indices.Add((j + 1) * 2 + 1);
                Indices.Add((j + 1) * 2);
            }
            return new MeshGeometry3D
            {
                Positions = new Point3DCollection(Points),
                TriangleIndices = new Int32Collection(Indices)
            };
        }
        protected virtual MeshGeometry3D BuildHalfCone(double radiusBottom, double radiusTop, double height, int segments = 64, double x = 0, double y = 0, double z = 0, bool sideOnly = false)
        {
            List<Point3D> Points = new List<Point3D>();

            List<int> Indices = new List<int>();
            Points.Add(new Point3D(x, y, z));
            Points.Add(new Point3D(x, y, z + height));
            for (int i = 0; i < segments; i++)
            {
                double d = 2 * Math.PI / segments * i;

                double lxb = Math.Cos(d) * radiusBottom;
                double lyb = Math.Sin(d) * radiusBottom;

                double lxt = Math.Cos(d) * radiusTop;
                double lyt = Math.Sin(d) * radiusTop;

                Points.Add(new Point3D(lxb + x, lyb + y, z));
                Points.Add(new Point3D(lxt + x, lyt + y, z + height));
            }

            for (int i = 0; i < segments; i++)
            {
                int j = i + 1;

                if (j >= segments)
                {
                    j = 0;
                }

                if (!sideOnly)
                {
                    //bottom
                    Indices.Add(0);
                    Indices.Add((i + 1) * 2);
                    Indices.Add((j + 1) * 2);

                    //top
                    Indices.Add(1);
                    Indices.Add((j + 1) * 2 + 1);
                    Indices.Add((i + 1) * 2 + 1);
                }
                //sides
                Indices.Add((j + 1) * 2);
                Indices.Add((i + 1) * 2);
                Indices.Add((i + 1) * 2 + 1);

                Indices.Add((i + 1) * 2 + 1);
                Indices.Add((j + 1) * 2 + 1);
                Indices.Add((j + 1) * 2);
            }
            return new MeshGeometry3D
            {
                Positions = new Point3DCollection(Points),
                TriangleIndices = new Int32Collection(Indices)
            };
        }
        protected virtual MeshGeometry3D BuildSphere(double radius, int segments = 64, double x = 0.0, double y = 0.0, double z = 0.0)
        {
            List<Point3D> Points = new List<Point3D>();

            List<int> Indices = new List<int>();
            double d = 2 * Math.PI / segments;
            double h = 0.0;
            double dh = 0.0;
            double ly = radius;
            double rb = radius;
            double rt = 0.0;
            double h2 = 0.0;
            rt = Math.Cos(d) * radius;
            dh = Math.Sin(d) * radius;

            for (int i = -segments / 4; i < segments / 4; i++)
            {
                if (i == segments / 4 - 1 || i == -segments / 4 + 1)
                {
                    rt = 0.0;
                }

                var t = this.BuildHalfCone(rb, rt, dh, segments, x, h2, z, true);

                rb = rt;

                rt = Math.Cos(d * (i + 1)) * radius;

                h2 += dh;

                double nh = Math.Sin(d * (i + 1)) * radius;

                dh = nh - h;

                h = nh;

                Points.AddRange(t.Positions);

                Indices.AddRange(t.TriangleIndices.Select(idx => idx + Points.Count));
            }

            return new MeshGeometry3D
            {
                Positions = new Point3DCollection(Points),
                TriangleIndices = new Int32Collection(Indices)
            };
        }

    }
}
