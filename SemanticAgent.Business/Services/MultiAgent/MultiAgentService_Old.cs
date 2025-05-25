using Azure;
using Azure.AI.Agents.Persistent;
using Azure.Identity;
using SemanticAgent.Agents.ToolDefinitions;
using SemanticAgent.Business.Interfaces;
using SemanticAgent.Common.ToolModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAgent.Business.Services.MultiAgent
{
    public class MultiAgentService_Old
    {
        private PersistentAgent leadAgent;
        private PersistentAgent emailAgent;

        private string projectEndpoint = "https://aiservicesspa3.services.ai.azure.com/api/projects/projectspa3";
        private string modelDeploymentName = "gpt-4o";

        PersistentAgentsClient client;

        public void Run(string input)
        {
            client = new(projectEndpoint, new DefaultAzureCredential());

            EmailToolModel model = new EmailToolModel();
            EmailToolDefinition emailTool = new EmailToolDefinition(model);

            emailAgent = client.Administration.CreateAgent(
                model: modelDeploymentName,
                name: "email_bot",
                instructions: "Your job is to get send emails",
                tools: [emailTool]
            );

            ConnectedAgentToolDefinition connectedAgentDefinition = new(new ConnectedAgentDetails(emailAgent.Id, emailAgent.Name, "Send an email"));

            leadAgent = client.Administration.CreateAgent(
                model: modelDeploymentName,
                name: "lead_bot",
                instructions: "Your job is to do various tasks.",
                tools: [connectedAgentDefinition]
            );

            PersistentAgentThread thread = client.Threads.CreateThread();

            PersistentThreadMessage message = client.Messages.CreateMessage(
                thread.Id,
                MessageRole.User,
                "Send an email from Jane Smith to Ben Emanuel, subject \"Test\" body: \"Test Body\"");

            // Run the agent
            ThreadRun run = client.Runs.CreateRun(thread, leadAgent);
            do
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
                run = client.Runs.GetRun(thread.Id, run.Id);
            }
            while (run.Status == RunStatus.Queued
                || run.Status == RunStatus.InProgress);

            // Confirm that the run completed successfully
            if (run.Status != RunStatus.Completed)
            {
                throw new Exception("Run did not complete successfully, error: " + run.LastError?.Message);
            }

            Pageable<PersistentThreadMessage> messages = client.Messages.GetMessages(
                threadId: thread.Id,
                order: ListSortOrder.Ascending
            );

            foreach (PersistentThreadMessage threadMessage in messages)
            {
                Console.Write($"{threadMessage.CreatedAt:yyyy-MM-dd HH:mm:ss} - {threadMessage.Role,10}: ");
                foreach (MessageContent contentItem in threadMessage.ContentItems)
                {
                    if (contentItem is MessageTextContent textItem)
                    {
                        string response = textItem.Text;
                        if (textItem.Annotations != null)
                        {
                            foreach (MessageTextAnnotation annotation in textItem.Annotations)
                            {
                                if (annotation is MessageTextUriCitationAnnotation urlAnnotation)
                                {
                                    response = response.Replace(urlAnnotation.Text, $" [{urlAnnotation.UriCitation.Title}]({urlAnnotation.UriCitation.Uri})");
                                }
                            }
                        }
                        Console.Write($"Agent response: {response}");
                    }
                    else if (contentItem is MessageImageFileContent imageFileItem)
                    {
                        Console.Write($"<image from ID: {imageFileItem.FileId}");
                    }
                    Console.WriteLine();
                }
            }

            client.Threads.DeleteThread(threadId: thread.Id);
            client.Administration.DeleteAgent(agentId: leadAgent.Id);
            client.Administration.DeleteAgent(agentId: emailAgent.Id);
        }
    }
}
