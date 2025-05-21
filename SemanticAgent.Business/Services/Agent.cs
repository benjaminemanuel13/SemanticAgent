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
    public class Agent : IAgent
    {
        public static string OpenAIKey { get; set; }
        private Kernel kernel;

        public Agent()
        {
            kernel = GetKernel();
        }

        public async Task<string> Ask(string question)
        {
            OpenAIPromptExecutionSettings executionSettings = new() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };

            FunctionResult result = await kernel.InvokePromptAsync("Check current UTC time and return current weather in Boston city.", new(executionSettings));

            return result.ToString();
        }

        public Kernel GetKernel()
        {
            kernel = Kernel
                .CreateBuilder()
                .AddOpenAIChatCompletion("gpt-4", OpenAIKey)
                .Build();

            // Import sample plugins.
            kernel.ImportPluginFromType<TimePlugin>();
            kernel.ImportPluginFromType<WeatherPlugin>();

            return kernel;
        }
    }
}
