using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAgent.Common.ToolModels
{
    public class EmailToolModelProperties
    {
        public ToolProperty to { get; set; }
        public ToolProperty subject { get; set; }
        public ToolProperty body { get; set; }
        public ToolProperty from { get; set; }
    }
}
