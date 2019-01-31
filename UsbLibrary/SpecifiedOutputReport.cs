using System;

namespace UsbLibrary
{
    public class SpecifiedOutputReport : OutputReport
    {
        public SpecifiedOutputReport(HIDDevice oDev)
            : base(oDev)
        {

        }

        public bool SendData(byte[] data)
        {
            byte[] arrBuff = Buffer; //new byte[Buffer.Length];
            for (int i = 0; i <Math.Min( arrBuff.Length,data.Length); i++)
            {
                if(i+1<arrBuff.Length)
                    arrBuff[i+1] = data[i];
            }

            //Buffer = arrBuff;

            //returns false if the data does not fit in the buffer. else true
            //if (arrBuff.Length < data.Length)
            //{
            //    return false;
            //}
            //else
            {
                m_nLength = data.Length + 1;

                return true;
            }
        }
    }
}
