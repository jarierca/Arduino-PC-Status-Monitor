using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Monitor_GUI
{
    public class PC
    {
        public CPU cpu;
        public GPU gpu;
        public RAM ram;
        /*public MOBA moba;*/
        public override string ToString()
        {
            return "PC: \n" + cpu.ToString() + "\n" + gpu.ToString() + "\n" + ram.ToString()/* + "," + moba.ToString()*/;
        }
    }
    public class CPU
    {
        // public String name { get; set; }
        public String load { get; set; }
        public String temp { get; set; }
        public override string ToString()
        {
            return "CPU: "/* + name + "," */+ load + "," + temp;
        }
    }
    public class GPU
    {
        // public String name { get; set; }
        public String clock { get; set; }
        public String load { get; set; }
        public String temp { get; set; }
        public String memTotal { get; set; }
        public String memUsed { get; set; }
        public override string ToString()
        {
            return "GPU: " +/* name +*/ "," + temp + "," + memTotal + "," + memUsed;
        }
    }
    public class RAM
    {
        public String memTotal { get; set; }
        public String memUsed { get; set; }
        public override string ToString()
        {
            return "RAM: " + memTotal + "," + memUsed;
        }
    }
    public class MOBA
    {
        public String name { get; set; }
        public override string ToString()
        {
            return "MOBA: " + name;
        }
    }
}
