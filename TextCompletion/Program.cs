using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;
using TextCompletion;

// Get credentials from user secrets.
IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var credentials = new ApiKeyCredential(config["GithubModelToken"] ?? throw new InvalidOperationException("GithubModelToken is not set in user secrets."));

var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.github.ai/inference")
};

// Create chat client.
IChatClient chatClient = new OpenAIClient(credentials, options).GetChatClient("openai/gpt-4o-mini").AsIChatClient();

#region Basic completion.
// Send prompt and get response.
var response = await chatClient.GetResponseAsync("What is AI? Explain in 20 words");

Console.WriteLine(response);

Console.WriteLine($"Token used: input token {response.Usage?.InputTokenCount} and output token {response.Usage?.OutputTokenCount}");
#endregion

#region Streaming completion.
string prompt = "What is AI? Explain in 200 words";
Console.WriteLine($"Prompt >> {prompt}");

var responseStream = chatClient.GetStreamingResponseAsync(prompt);

await foreach (var message in responseStream)
{
    Console.Write(message.Text);
}
#endregion

#region Structured output
var carListings = new[]
{
    "Check out this stylish 2019 Toyota Camry. It has a clean title, only 40,000 miles on the odometer, and a well-maintained interior. The car offers great fuel efficiency, a spacious trunk, and modern safety features like lane departure alert. Minimum offer price: $18,000. Contact Metro Auto at (555) 111-2222 to schedule a test drive.",
    "Lease this sporty 2021 Honda Civic! With only 10,000 miles, it includes a sunroof, premium sound system, and backup camera. Perfect for city driving with its compact size and great fuel mileage. Located in Uptown Motors, monthly lease starts at $250 (excl. taxes). Call (555) 333-4444 for more info.",
    "A classic 1968 Ford Mustang, perfect for enthusiasts. The vehicle needs some interior restoration, but the engine runs smoothly. V8 engine, manual transmission, around 80,000 miles. This vintage gem is priced at $25,000. Contact Retro Wheels at (555) 777-8888 if you’re interested.",
    "Brand new 2023 Tesla Model 3 for lease. Zero miles, fully electric, autopilot capabilities, and a sleek design. Monthly lease starts at $450. Clean lines, minimalist interior, top-notch performance. For more details, call EVolution Cars at (555) 999-0000.",
    "Selling a 2015 Subaru Outback in good condition. 60,000 miles on it, includes all-wheel drive, heated seats, and ample cargo space for family getaways. Minimum offer price: $14,000. Contact Forrest Autos at (555) 222-1212 if you want a reliable adventure companion.",
};

foreach (var car in carListings)
{
    var responseStructured = await chatClient.GetResponseAsync<CarDetails>($"""
        Convert the following car listing into a JSON object matching this C# schema:
        Condition: "New" or "Used"
        Make: (car manufacturer)
        Model: (car model)
        Year: (four-digit year)
        ListingType: "Sale" or "Lease"
        Price: integer only
        Features: array of short strings
        TenWordSummary: exactly ten words to summarize this listing
        Here is the listing:
        {car}
        """);

    if (responseStructured.TryGetResult(out var info))
    {
        // Convert the CarDetails object to JSON for display
        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(
            info, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
    }
    else
    {
        Console.WriteLine("Response was not in the expected format.");
    }
}
#endregion

#region Chat App
List<ChatMessage> chatHistory =
[
    new ChatMessage(ChatRole.System, """
            You are a friendly hiking enthusiast who helps people discover fun hikes in their area.
            You introduce yourself when first saying hello.
            When helping people out, you always ask them for this information
            to inform the hiking recommendation you provide:

            1. The location where they would like to hike
            2. What hiking intensity they are looking for

            You will then provide three suggestions for nearby hikes that vary in length
            after you get that information. You will also share an interesting fact about
            the local nature on the hikes when making a recommendation. At the end of your
            response, ask if there is anything else you can help with.
        """)
];

while (true)
{
    // Get user prompt and add to chat history
    Console.WriteLine("Your prompt:");
    var userPrompt = Console.ReadLine();
    chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

    // Stream the AI response and add to chat history
    Console.WriteLine("AI Response:");
    var responseChat = "";
    await foreach (var item in chatClient.GetStreamingResponseAsync(chatHistory))
    {
        Console.Write(item.Text);
        responseChat += item.Text;
    }
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, responseChat));
    Console.WriteLine();
}
#endregion