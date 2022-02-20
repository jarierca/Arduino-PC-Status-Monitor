using System;
using System.Threading;
using OpenHardwareMonitor.Hardware;
using System.IO.Ports;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace JariTemp
{
    class PC{
        public CPU cpu;
        public GPU gpu;
        public RAM ram;
        public MOBA moba;
        public override string ToString()
        {
            return "PC: " + cpu.ToString() + "," + gpu.ToString() + "," + ram.ToString() + "," + moba.ToString();
        }
    }
    class CPU
    {
        public String name { get; set; }
        public String load { get; set; }
        public String temperature { get; set; }
        public override string ToString()
        {
            return "CPU: " + name + "," + load + "," + temperature;
        }
    }
    class GPU
    {
        public String name { get; set; }
        public String temperature { get; set; }
        public String memTotal { get; set; }
        public String memUsed { get; set; }
        public override string ToString()
        {
            return "GPU: " + name + "," + temperature + "," + memTotal + "," + memUsed;
        }
    }
    class RAM
    {
        public String memTotal { get; set; }
        public String memUsed { get; set; }
        public override string ToString()
        {
            return "RAM: " + memTotal + "," + memUsed;
        }
    }
    class MOBA
    {
        public String name { get; set; }
        public override string ToString()
        {
            return "MOBA: " + name;
        }
    }
    class Program
    {
        public class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }
            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
            }
            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }

        static PC GetSystemInfo()
        {
            UpdateVisitor updateVisitor = new UpdateVisitor();
            Computer computer = new Computer();
            computer.Open();
            computer.CPUEnabled = true;
            computer.FanControllerEnabled = true;
            computer.GPUEnabled = true;
            computer.HDDEnabled = true;
            computer.MainboardEnabled = true;
            computer.RAMEnabled = true;

            computer.Accept(updateVisitor);

            PC pc = new PC();
            for (int i = 0; i < computer.Hardware.Length; i++)
            {
                //CPU
                if (computer.Hardware[i].HardwareType == HardwareType.CPU)
                {
                    CPU cpu = new CPU();
                    cpu.name = computer.Hardware[i].Name;
                    //Console.WriteLine("[+] CPU -- " + computer.Hardware[i].Name + " --\r");

                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {

                        if (computer.Hardware[i].Sensors[j].Name.Equals("CPU Total"))
                        {
                            cpu.load = computer.Hardware[i].Sensors[j].Value.ToString();
                            //Console.WriteLine("   [-]" + computer.Hardware[i].Sensors[j].Name + ": " + computer.Hardware[i].Sensors[j].Value.ToString() + "ºC\r");
                        }

                        if (computer.Hardware[i].Sensors[j].Name.Equals("CPU Package"))
                        {
                            cpu.temperature = computer.Hardware[i].Sensors[j].Value.ToString();
                            //Console.WriteLine("   [-]" + computer.Hardware[i].Sensors[j].Name + ": " + computer.Hardware[i].Sensors[j].Value.ToString() + "% \r");
                        }
                    }
                    pc.cpu = cpu;
                }
                //GPU NVIDIA && GPU ATI / AMD 
                if (computer.Hardware[i].HardwareType == HardwareType.GpuNvidia || computer.Hardware[i].HardwareType == HardwareType.GpuAti)
                {
                    GPU gpu = new GPU();
                    gpu.name = computer.Hardware[i].Name;
                    //Console.WriteLine("[+] GPU -- " + computer.Hardware[i].Name + " --\r");

                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        //GPU Temperature
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                        {
                            gpu.temperature = computer.Hardware[i].Sensors[j].Value.ToString();
                            //Console.WriteLine("   [-] " + computer.Hardware[i].Sensors[j].Name + ": " + computer.Hardware[i].Sensors[j].Value.ToString() + "ºC \r");
                        }
                        //GPU Memory Total
                        if (computer.Hardware[i].Sensors[j].Name.Equals("GPU Memory Total")) {
                            gpu.memTotal = computer.Hardware[i].Sensors[j].Value.ToString();
                            //Console.WriteLine("   [-] " + computer.Hardware[i].Sensors[j].Name + ": " + computer.Hardware[i].Sensors[j].Value.ToString() + " MB \r");
                        }
                        //GPU Memory Used
                        if (computer.Hardware[i].Sensors[j].Name.Equals("GPU Memory Used"))
                        {
                            gpu.memUsed = computer.Hardware[i].Sensors[j].Value.ToString();
                            //Console.WriteLine("   [-] " + computer.Hardware[i].Sensors[j].Name + ": " + computer.Hardware[i].Sensors[j].Value.ToString() + " MB \r");
                        }
                    }
                    pc.gpu = gpu;
                }

                //RAM
                if (computer.Hardware[i].HardwareType == HardwareType.RAM)
                {
                    //Console.WriteLine("[+] RAM -- " + computer.Hardware[i].Name + " --\r");
                    RAM ram = new RAM();
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {

                        if (computer.Hardware[i].Sensors[j].Name.Equals("Used Memory"))
                        {
                            ram.memUsed = computer.Hardware[i].Sensors[j].Value.ToString();
                            //Console.WriteLine("   [-] " + computer.Hardware[i].Sensors[j].Name + ": " + computer.Hardware[i].Sensors[j].Value.ToString() + "MB \r");
                        }

                        if (computer.Hardware[i].Sensors[j].Name.Equals("Memory"))
                        {
                            ram.memTotal = computer.Hardware[i].Sensors[j].Value.ToString();
                            //Console.WriteLine("   [-] " + computer.Hardware[i].Sensors[j].Name + ": " + computer.Hardware[i].Sensors[j].Value.ToString() + "% \r");
                        }
                    }
                    pc.ram = ram;
                }

                //MOBA
                if (computer.Hardware[i].HardwareType == HardwareType.Mainboard)
                {
                    MOBA moba = new MOBA();
                    moba.name = computer.Hardware[i].Name;
                    pc.moba = moba;
                    //Console.WriteLine("[+]  -- " + computer.Hardware[i].Name + " --\r");
                }
            }
            //Console.WriteLine("X=-------------={END}=--------------=X");
            Thread.Sleep(2000);
            computer.Close();
            return pc;
        }
        static String GetCpuTemp()
        {
            String Cputemp = "";
            UpdateVisitor updateVisitor = new UpdateVisitor();
            Computer computer = new Computer();
            computer.Open();
            computer.CPUEnabled = true;
            computer.FanControllerEnabled = true;
            computer.GPUEnabled = true;
            computer.HDDEnabled = true;
            computer.MainboardEnabled = true;
            computer.RAMEnabled = true;

            computer.Accept(updateVisitor);

            for (int i = 0; i < computer.Hardware.Length; i++)
            {

                //CPU
                if (computer.Hardware[i].HardwareType == HardwareType.CPU)
                {

                    Console.WriteLine("[+] CPU -- " + computer.Hardware[i].Name + " --\r");

                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {

                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature) {
                            Cputemp = computer.Hardware[i].Sensors[j].Value.ToString();
                            Console.WriteLine(computer.Hardware[i].Sensors[j].Value.ToString());
                        }
                    }
                }
            }
            computer.Close();
            return Cputemp;
        }

        static void Main(string[] args)
        {
            SerialPort port = new SerialPort("COM6", 9600);
            port.DataBits = 8;
            port.DtrEnable = true;
            port.RtsEnable = true;
            port.StopBits = StopBits.One;
            port.Handshake = Handshake.None;
            port.Parity = Parity.None;
            port.Open();
            while (true)
            {
                PC pc = GetSystemInfo();
                port.Write(pc.cpu.temperature.ToString() + ";" + pc.gpu.temperature.ToString() + ";" + pc.ram.memUsed.ToString());
                Console.WriteLine(pc.cpu.temperature.ToString() + " ºC; " + pc.gpu.temperature.ToString() + " ºC;" + pc.ram.memUsed.ToString() + " Mb");
                //port.Write(Newtonsoft.Json.JsonConvert.SerializeObject(pc));
                //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(pc));
            }
            port.Close();
        }
    }
}
