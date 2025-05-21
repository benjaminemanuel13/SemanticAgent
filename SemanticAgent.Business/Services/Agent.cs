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
        public static string OpenAIKey { get; set; } = string.Empty;

        private Kernel kernel;

        public Agent()
        {
            kernel = GetKernel();
        }

        public async Task<string> Ask(string question = "Check current UTC time and return current weather in Boston city.")
        {
            OpenAIPromptExecutionSettings executionSettings = new() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };

            FunctionResult result = await kernel.InvokePromptAsync(question, new(executionSettings));

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
