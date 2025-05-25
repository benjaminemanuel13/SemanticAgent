using Microsoft.Extensions.Hosting;
using SemanticAgent.Agents.Agents.MultiAgent;
using SemanticAgent.Agents.Agents.SingleAgent;
using SemanticAgent.Business.Services.MultiAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAgent.Workers
{
    public class DynamicAgentWorker : BackgroundService
    {
        private readonly DynamicAgent agent;

        public DynamicAgentWorker(DynamicAgent agent)
        {
            this.agent = agent;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var running = true;

            while (running)
            {
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
                    var result = await agent.Ask(input);
                }
            }
        }
    }
}
