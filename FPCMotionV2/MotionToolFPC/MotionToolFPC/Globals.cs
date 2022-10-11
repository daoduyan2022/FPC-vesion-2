using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using EasyModbus;
using System.Collections;
using System.Diagnostics;

namespace MotionToolFPC
{
    public class Globals
    {
        public string PathConfig = @"D:\TSB_Project\FPCMotionV3\FPC-vesion-2\FPCMotionV2\MotionToolFPC\FPC\bin\Debug\Config\config.txt";
        public string PathModel = @"D:\TSB_Project\FPCMotionV3\FPC-vesion-2\FPCMotionV2\MotionToolFPC\FPC\bin\Debug\Config\model2108.txt";
        public string PathFunction = @"D:\TSB_Project\FPCMotionV3\FPC-vesion-2\FPCMotionV2\MotionToolFPC\FPC\bin\Debug\Config\Function.txt";
        private static Globals globals = null;
        public static Globals GetInstance()
        {
            if (globals == null)
            {
                globals = new Globals();
            }
            return globals;
        }
        public int[] D0D499 { get; set; } = new int[500];
        public int[] D500D999 { get; set; } = new int[500];

        public int CurrentPosX1
        {
            get { return ModbusClient.ConvertRegistersToInt(new int[] { this.D0D499[22], this.D0D499[23] }, ModbusClient.RegisterOrder.LowHigh); }
        }
        public int CurrentPosX2
        {
            get { return ModbusClient.ConvertRegistersToInt(new int[] { this.D0D499[24], this.D0D499[25] }, ModbusClient.RegisterOrder.LowHigh); }
        }
        public int CurrentPosY
        {
            get { return ModbusClient.ConvertRegistersToInt(new int[] { this.D0D499[26], this.D0D499[27] }, ModbusClient.RegisterOrder.LowHigh); }
        }
        public int CurrentPosR
        {
            get { return ModbusClient.ConvertRegistersToInt(new int[] { this.D0D499[20], this.D0D499[21] }, ModbusClient.RegisterOrder.LowHigh); }
        }

        public bool[] X0X17
        {
            get { return new BitArray(new int[] { this.D0D499[50] }).Cast<bool>().ToArray(); }
        }
        public bool[] X20X37
        {
            get { return new BitArray(new int[] { this.D0D499[51] }).Cast<bool>().ToArray(); }
        }

        public bool[] Y0Y17
        {
            get { return new BitArray(new int[] { this.D0D499[100] }).Cast<bool>().ToArray(); }
        }
        public bool[] Y20Y37
        {
            get { return new BitArray(new int[] { this.D0D499[101] }).Cast<bool>().ToArray(); }
        }
        public bool[] ServoReadys
        {
            get { return new BitArray(new int[] { this.D0D499[91] }).Cast<bool>().ToArray(); }
        }
        public bool[] AxisStatus
        {
            get { return new BitArray(new int[] { this.D0D499[480] }).Cast<bool>().ToArray(); }
        }

        public bool[] OprAndLimit
        {
            get { return new BitArray(new int[] { this.D0D499[46] }).Cast<bool>().ToArray(); }
        }
        public bool[] RunInstrucTion
        {
            get { return new BitArray(new int[] { this.D0D499[49] }).Cast<bool>().ToArray(); }
        }
        public string[] strConfigData { get; set; } = new string[] { };
        public string[] strModelData { get; set; } = new string[] { };
        public int[] Parameter { get; set; } = new int[] { };
        public int[] ModelData { get; set; } = new int[] { };
        public int[] Function { get; set; } = new int[] { };
        public int[] TimeDelay { get; set; } = new int[] { };

        public int[] DataPointX1 { get; set; } = new int[] { };
        public int[] DataPointX2 { get; set; } = new int[] { };
        public int[] DataPointY { get; set; } = new int[] { };
        public int[] DataPointR { get; set; } = new int[] { };

        public int[] DataSpeedX1 { get; set; } = new int[] { };
        public int[] DataSpeedX2 { get; set; } = new int[] { };
        public int[] DataSpeedY { get; set; } = new int[] { };
        public int[] DataSpeedR { get; set; } = new int[] { };

        public int OffsetX1 { get; set; } = 0;
        public int OffsetX2 { get; set; } = 0;
        public int OffsetY { get; set; } = 0;

        public int enterCoordX1 { get; set; }
        public int enterCoordX2 { get; set; }
        public int enterCoordY { get; set; }
        public int enterCoordR { get; set; }
        public int speedX1 { get; set; }
        public int speedX2 { get; set; }
        public int speedY { get; set; }
        public int speedR { get; set; }
        public int TimeDwell { get; set; }

        public int[] IntToRegister(List<int> values)
        {
            List<int> result = new List<int> { };
            foreach (int value in values)
            {
                int[] result1 = ModbusClient.ConvertIntToRegisters(value, ModbusClient.RegisterOrder.LowHigh);
                result.Add(result1[0]);
                result.Add(result1[1]);
            }
            return result.ToArray();
        }

        public void ReadFileConfig()
        {
            try
            {
                strConfigData = File.ReadAllLines(PathConfig);
                string[] param = new string[strConfigData.Length - 2];
                Array.Copy(strConfigData, 2, param, 0, strConfigData.Length - 2);
                Parameter = param.Select(int.Parse).ToArray();
            }
            catch { }
        }

        public void ReadFileModel()
        {
            try
            {
                strModelData = File.ReadAllLines(PathModel);
                //Function = strModelData[1].Split(',').Select(int.Parse).ToArray();
                DataPointX1 = strModelData[1].Split(',').Select(int.Parse).ToArray();
                DataPointX2 = strModelData[3].Split(',').Select(int.Parse).ToArray();
                DataPointY = strModelData[5].Split(',').Select(int.Parse).ToArray();
                DataPointR = strModelData[7].Split(',').Select(int.Parse).ToArray();

                DataSpeedX1 = strModelData[9].Split(',').Select(int.Parse).ToArray();
                DataSpeedX2 = strModelData[11].Split(',').Select(int.Parse).ToArray();
                DataSpeedY = strModelData[13].Split(',').Select(int.Parse).ToArray();
                DataSpeedR = strModelData[15].Split(',').Select(int.Parse).ToArray();

                OffsetX1 = strModelData[17].Split(',').Select(int.Parse).ToArray()[0];
                OffsetX2 = strModelData[19].Split(',').Select(int.Parse).ToArray()[0];
                OffsetY = strModelData[21].Split(',').Select(int.Parse).ToArray()[0];

                TimeDelay = strModelData[23].Split(',').Select(int.Parse).ToArray();


            }
            catch { }
        }

        public void ReadFileFunction()
        {
            string[] strFc = File.ReadAllLines(PathFunction);
            foreach (string value in strFc)
            {
                try
                {
                    var value1 = int.Parse(value);
                    Function = Function.Append(value1).ToArray();
                }
                catch { }
            }
        }

        public void SaveFileConfig()
        {
            string[] param = Parameter.Select(i => i.ToString()).ToArray();
            Array.Copy(param, 0, strConfigData, 2, param.Length);
            File.WriteAllLines(PathConfig, strConfigData);
        }

        public void SaveFileModel()
        {
            strModelData[1] = string.Join(",", DataPointX1.Select(i => i.ToString()).ToArray());
            strModelData[3] = string.Join(",", DataPointX2.Select(i => i.ToString()).ToArray());
            strModelData[5] = string.Join(",", DataPointY.Select(i => i.ToString()).ToArray());
            globals.strModelData[7] = string.Join(",", globals.DataPointR.Select(i => i.ToString()).ToArray());

            strModelData[9] = string.Join(",", DataSpeedX1.Select(i => i.ToString()).ToArray());
            strModelData[11] = string.Join(",", DataSpeedX2.Select(i => i.ToString()).ToArray());
            strModelData[13] = string.Join(",", DataSpeedY.Select(i => i.ToString()).ToArray());
            strModelData[15] = string.Join(",", DataSpeedR.Select(i => i.ToString()).ToArray());

            strModelData[17] = OffsetX1.ToString();
            strModelData[19] = OffsetX2.ToString();
            strModelData[21] = OffsetY.ToString();

            strModelData[23] = string.Join(",", TimeDelay.Select(i => i.ToString()).ToArray());


            File.WriteAllLines(PathModel, strModelData);


        }
        public void abc()
        {
            string a = string.Join(",", funtionRunRotate.Select(i => i.ToString()).ToArray());
            string[] _a = a.Split(',');
            File.WriteAllLines(@"D:\FPC new version\FPCMotionV2\MotionToolFPC\FPC\bin\Debug\Config\fc1.txt", _a);
        }

        public int[] funtionRunRotate = new int[]
            {

                506, 19010, 19011, 19012, 19013,

                19096, 18834, 18836, 19401, 19069, 18835, 18837,

                500, 4,

                901, 19041, 4, 516, 503, 515, 19040, 19061, 4, 512, 504, 511, 19060,531,
                901, 19041, 4, 516, 503, 515, 19040,531,
                901, 19041, 4, 516, 503, 515, 19040,531,
                901, 19041, 4, 516, 503, 515, 19040,531,
                901, 19041, 4, 516, 503, 515, 19040,531,
                901, 19041, 4, 516, 503, 515, 19040, 19061, 4, 512, 504, 511, 19060,531,

                901, 19061, 4, 512, 501, 511, 19060, 19051, 4, 514, 502, 513, 19050,531,
                901, 19061, 4, 512, 501, 511, 19060, 19051, 4, 514, 502, 513, 19050,531,
                901, 19061, 4, 512, 501, 511, 19060, 19051, 4, 514, 502, 513, 19050,531,

                901, 19061, 4, 512, 504, 511, 19060, 19051, 4, 514, 505, 513, 19050,531,

                901, 19061, 4, 512, 501, 511, 19060, 19051, 4, 514, 502, 513, 19050,531,
                901, 19061, 4, 512, 501, 511, 19060, 19051, 4, 514, 502, 513, 19050,531,
                901, 19061, 4, 512, 501, 511, 19060, 19051, 4, 514, 502, 513, 19050,531,

                901, 19061, 4, 512, 504, 511, 19060, 19051, 4, 514, 505, 513, 19050,531,

                901, 506, 4,

                1030
            };
    }
}
