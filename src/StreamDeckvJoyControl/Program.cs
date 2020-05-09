using BarRaider.SdTools;

namespace VJoyControl
{
    class Program
    {
        static void Main(string[] args)
        {
            //while (!System.Diagnostics.Debugger.IsAttached) { System.Threading.Thread.Sleep(100); }
            SDWrapper.Run(args);
        }
    }
}