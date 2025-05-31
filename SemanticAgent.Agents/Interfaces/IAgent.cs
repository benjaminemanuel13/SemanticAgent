using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAgent.Business.Interfaces
{
    public interface IAgent
    {
        Task<string> Ask(string question, Action<string> del = null);
    }
}
