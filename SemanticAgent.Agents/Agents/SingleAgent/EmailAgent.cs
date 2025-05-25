using Azure;
using Azure.AI.Agents.Persistent;
using Azure.Identity;
using SemanticAgent.Agents.ToolDefinitions;
using SemanticAgent.Common.ToolModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SemanticAgent.Agents.Agents.SingleAgent
{
    public class EmailAgent
    {
        public string SendEmail(string To, string Subject, string Body, string From)
        {
            //Send Email logic here
            string msg = $"Email sent to {To} with subject '{Subject}' from {From}.";

            Console.WriteLine(msg);
            return msg;
        }

        public void Run(string message)
        {
            var projectEndpoint = "https://aiservicesspa3.services.ai.azure.com/api/projects/projectspa3";
            var modelDeploymentName = "gpt-4o";

            PersistentAgentsClient client = new(projectEndpoint, new DefaultAzureCredential());

            EmailToolModel model = new EmailToolModel();

            EmailToolDefinition emailTool = new EmailToolDefinition(model);

            PersistentAgent agent = client.Administration.CreateAgent(
                model: modelDeploymentName,
                name: "SDK Test Agent - Functions",
                instructions: "You are an email sender. Use the provided functions to help send the email.",    
                tools: [emailTool]);

            PersistentAgentThread thread = client.Threads.CreateThread();

            client.Messages.CreateMessage(
                thread.Id,
                MessageRole.User,
                message);

            ThreadRun run = client.Runs.CreateRun(thread.Id, agent.Id);

            do
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(500));

                run = client.Runs.GetRun(thread.Id, run.Id);

                if (run.Status == RunStatus.RequiresAction
                    && run.RequiredAction is SubmitToolOutputsAction submitToolOutputsAction)
                {
                    List<ToolOutput> toolOutputs = [];
                 
                    foreach (RequiredToolCall toolCall in submitToolOutputsAction.ToolCalls)
                    {
                        toolOutputs.Add(GetResolvedToolOutput(toolCall));
                    }
                    
                    run = client.Runs.SubmitToolOutputsToRun(run, toolOutputs);
                }
            }
            
            while (run.Status == RunStatus.Queued
                || run.Status == RunStatus.InProgress
                || run.Status == RunStatus.RequiresAction);

            Pageable<PersistentThreadMessage> messages = client.Messages.GetMessages(
                threadId: thread.Id,
                order: ListSortOrder.Ascending
            );

            foreach (PersistentThreadMessage threadMessage in messages)
            {
                foreach (MessageContent content in threadMessage.ContentItems)
                {
                    switch (content)
                    {
                        case MessageTextContent textItem:
                            Console.WriteLine($"[{threadMessage.Role}]: {textItem.Text}");
                            break;
                    }
                }
            }

            client.Threads.DeleteThread(threadId: thread.Id);
            client.Administration.DeleteAgent(agentId: agent.Id);
        }

        ToolOutput GetResolvedToolOutput(RequiredToolCall toolCall)
        {
            if (toolCall is RequiredFunctionToolCall functionToolCall)
            {
                using JsonDocument argumentsJson = JsonDocument.Parse(functionToolCall.Arguments);

                if (functionToolCall.Name == "EmailTool")
                {
                    string to = argumentsJson.RootElement.GetProperty("to").GetString();
                    string subject = argumentsJson.RootElement.GetProperty("subject").GetString();
                    string body = argumentsJson.RootElement.GetProperty("body").GetString();
                    string from = argumentsJson.RootElement.GetProperty("from").GetString();

                    return new ToolOutput(toolCall, SendEmail(to, subject, body, from));
                }
            }

            return null;
        }
    }
}
