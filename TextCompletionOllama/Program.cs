// Create Ollama Chat client.
using Microsoft.Extensions.AI;
using OllamaSharp;

IChatClient chatClient = new OllamaApiClient("http://localhost:11434", "smollm");

while (true)
{
    List<ChatMessage> chatHistory = [];

    Console.WriteLine("Enter your prompt: ");
    string userPrompt = Console.ReadLine();

    if (userPrompt is null)
    {
        break;
    }

    chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

    string aiResponse = "";
    await foreach (var response in chatClient.GetStreamingResponseAsync(chatHistory))
    {
        aiResponse += response.Text;
        Console.Write(response.Text);
    }

    Console.WriteLine("\n");
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, aiResponse));
}