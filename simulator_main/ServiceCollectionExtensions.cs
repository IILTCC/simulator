using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using simulator_libary;
using simulator_main.services;

namespace simulator_main
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSingleTones(this IServiceCollection services, IConfigurationRoot configFile)
        {
            SimulatorSettings simulatorSettings = configFile.GetRequiredSection(nameof(SimulatorSettings)).Get<SimulatorSettings>();

            SocketConnection socketConnection = new SocketConnection(simulatorSettings);
            services.AddSingleton(socketConnection);
            services.AddSingleton<IBitstreamService, BitstreamService>();
            socketConnection.Connect();

            return services;
        }
    }
}
