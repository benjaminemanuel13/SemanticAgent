using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAgent.Common.ToolModels
{
    public class EmailToolModel
    {
        public string Type { get; set; } = "object";

        public EmailToolModelProperties Properties { get; set; } = new EmailToolModelProperties()
        {
            to = new ToolProperty() { Description = "This is who the email is being sent to.", Type = "string" },
            subject = new ToolProperty() { Description = "This is the subject of the email.", Type = "string" },
            body = new ToolProperty() { Description = "This is the body of the email.", Type = "string" },
            from = new ToolProperty() { Description = "This is who the email is from.", Type = "string" }
        };

        public string[] Required { get; set; } = new string[] { "to", "subject", "body", "from" };
    }
}
