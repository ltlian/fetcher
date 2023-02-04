using Microsoft.Extensions.Hosting;

namespace Fetcher
{
    internal class Program
    {
        private static void Main(string[] _)
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .Build();

            host.Run();
        }
    }
}