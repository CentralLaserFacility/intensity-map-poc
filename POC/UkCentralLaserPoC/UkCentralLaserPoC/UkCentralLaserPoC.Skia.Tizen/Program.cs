using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace UkCentralLaserPoC.Skia.Tizen
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new TizenHost(() => new UkCentralLaserPoC.App(), args);
            host.Run();
        }
    }
}
