using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SemanticAgent.Business.Interfaces;
using SemanticAgent.Business.Services;
using System;

namespace SemanticAgent
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BaseAgent.OpenAIKey = Environment.GetEnvironmentVariable("OPENAIKEY");

            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                })
                .ConfigureServices(services => {
                    services.AddKeyedTransient<IAgent, TimeAndWeatherAgent>("TimeAndWeather");
                    services.AddKeyedTransient<IAgent, HumanResourcesAgent>("HumanResources");

                    services.AddHostedService<Worker>();
                }).Build()
                .Run();
        }
    }
}
