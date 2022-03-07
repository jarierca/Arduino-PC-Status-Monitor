using OpenHardwareMonitor.Hardware;

namespace PC_Monitor_GUI
{
    internal class SysInfo
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

        public static PC GetSystemInfo()
        {
            PC pc = new PC();
            try
            {
                UpdateVisitor updateVisitor = new UpdateVisitor();
                Computer computer = new Computer();
                computer.CPUEnabled = true;
                computer.FanControllerEnabled = true;
                computer.GPUEnabled = true;
                computer.HDDEnabled = true;
                computer.MainboardEnabled = true;
                computer.RAMEnabled = true;
                computer.Open();

                computer.Accept(updateVisitor);


                for (int i = 0; i < computer.Hardware.Length; i++)
                {
                    //CPU
                    if (computer.Hardware[i].HardwareType == HardwareType.CPU)
                    {
                        CPU cpu = new CPU();
                        //cpu.name = computer.Hardware[i].Name;

                        for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                        {

                            if (computer.Hardware[i].Sensors[j].Name.Equals("CPU Total"))
                            {
                                cpu.load = Math.Round(Double.Parse(computer.Hardware[i].Sensors[j].Value.ToString()), 2) + "%";
                            }

                            if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                            {
                                cpu.temp = computer.Hardware[i].Sensors[j].Value.ToString() + " C";
                            }
                        }
                        pc.cpu = cpu;
                    }

                    //GPU NVIDIA && GPU ATI / AMD 
                    if (computer.Hardware[i].HardwareType == HardwareType.GpuNvidia || computer.Hardware[i].HardwareType == HardwareType.GpuAti)
                    {
                        GPU gpu = new GPU();
                        //gpu.name = computer.Hardware[i].Name;

                        for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                        {
                            //GPU Temperature
                            if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                            {
                                gpu.temp = computer.Hardware[i].Sensors[j].Value.ToString() + " C";
                            }
                            //GPU Core
                            if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Clock)
                            {
                                gpu.clock = computer.Hardware[i].Sensors[j].Value.ToString() + " Mhz";
                            }
                            if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Load)
                            {
                                gpu.load = Math.Round(Double.Parse(computer.Hardware[i].Sensors[j].Value.ToString()), 2) + "%";
                            }
                            //GPU Memory Total
                            if (computer.Hardware[i].Sensors[j].Name.Equals("GPU Memory Total"))
                            {
                                gpu.memTotal = Math.Round(Double.Parse(computer.Hardware[i].Sensors[j].Value.ToString())).ToString() + " MB";
                            }
                            //GPU Memory Used
                            if (computer.Hardware[i].Sensors[j].Name.Equals("GPU Memory Used"))
                            {
                                gpu.memUsed = Math.Round(Double.Parse(computer.Hardware[i].Sensors[j].Value.ToString())).ToString();
                            }
                        }
                        pc.gpu = gpu;
                    }

                    //RAM
                    if (computer.Hardware[i].HardwareType == HardwareType.RAM)
                    {
                        RAM ram = new RAM();
                        for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                        {

                            if (computer.Hardware[i].Sensors[j].Name.Equals("Used Memory"))
                            {
                                ram.memUsed = Math.Round(Double.Parse(computer.Hardware[i].Sensors[j].Value.ToString()), 2) + " GB";
                            }

                            if (computer.Hardware[i].Sensors[j].Name.Equals("Memory"))
                            {
                                ram.memTotal = Math.Round(Double.Parse(computer.Hardware[i].Sensors[j].Value.ToString()), 2) + "%";
                            }
                        }
                        pc.ram = ram;
                    }
                    /*
                    //MOBA
                    if (computer.Hardware[i].HardwareType == HardwareType.Mainboard)
                    {
                        MOBA moba = new MOBA();
                        moba.name = computer.Hardware[i].Name;
                        pc.moba = moba;
                    }*/
                }
            }
            catch (Exception ex)
            {
                //computer.Close();
            }
            return pc;
        }
    }
}
