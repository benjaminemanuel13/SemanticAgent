using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using SemanticAgent.Business.Interfaces;
using SemanticAgent.Business.Services;

namespace SemanticAgent
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BaseAgent.OpenAIKey = Environment.GetEnvironmentVariable("OPENAIKEY");

            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => {
                    services.AddKeyedTransient<TimeAndWeatherAgent>("TimeAndWeather");
                    services.AddHostedService<Worker>();
                }).Build()
                .Run();
        }
    }
}
