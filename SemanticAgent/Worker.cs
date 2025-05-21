using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SemanticAgent.Business.Interfaces;
using SemanticAgent.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAgent
{
    public class Worker : BackgroundService
    {
        private readonly IAgent agent;

        public Worker([FromKeyedServices("TimeAndWeather")] TimeAndWeatherAgent agent)
        {
            this.agent = agent;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var running = true;

            while (running) {
                Console.WriteLine("What would you like to do?");
                Console.Write(">");

                var input = Console.ReadLine();

                if (input == "exit" || string.IsNullOrEmpty(input))
                {
                    running = false;
                    break;
                }

                var res = await agent.Ask(input);
                Console.WriteLine(res);
            }
        }
    }
}
