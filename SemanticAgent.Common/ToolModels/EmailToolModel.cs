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

        public EmailToolModelProperties Properties { get; set; } = new EmailToolModelProperties();

        public string[] Required { get; set; } = new string[] { "to", "subject", "body", "from" };
    }
}
