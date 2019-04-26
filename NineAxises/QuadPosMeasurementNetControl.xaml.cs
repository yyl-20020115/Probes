using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Probes
{
    /// <summary>
    /// QuadPosMeasurementNetControl.xaml 的交互逻辑
    /// </summary>
    public partial class QuadPosMeasurementNetControl : MeasurementBaseNetControl
    {
        public override int ReceivePartLength => 52;
        public override string[] Headers => new string[] { "W:", "WEIGHT:" };
        protected override Grid LinesGrid => this.Lines;
        protected override CheckBox PauseCheckBox => this.Pause;
        protected override ComboBox RemoteAddressComboBox => this._RemoteAddressComboBox;
        protected override CheckBox SetRemoteCheckBox => this._SetRemoteCheckBox;

        protected DispatcherTimer CommandTimer = new DispatcherTimer();
        protected TimeSpan DefaultCommandInterval = TimeSpan.FromMilliseconds(40);

        public QuadPosMeasurementNetControl()
        {
            this.LinesGroup[0].Description = "Position";
            this.CommandTimer.Interval = DefaultCommandInterval;
            this.CommandTimer.Tick += Timer_Tick;
        }
        protected override void CallInitializeComponent()
        {
            InitializeComponent();
        }
        protected virtual void Timer_Tick(object sender, System.EventArgs e)
        {
            this.Send("#?data%");
        }
        protected override void OnReceivedInternal(string input)
        {

        }
        protected override void SetRemoteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            base.SetRemoteCheckBox_Checked(sender, e);
            this.CommandTimer.Start();

        }
        protected override void SetRemoteCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            base.SetRemoteCheckBox_Unchecked(sender, e);
            this.CommandTimer.Stop();
        }


        ////------------------------------
        //int check_end = 0;
        //String result_str;
        ////----------------------------------
        //String Reading_Str;
        //Byte start_record_sign = 0; // 0:start, 1: pause, 2: stop
        //Byte read_cmd_sign = 0;
        //ushort record_interval = 60;
        //int record_time = 60, total_record_time = 3600;
        //int interval_counter = 0;
        //Byte total_time_chg_sign = 0, total_point_chg_sign = 0, pause_resume_sign = 0;
        //int total_point = 60, left_point_counter = 60;
        //double[] power_data;
        //double laser_power_value;
        //double max_power, min_power, aver_power, temp_power_sum, rmsdev_power;
        //Byte get_direct_pos_sign = 0;

        //Byte get_quad_type_sign = 0;
        //Byte quad_type = 0;

        ////115200,n,8,1
        //string GetTypeCommand = "#?type%";
        //string GetDataCommand = "#?data%";
        //string GetPosCommand = "#?pos%";
        //string SetUpdownCommand0 = "#SI:%02d%";
        //string SetUpdownCommand1 = "#SS:%03d%";


        ////---------------------------------------------------------------------------
        //Byte int_pos_sign = 0;

        //void cal_beam_pos()
        //{

        //    char temp_str[35];
        //    if (Reading_Str.Length() > 32)
        //        return;
        //    sprintf(temp_str, "%s", Reading_Str.c_str());

        //    int i, k = 0;
        //    long temp_max;
        //    int vol_data[4];
        //    ushort temp_D1, temp_D2, temp_D3, temp_D4;
        //    double xx, yy, temp_total;
        //    char xpos_str[10], ypos_str[10];


        //    ZeroMemory((Byte*)vol_data, 4 * sizeof(int));

        //    Edit9->Text = temp_str;

        //    if (quad_type == 1)
        //    {
        //        for (i = 0; i < 24; i++)
        //        {
        //            if (temp_str[i] > ('0' + 15) || temp_str[i] < '0')
        //                return;
        //        }

        //        temp_max = 0;

        //        for (k = 0; k < 24; k += 6)
        //        {
        //            vol_data[k / 6] = (temp_str[k] - '0') * 256 * 256 * 16 + (temp_str[k + 1] - '0') * 256 * 256 + (temp_str[k + 2] - '0') * 256 * 16
        //                           + (temp_str[k + 3] - '0') * 256 + (temp_str[k + 4] - '0') * 16 + (temp_str[k + 5] - '0');

        //            if (vol_data[k / 6] > temp_max)
        //                temp_max = vol_data[k / 6];
        //        }

        //        /*
        //                for(k=0;k<12;k+=3)
        //                {
        //                        vol_data[k/3]=(temp_str[k]-'0')*256+(temp_str[k+1]-'0')*16+(temp_str[k+2]-'0');

        //                        if(vol_data[k/3]>temp_max)
        //                              temp_max=vol_data[k/3];
        //                }
        //       */
        //        if (temp_max < 1000)
        //        {
        //            temp_D1 = temp_D2 = temp_D3 = temp_D4 = 0;
        //        }
        //        else
        //        {
        //            temp_D1 = (int)(vol_data[0] * 1.0 / temp_max * 4095);
        //            temp_D2 = (int)(vol_data[1] * 1.0 / temp_max * 4095);
        //            temp_D3 = (int)(vol_data[2] * 1.0 / temp_max * 4095);
        //            temp_D4 = (int)(vol_data[3] * 1.0 / temp_max * 4095);
        //        }

        //        Edit1->Text = String(temp_D1);
        //        D00->Position = temp_D1;

        //        Edit3->Text = String(temp_D2);
        //        D01->Position = temp_D2;

        //        Edit4->Text = String(temp_D3);
        //        D10->Position = temp_D3;

        //        Edit5->Text = String(temp_D4);
        //        D11->Position = temp_D4;


        //        temp_total = vol_data[0] + vol_data[1] + vol_data[2] + vol_data[3];
        //        if (temp_total < 1000)
        //            xx = 0;
        //        else
        //            xx = (vol_data[1] + vol_data[3] - vol_data[0] - vol_data[2]) * 1.0 / temp_total;
        //        xx = (xx + 1) * 50;

        //        if (temp_total < 1000)
        //            yy = 0;
        //        else
        //            yy = (vol_data[0] + vol_data[1] - vol_data[2] - vol_data[3]) * 1.0 / temp_total;
        //        yy = (yy + 1) * 50;

        //        Series1->Clear();
        //        Series1->AddXY(xx, yy);
        //        xPos_Label->Caption = String(xx);

        //        sprintf(xpos_str, "X:%.1f", xx);
        //        xPos_Label->Caption = xpos_str;
        //        sprintf(ypos_str, "Y:%.1f", yy);
        //        yPos_Label->Caption = ypos_str;
        //    }
        //    else if (quad_type == 0)
        //    {
        //        if (int_pos_sign == 0)
        //        {
        //            if (get_direct_pos_sign == 1)
        //                int_pos_sign = 1;

        //            for (i = 0; i < 12; i++)
        //            {
        //                if (temp_str[i] > ('0' + 15) || temp_str[i] < '0')
        //                    return;
        //            }

        //            for (k = 0; k < 12; k += 3)
        //            {
        //                vol_data[k / 3] = (temp_str[k] - '0') * 256 + (temp_str[k + 1] - '0') * 16 + (temp_str[k + 2] - '0');
        //            }


        //            Edit1->Text = String(vol_data[0]);
        //            D00->Position = vol_data[0];

        //            Edit3->Text = String(vol_data[1]);
        //            D01->Position = vol_data[1];

        //            Edit4->Text = String(vol_data[2]);
        //            D10->Position = vol_data[2];

        //            Edit5->Text = String(vol_data[3]);
        //            D11->Position = vol_data[3];


        //            temp_total = vol_data[0] + vol_data[1] + vol_data[2] + vol_data[3];
        //            if (temp_total < 10)
        //                xx = 0;
        //            else
        //                xx = (vol_data[1] + vol_data[3] - vol_data[0] - vol_data[2]) * 1.0 / temp_total;
        //            xx = (xx + 1) * 50;

        //            if (temp_total < 10)
        //                yy = 0;
        //            else
        //                yy = (vol_data[0] + vol_data[1] - vol_data[2] - vol_data[3]) * 1.0 / temp_total;
        //            yy = (yy + 1) * 50;

        //            Series1->Clear();
        //            Series1->AddXY(xx, yy);
        //            xPos_Label->Caption = String(xx);

        //            sprintf(xpos_str, "X:%.1f", xx);
        //            xPos_Label->Caption = xpos_str;
        //            sprintf(ypos_str, "Y:%.1f", yy);
        //            yPos_Label->Caption = ypos_str;
        //        }
        //        else if (int_pos_sign == 1)
        //        {
        //            int_pos_sign = 0;
        //            for (i = 0; i < 6; i++)
        //            {
        //                if (temp_str[i] > ('0' + 15) || temp_str[i] < '0')
        //                    return;
        //            }
        //            for (k = 0; k < 6; k += 3)
        //            {
        //                vol_data[k / 3] = (temp_str[k] - '0') * 256 + (temp_str[k + 1] - '0') * 16 + (temp_str[k + 2] - '0');
        //            }
        //            Series2->Clear();
        //            Series2->AddXY(vol_data[0] / 10.0, vol_data[1] / 10.0);
        //        }
        //    }//else if(quad_type==0)
        //}


        //---------------------------------------------------------------------------
        //        void __fastcall TForm1::Reading_TimerTimer(TObject* Sender)
        //{
        //      if(get_quad_type_sign<4)
        //      {
        //            char temp_str[10];
        //            if(get_quad_type_sign==0)
        //            {
        //                MSComm1->Output=StringToOleStr("#?type%"); get_quad_type_sign=1;
        //            }
        //            else if(get_quad_type_sign==1)
        //            {
        //                   get_quad_type_sign=2;             //CCD4Q
        //                   if(Reading_Str.Length()>9)
        //                        return;
        //                   sprintf(temp_str,"%s", Reading_Str.c_str());
        //                   if(temp_str[0]=='C'&&temp_str[1]=='C'&&temp_str[2]=='D'&&temp_str[3]=='4'&&temp_str[4]=='Q')
        //                   {
        //                        quad_type=1;
        //                        get_quad_type_sign=5;

        //                        On1->Enabled=false;
        //                        Button2->Caption="Set Sens";
        //                        UpDown1->Min=0;
        //                        UpDown1->Max=5000;
        //                        ResPosEdit->Text="150";
        //                        ResPosEdit->Color=clWhite;

        //                        Chart2->Enabled=false;
        //                        Caption="CCD-4Quad connected";
        //                        return;
        //                   }
        //            }
        //            else if(get_quad_type_sign==2)
        //            {
        //                MSComm1->Output=StringToOleStr("#?type%"); get_quad_type_sign=3;
        //            }
        //            else if(get_quad_type_sign==3)
        //            {
        //                   get_quad_type_sign=5;             //CCD4Q
        //                   if(Reading_Str.Length()>9)
        //                        return;
        //                   sprintf(temp_str,"%s", Reading_Str.c_str());
        //                   if(temp_str[0]=='C'&&temp_str[1]=='C'&&temp_str[2]=='D'&&temp_str[3]=='4'&&temp_str[4]=='Q')
        //                   {
        //                        quad_type=1;
        //                        get_quad_type_sign=5;

        //                        On1->Enabled=false;
        //                        Button2->Caption="Set Sens";
        //                        UpDown1->Min=0;
        //                        UpDown1->Max=5000;
        //                        ResPosEdit->Text="150";
        //                        ResPosEdit->Color=clWhite;

        //                         Chart2->Enabled=false;
        //                        Caption="CCD-4Quad connected";

        //                        return;
        //                   }
        //            }
        //            return;
        //      }
        ////-------------------------------------------------
        //      if(read_cmd_sign==0)
        //      {
        //           read_cmd_sign=1;
        //           Reading_Str="";
        //           if(quad_type==0)
        //           {
        //                if(int_pos_sign==0)
        //                        MSComm1->Output=StringToOleStr("#?data%");
        //                else
        //                        MSComm1->Output=StringToOleStr("#?pos%");
        //           }
        //           else
        //                MSComm1->Output=StringToOleStr("#?data%");
        //      }
        //      else
        //      {
        //           read_cmd_sign=0;
        //           cal_beam_pos();
        //      }
        //}


    }
}
