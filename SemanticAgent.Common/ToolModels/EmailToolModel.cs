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

        public EmailToolModelProperties Properties = new EmailToolModelProperties()
        {
            to = new ToolProperty() { Description = "This is who the email is being sent to." },
            subject = new ToolProperty() { Description = "This is the subject of the email." },
            body = new ToolProperty() { Description = "This is the body of the email." },
            from = new ToolProperty() { Description = "This is who the email is from." }
        };

        public string[] Required { get; set; } = new string[] { "to", "subject", "body", "from" };
    }
}
