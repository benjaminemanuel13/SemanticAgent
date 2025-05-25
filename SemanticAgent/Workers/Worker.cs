using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SemanticAgent.Business.Interfaces;
using SemanticAgent.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAgent.Workers
{
    public class Worker : BackgroundService
    {
        private readonly IAgent agent;

        public Worker([FromKeyedServices("HumanResources")] IAgent agent)
        {
            this.agent = agent;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var running = true;

            while (running) {
                Console.WriteLine("What can I do for you?");
                Console.Write(">");

                var input = Console.ReadLine();

                if (input == "exit" || string.IsNullOrEmpty(input))
                {
                    running = false;
                    break;
                }
                else
                {
                    var res = await agent.Ask(input);
                    Console.WriteLine(res + "\r\n");
                }
            }
        }
    }
}
