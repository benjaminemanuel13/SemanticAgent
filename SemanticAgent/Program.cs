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
            Agent.OpenAIKey = "<Place your Api Key Here>";

            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => {
                    services.AddTransient<IAgent, Agent>();
                    services.AddHostedService<Worker>();
                }).Build()
                .Run();
        }
    }
}
