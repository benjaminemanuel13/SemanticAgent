using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SemanticAgent.Agents.Agents.MultiAgent;
using SemanticAgent.Agents.Agents.SingleAgent;
using SemanticAgent.Agents.FunctionCall;
using SemanticAgent.Agents.ToolDefinitions;
using SemanticAgent.Business.Interfaces;
using SemanticAgent.Business.Services.MultiAgent;
using SemanticAgent.Common.ToolModels;
using SemanticAgent.Workers;
using Smile.Assistant.AzureOpenAI.V2.Services;
using System;
using System.Net;

namespace SemanticAgent
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BaseAgent.OpenAIKey = Environment.GetEnvironmentVariable("OPENAIKEY");

            AzureOpenAIService.Endpoint = Environment.GetEnvironmentVariable("AZUREOPENAIENDPOINT");
            AzureOpenAIService.Key = Environment.GetEnvironmentVariable("AZUREOPENAIKEY");

            AzureOpenAIService.Model = "gpt-4o-smile";

            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                })
                .ConfigureServices(services => {
                    services.AddKeyedTransient<IAgent, TimeAndWeatherAgent>("TimeAndWeather");
                    services.AddKeyedTransient<IAgent, HumanResourcesAgent>("HumanResources");

                    services.AddTransient<DynamicAgent>();
                    services.AddTransient<AzureOpenAIService>();

                    services.AddTransient<EmailAgent>();
                    services.AddTransient<EmailToolDefinition>();

                    // Uncomment the worker you want to run...

                    //services.AddHostedService<Worker>();
                    //services.AddHostedService<AgentWorker>();
                    services.AddHostedService<DynamicAgentWorker>();
                }).Build()
                .Run();
        }
    }
}
