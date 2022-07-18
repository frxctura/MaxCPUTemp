using System;
using OpenHardwareMonitor.Hardware;
using System.Diagnostics;
namespace Get_CPU_Temp
{
    class AutoShutDown
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
        private static int enteredTemperature = int.MaxValue;
        static void EnterTemp()
        {
            Console.WriteLine("Enter the temperature on which you want the system to automatically shut down: ");
            enteredTemperature = int.Parse(Console.ReadLine());
        }
        private static int enteredTime = int.MaxValue;
        static void EnterTime()
        {
            Console.WriteLine("Enter the time in which your system will shut down: ");
            enteredTime = int.Parse(Console.ReadLine());
        }
        static void GrabInfo()
        {
            UpdateVisitor update = new UpdateVisitor();
            Computer computer = new Computer();
            computer.Open();
            computer.CPUEnabled = true;
            computer.Accept(update);
            for (int i = 0; i < computer.Hardware.Length; i++)
            {
                if (computer.Hardware[i].HardwareType == HardwareType.CPU)
                {
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                        {
                            Console.WriteLine(computer.Hardware[i].Sensors[j].Value.ToString() + "\r");
                            if (computer.Hardware[i].Sensors[j].Value > enteredTemperature)
                            {
                                ShutDown();
                            }
                        }
                    }
                }
            }
        }
        static void Main(string[] args)
        {
            EnterTemp();
            EnterTime();
            while (true)
            {
                GrabInfo();
            }
        }
        static void ShutDown()
        {
            Process.Start("shutdown", $"/s /t {enteredTime}");
            Console.WriteLine("Too high of a temperature, shutting down");
            System.Environment.Exit(0);
        }
    }
}