using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace MCP_POC.Business.MCP
{
    public class McpManager: IMcpManager
    {
        //private readonly IChatClient _chatClient;
        private readonly McpClient _mcpClient;

        public McpManager()
        {
            // Azure OpenAI client setup
            //var azureClient = new AzureOpenAIClient(
            //    new Uri(azureOpenAiEndpoint),
            //    new DefaultAzureCredential());

            //_chatClient = new ChatClientBuilder(
            //    azureClient.GetChatClient("gpt-4o").AsIChatClient())
            //    .UseFunctionInvocation()
            //    .Build();


            HttpClientTransportOptions transportOptions = new HttpClientTransportOptions
            {
                Endpoint = new Uri("https://kinetic.fastmcp.app/mcp")
            };

            // MCP remote server connection (via HTTPS)
            _mcpClient = McpClient.CreateAsync(
                                new HttpClientTransport(
                                    transportOptions
                                )
            ).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task<IList<McpClientTool>> GetTools()
        {
            IList<McpClientTool> tools = await _mcpClient.ListToolsAsync();
            return tools;
        }
        //public async Task<Response> SendPromptAsync(Request request)
        //{
        //    var messages = new List<ChatMessage> { new(ChatRole.User, request.Prompt) };
        //    var tools = await _mcpClient.ListToolsAsync();
        //    var updates = new List<ChatResponseUpdate>();

        //    await foreach (var update in _chatClient.GetStreamingResponseAsync(messages, new() { Tools = [.. tools] }))
        //    {
        //        updates.Add(update);
        //        Console.Write(update); // stream partial output to console
        //    }

        //    messages.AddMessages(updates);
        //    return new Response { Content = string.Join("", updates) };
        //}
    }
}
