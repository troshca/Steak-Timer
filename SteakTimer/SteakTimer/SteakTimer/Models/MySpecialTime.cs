using System;
using System.Collections.Generic;
using System.Text;

namespace SteakTimer.Models
{
    public class MySpecialTime
    {
        public string Name { get; set; }
        public int Value { get; set; }

        public MySpecialTime(string name, int value) 
        { 
            Name = name;
            Value = value;
        }

        public override string ToString()
        { 
            return Name; 
        }
    }
}
