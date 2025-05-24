using Microsoft.Extensions.Hosting;
using SemanticAgent.Agents.Agents;
using SemanticAgent.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAgent
{
    public class AgentWorker : BackgroundService
    {
        private readonly EmailAgent email;

        public AgentWorker(EmailAgent email)
        { 
            this.email = email;
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
                    email.Run(input);
                    //var res = await agent.Ask(input);

                    //Console.WriteLine(res + "\r\n");
                }
            }
        }
    }
}
