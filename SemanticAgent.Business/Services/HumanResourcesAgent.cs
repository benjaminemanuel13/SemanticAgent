using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SemanticAgent.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAgent.Business.Services
{
    public class HumanResourcesAgent : BaseAgent
    {
        public HumanResourcesAgent() : base()
        {
        }
        public override async Task<string> Ask(string question)
        {
            OpenAIPromptExecutionSettings executionSettings = new() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };

            FunctionResult result = await kernel.InvokePromptAsync(question, new(executionSettings));

            return result.ToString();
        }
        protected override void AddPlugins()
        {
            kernel.ImportPluginFromType<EmailPlugin>();
            kernel.ImportPluginFromType<StaffLookupPlugin>();
        }
    }
}
