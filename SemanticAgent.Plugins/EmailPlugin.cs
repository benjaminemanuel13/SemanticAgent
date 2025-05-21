using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAgent.Plugins
{
    public class EmailPlugin
    {
        [KernelFunction]
        [Description("Sends an Email using provided recipient using email provided by StaffLookupPlugin plugin")]
        public string SendEmail(string recipient)
        { 
            // Simulate sending an email
            return $"Email sent to {recipient}";
        }
    }
}
