using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SemanticAgent.Agents.Agents.SingleAgent;
using SemanticAgent.Agents.ToolDefinitions;
using SemanticAgent.Business.Interfaces;
using SemanticAgent.Business.Services;
using SemanticAgent.Business.Services.MultiAgent;
using SemanticAgent.Common.ToolModels;
using SemanticAgent.Workers;
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

                    services.AddTransient<EmailAgent>();
                    services.AddTransient<EmailToolDefinition>();

                    //// Doesn't work - services.AddTransient<MultiAgentService>();

                    services.AddHostedService<Worker>();
                    //services.AddHostedService<AgentWorker>();

                    //// Doesn't work - services.AddHostedService<MultiAgentWorker>();
                }).Build()
                .Run();
        }
    }
}
