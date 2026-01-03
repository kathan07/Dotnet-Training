using ModelContextProtocol.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCP_POC.Business.MCP
{
    public interface IMcpManager
    {
        Task<IList<McpClientTool>> GetTools();
    }
}
