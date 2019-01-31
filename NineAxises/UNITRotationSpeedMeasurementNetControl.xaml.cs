using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UsbLibrary;

namespace Probes
{
    /// <summary>
    /// UNITRotationSpeedMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class UNITRotationSpeedMeasurementNetControl : MeasurementBaseNetControl
    {
        public delegate void OnReceiveDoubleDataDelegate(double data);

        public override int ReceivePartLength => 21;
        public override string[] Headers => new string[] { "RS:" };
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;

        protected const int DefaultSysFrequency = 168000000;
        protected const double ScaleFactor = 1.0;
        protected OnReceiveDoubleDataDelegate onReceiveDoubleData = null;
        protected UsbHidPort port = null;
        public UNITRotationSpeedMeasurementNetControl()
        {
            this.LinesGroup[0].Description = "Rotation Speed in RPM";
            this.LinesGroup[0].Stroke = Brushes.Green;
            this.onReceiveDoubleData = new OnReceiveDoubleDataDelegate(OnReceiveDoubleData);
            this.port = new UsbHidPort();
            this.port.OnDataRecieved += Port_OnDataRecieved;
            this.port.OnDataSend += Port_OnDataSend;
            this.port.OnDeviceArrived += Port_OnDeviceArrived;
            this.port.OnDeviceRemoved += Port_OnDeviceRemoved;
            this.port.OnSpecifiedDeviceArrived += Port_OnSpecifiedDeviceArrived;
            this.port.OnSpecifiedDeviceRemoved += Port_OnSpecifiedDeviceRemoved;
        }

        protected virtual void Port_OnSpecifiedDeviceRemoved(object sender, EventArgs e)
        {

        }

        protected virtual void Port_OnSpecifiedDeviceArrived(object sender, EventArgs e)
        {

        }

        protected virtual void Port_OnDeviceRemoved(object sender, EventArgs e)
        {

        }

        protected virtual void Port_OnDeviceArrived(object sender, EventArgs e)
        {

        }

        protected virtual void Port_OnDataSend(object sender, EventArgs e)
        {

        }

        protected virtual void Port_OnDataRecieved(object sender, DataRecievedEventArgs args)
        {
            if (args != null && this.working)
            {
                this.OnInput(args.data);
            }
        }

        protected override void CallInitializeComponent()
        {
            this.InitializeComponent();
        }
        protected bool working = false;
        protected override void SetRemoteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            string rat = this.RemoteAddressText;
            if (!string.IsNullOrEmpty(rat))
            {
                string[] parts = rat.Split(',');
                if(parts.Length == 2)
                {
                    string vid = parts[0] != null && parts[0].Length >= 4 ? parts[0].Substring(4) : string.Empty;
                    string pid = parts[1] != null && parts[1].Length >= 4 ? parts[1].Substring(4) : string.Empty;
                    if(int.TryParse(vid, System.Globalization.NumberStyles.HexNumber,null, out int vidn))
                    {
                        this.port.VendorId = vidn;
                    }
                    if (int.TryParse(pid, System.Globalization.NumberStyles.HexNumber, null, out int pidn))
                    {
                        this.port.ProductId = pidn;
                    }
                    var sp = this.port.CheckDevicePresent();
                    if (sp != null)
                    {
                        //Enable Command, sending to device
                        sp.SetFeature(new byte[] 
                        { 0x00, 0x60, 0x09, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00 });
                        this.working = true;
                    }
                }
            }

        }
        protected override void SetRemoteCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.working = false;
            this.port.ProductId = 0;
            this.port.VendorId = 0;
            this.port.CheckDevicePresent();
        }
        protected StringBuilder builder = new StringBuilder();


        protected void OnInput(byte[] buffer)
        {
            if (buffer != null && buffer.Length >= 2)
            {
                int ct = (buffer[1] & 0x0f);
                int rc = ct < buffer.Length - 1 ? ct : buffer.Length - 1;

                for (int i = 0; i < rc; i++)
                {
                    builder.AppendFormat("{0:X1}", (buffer[i + 2] & 0xf));
                }
                string text = builder.ToString();
                if (text.Length >= 26 && text.EndsWith("DA"))
                {
                    builder.Clear();

                    string value = this.ExtractInfo(this.TranslateInput(text), out string time);

                    if (double.TryParse(value, out double data))
                    {
                        Dispatcher.BeginInvoke(this.onReceiveDoubleData, data);
                    }
                }
            }
        }
        protected virtual void OnReceiveDoubleData(double data)
        {
            this.AddData(data);
        }
        protected string TranslateInput(string text)
        {
            StringBuilder result = new StringBuilder();
            if (!string.IsNullOrEmpty(text))
            {
                if (text.Length > 26)
                {
                    text = text.Substring(text.Length - 26);
                }
                int digits = 0;
                bool sign = false;
                result.Append(this.Decode(text.Substring(0, 2), false, out sign));
                result.Append(this.Decode(text.Substring(2, 2), true, out sign));
                if (sign)
                {
                    digits = 4;
                }
                result.Append(this.Decode(text.Substring(4, 2), true, out sign));
                if (sign)
                {
                    digits = 3;
                }
                result.Append(this.Decode(text.Substring(6, 2), true, out sign));
                if (sign)
                {
                    digits = 2;
                }
                result.Append(this.Decode(text.Substring(8, 2), true, out sign));
                if (sign)
                {
                    digits = 1;
                }
                result.Append(digits);
                result.Append(this.Decode(text.Substring(10, 2), false, out sign));
                result.Append(this.Decode(text.Substring(12, 2), true, out sign));
                digits = 5;
                if (sign)
                {
                    digits = 1;
                }
                result.Append(this.Decode(text.Substring(14, 2), true, out sign));
                if (sign)
                {
                    digits = 2;
                }
                result.Append(this.Decode(text.Substring(16, 2), true, out sign));
                if (sign)
                {
                    digits = 3;
                }
                result.Append(this.Decode(text.Substring(18, 2), true, out sign));
                if (sign)
                {
                    digits = 4;
                }
                result.Append(digits);
                result.Append(text.Substring(20, 4));
                result.Append("DA");
            }

            return result.ToString();
        }
        protected string Decode(string part, bool ext, out bool sign)
        {
            sign = false;
            string result = string.Empty;
            if (!string.IsNullOrEmpty(part))
            {
                if (!ext && part == "0B")
                {
                    result = "L";
                }
                else if (ext && part == "8B")
                {
                    sign = true;
                    result = "L";
                }
                if (result == string.Empty)
                {
                    switch (part)
                    {
                        case "7B":
                            result = "0";
                            break;
                        case "60":
                            result = "1";
                            break;
                        case "5E":
                            result = "2";
                            break;
                        case "7C":
                            result = "3";
                            break;
                        case "65":
                            result = "4";
                            break;
                        case "3D":
                            result = "5";
                            break;
                        case "3F":
                            result = "6";
                            break;
                        case "70":
                            result = "7";
                            break;
                        case "7F":
                            result = "8";
                            break;
                        case "7D":
                            result = "9";
                            break;

                        default:
                            if (ext)
                            {
                                switch (part)
                                {
                                    case "FB":
                                        sign = true;
                                        result = "0";
                                        break;
                                    case "E0":
                                        sign = true;
                                        result = "1";
                                        break;
                                    case "DE":
                                        sign = true;
                                        result = "2";
                                        break;
                                    case "FC":
                                        sign = true;
                                        result = "3";
                                        break;
                                    case "E5":
                                        sign = true;
                                        result = "4";
                                        break;
                                    case "BD":
                                        sign = true;
                                        result = "5";
                                        break;
                                    case "BF":
                                        sign = true;
                                        result = "6";
                                        break;
                                    case "F0":
                                        sign = true;
                                        result = "7";
                                        break;
                                    case "FD":
                                        sign = true;
                                        result = "9";
                                        break;
                                    default:
                                        result = "G";
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            return result;
        }
        protected string ExtractInfo(string text, out string time)
        {
            time = string.Empty;
            StringBuilder builder = new StringBuilder();
            if (!string.IsNullOrEmpty(text) && text.Length == 18 && text.EndsWith("DA"))
            {
                //0,1,2,3,4
                if (text.Substring(0, 5).Contains("L"))
                {
                    builder.Append("@0");
                }
                else if (text.Substring(6, 5).Contains("L"))
                {
                    builder.Append("@0");
                }
                else
                {
                    time = string.Format("{0}{1}:{2}{3}", text[9], text[8], text[7], text[6]);
                    int ret = text[11] - '0';
                    if (ret != 0)
                    {

                    }

                    int ret2 = text[12] - '0';
                    if ((ret2 & 1) == 1)
                    {
                        //odd
                    }
                    int ret3 = text[13] - '0';  //Hoding 
                    if ((ret3 & 1) == 1)
                    {
                        //odd
                    }
                    int ret4 = text[14] - '0';  //Max Min Avg
                    if ((ret4 & 1) == 1)
                    {
                        //odd
                    }
                    int ret5 = text[15] - '0';  //Count/RPM
                    if ((ret5 & 1) == 1)
                    {
                        //odd
                    }

                    int digits = text[5] - '0';
                    switch (digits)
                    {
                        case 1:
                            builder.AppendFormat("{0}.{1}{2}{3}{4}", text[4], text[3], text[2], text[1], text[0]);
                            break;
                        case 2:
                            builder.AppendFormat("{0}{1}.{2}{3}{4}", text[4], text[3], text[2], text[1], text[0]);
                            break;
                        case 3:
                            builder.AppendFormat("{0}{1}{2}.{3}{4}", text[4], text[3], text[2], text[1], text[0]);
                            break;
                        case 4:
                            builder.AppendFormat("{0}{1}{2}{3}.{4}", text[4], text[3], text[2], text[1], text[0]);
                            break;
                        case 5:
                            builder.AppendFormat("{0}{1}{2}{3}{4}", text[4], text[3], text[2], text[1], text[0]);
                            break;
                    }
                }

            }
            System.Diagnostics.Debug.WriteLine(builder.ToString());
            return builder.ToString();
        }
    }
}
