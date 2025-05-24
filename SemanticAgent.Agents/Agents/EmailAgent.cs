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

namespace SemanticAgent.Agents.Agents
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

            EmailToolModel model = new EmailToolModel() { 
                Type = "object",
                Properties = new EmailToolModelProperties() { 
                    to = new ToolProperty() { Description = "This is who the email is being sent to." },
                    subject = new ToolProperty() { Description = "This is the subject of the email." },
                    body = new ToolProperty() { Description = "This is the body of the email." },
                    from = new ToolProperty() { Description = "This is who the email is from." }
                }
            };

            EmailToolDefinition emailTool = new EmailToolDefinition(model);

            // Create the agent instance
            PersistentAgent agent = client.Administration.CreateAgent(
                model: modelDeploymentName,
                name: "SDK Test Agent - Functions",
                instructions: "You are an email sender. Use the provided functions to help send the email.",    
                tools: [emailTool]);

            // Create a new conversation thread for the agent
            PersistentAgentThread thread = client.Threads.CreateThread();

            // Add the initial user message to the thread
            client.Messages.CreateMessage(
                thread.Id,
                MessageRole.User,
                message);

            // Start a run for the agent to process the messages in the thread
            ThreadRun run = client.Runs.CreateRun(thread.Id, agent.Id);

            // Loop to check the run status and handle required actions
            do
            {
                // Wait briefly before checking the status again
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
                // Get the latest status of the run
                run = client.Runs.GetRun(thread.Id, run.Id);

                // Check if the agent requires a function call to proceed
                if (run.Status == RunStatus.RequiresAction
                    && run.RequiredAction is SubmitToolOutputsAction submitToolOutputsAction)
                {
                    // Prepare a list to hold the outputs of the tool calls
                    List<ToolOutput> toolOutputs = [];
                    // Iterate through each required tool call
                    foreach (RequiredToolCall toolCall in submitToolOutputsAction.ToolCalls)
                    {
                        // Execute the function and get the output using the helper method
                        toolOutputs.Add(GetResolvedToolOutput(toolCall));
                    }
                    // Submit the collected tool outputs back to the run
                    run = client.Runs.SubmitToolOutputsToRun(run, toolOutputs);
                }
            }
            // Continue looping while the run is in progress or requires action
            while (run.Status == RunStatus.Queued
                || run.Status == RunStatus.InProgress
                || run.Status == RunStatus.RequiresAction);

            Pageable<PersistentThreadMessage> messages = client.Messages.GetMessages(
                threadId: thread.Id,
                order: ListSortOrder.Ascending
            );

            // Iterate through each message in the thread
            foreach (PersistentThreadMessage threadMessage in messages)
            {
                // Iterate through content items in the message (usually just one text item)
                foreach (MessageContent content in threadMessage.ContentItems)
                {
                    // Process based on content type
                    switch (content)
                    {
                        // If it's a text message
                        case MessageTextContent textItem:
                            // Print the role (user/agent) and the text content
                            Console.WriteLine($"[{threadMessage.Role}]: {textItem.Text}");
                            break;
                            // Add handling for other content types if necessary (e.g., images)
                    }
                }
            }

            // Delete the conversation thread
            client.Threads.DeleteThread(threadId: thread.Id);
            // Delete the agent definition
            client.Administration.DeleteAgent(agentId: agent.Id);
        }

        // Helper function to execute the correct local C# function based on the tool call request from the agent
        ToolOutput GetResolvedToolOutput(RequiredToolCall toolCall)
        {
            // Check if the required call is a function call
            if (toolCall is RequiredFunctionToolCall functionToolCall)
            {
                using JsonDocument argumentsJson = JsonDocument.Parse(functionToolCall.Arguments);

                // Execute GetUserFavoriteCity if its name matches
                if (functionToolCall.Name == "EmailTool")
                {
                    string to = argumentsJson.RootElement.GetProperty("to").GetString();
                    string subject = argumentsJson.RootElement.GetProperty("subject").GetString();
                    string body = argumentsJson.RootElement.GetProperty("body").GetString();
                    string from = argumentsJson.RootElement.GetProperty("from").GetString();

                    return new ToolOutput(toolCall, SendEmail(to, subject, body, from));
                }
            }
            // Return null if the tool call type isn't handled
            return null;
        }
    }
}
