using Azure.AI.Agents.Persistent;
using SemanticAgent.Common.ToolModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SemanticAgent.Agents.ToolDefinitions
{
    public class EmailToolDefinition : FunctionToolDefinition
    {
        public EmailToolDefinition(EmailToolModel model) : base("EmailTool", "Send an email to a recipient", 
            parameters: BinaryData.FromObjectAsJson(model, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }))
        {

        }
    }
}
