using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionToolFPC
{
    public class Parameter
    {
        public string Name { get; set; }
        public int HomeSpeed { get; set; }
        public int CreepSpeed { get; set; }
        public int JogContinuosSpeed { get; set; }
        public int JogIncreSpeed { get; set; }
        public int JogIncreSize { get; set; }
        public int StratPoint { get; set; }
        public int StartPointSpeed { get; set; }
        public Parameter(string name, int homeSpeed, int creepSpeed, int jogContinuosSpeed, int jogIncreSpeed,  int jogIncreSize, int stratPoint, int startPointSpeed)
        {
            Name = name;
            HomeSpeed = homeSpeed;
            CreepSpeed = creepSpeed;
            JogContinuosSpeed = jogContinuosSpeed;
            JogIncreSpeed = jogIncreSpeed;
            JogIncreSize = jogIncreSize;
            StratPoint = stratPoint;
            StartPointSpeed = startPointSpeed;
        }
    }
    public class DataPoint
    {
        
        public int ID { get; set; }
        public int Coordinate_X1 { get; set; }
        public int Coordinate_X2 { get; set; }
        public int Coordinate_Y { get; set; }
        public int Coordinate_R { get; set; }
        public int Speed_X1 { get; set; }
        public int Speed_X2 { get; set; }
        public int Speed_Y { get; set; }
        public int Speed_R { get; set; }
        public DataPoint(int iD, int coordinate_X1, int coordinate_X2, int coordinate_Y, int coordinate_R, int speed_X1, int speed_X2, int speed_Y, int speed_R)
        {
            ID = iD;
            Coordinate_X1 = coordinate_X1;
            Coordinate_X2 = coordinate_X2;
            Coordinate_Y = coordinate_Y;
            Coordinate_R = coordinate_R;
            Speed_X1 = speed_X1;
            Speed_X2 = speed_X2;
            Speed_Y = speed_Y;
            Speed_R = speed_R;
        }
    }
    public class Progress
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Progress(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }
    public enum JogMode
    {
        Incremental,
        Continuous
    }
    public enum OnOff
    {
        On,
        Off
    }
}
