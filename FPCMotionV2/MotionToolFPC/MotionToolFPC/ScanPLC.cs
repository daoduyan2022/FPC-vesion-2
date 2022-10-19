using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol;
using System.Threading;

namespace MotionToolFPC
{
    public class ScanPLC
    {
        private static ScanPLC plc = null;
        private iQF fx5u { get; set; } = new iQF();
        public Mode IsRead = Mode.Read;
        public bool EnableScanPLC = true;
        public bool IsError = false;
        public List<dataSend> dataSends = new List<dataSend>();
        private Globals globals = Globals.GetInstance();
        object newob = new object();


        public static ScanPLC GetInstance()
        {
            if (plc == null)
            {
                plc = new ScanPLC();
            }
            return plc;
        }
        public bool ConnectPLC(string IpAddress, int port)
        {
            try
            {
                IsError = !fx5u.Connect(IpAddress, port);
            }
            catch
            {
                IsError = false;
            }
            return IsError;
        }
        public void DisconnectPLC()
        {
            fx5u.Disconnect();
        }
        public void StartScan()
        {
            while (true)
            {
                if (!EnableScanPLC)
                {
                    break;
                }

                if (IsRead == Mode.Read)
                {
                    try
                    {
                        globals.D0D499 = fx5u.ReadHoldingRegister(0, 500);
                        globals.D500D999 = fx5u.ReadHoldingRegister(500, 500);
                        IsError = false;
                    }
                    catch
                    {
                        IsError = true;
                        globals.D0D499[499] = 0;
                    }
                }
                else if (IsRead == Mode.Write)
                {
                    foreach (dataSend s in dataSends)
                    {
                        try
                        {
                            fx5u.WriteMultiRegister(s.startAddress, s.data.Length, s.data);
                            IsError = false;
                        }
                        catch
                        {
                            IsError = true;
                            break;
                        }
                    }
                    IsRead = Mode.Read;
                    dataSends.Clear();
                }

                Thread.Sleep(50);
            }

        }
    }


    public class dataSend
    {
        public int[] data { get; set; }
        public int startAddress { get; set; }
        public dataSend(int[] data, int startAddress)
        {
            this.data = data;
            this.startAddress = startAddress;
        }
    }

    public enum Mode
    {
        Read,
        Write,
        Wait
    }

}



