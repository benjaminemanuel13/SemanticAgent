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
                    services.AddKeyedTransient<TimeAndWeatherAgent>("TimeAndWeather");
                    services.AddHostedService<Worker>();
                }).Build()
                .Run();
        }
    }
}
