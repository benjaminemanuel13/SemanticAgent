using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SemanticAgent.Business.Interfaces;
using SemanticAgent.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAgent.Business.Services
{
    public abstract class BaseAgent : IAgent
    {
        public static string OpenAIKey { get; set; } = string.Empty;

        protected Kernel kernel;

        public BaseAgent()
        {
            kernel = GetKernel();
            AddPlugins();
        }

        private Kernel GetKernel()
        {
            kernel = Kernel
                .CreateBuilder()
                .AddOpenAIChatCompletion("gpt-4", OpenAIKey)
                .Build();

            return kernel;
        }

        public abstract Task<string> Ask(string question);

        protected abstract void AddPlugins();
    }
}
