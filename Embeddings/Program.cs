using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;
using System.Numerics.Tensors;

// Get credentials from user secrets.
IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var credentials = new ApiKeyCredential(config["GithubModelToken"] ?? throw new InvalidOperationException("GithubModelToken is not set in user secrets."));

var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.github.ai/inference")
};

// Create chat client.
IChatClient chatClient = new OpenAIClient(credentials, options).GetChatClient("openai/gpt-4o-mini").AsIChatClient();

// Create embeddings client.
IEmbeddingGenerator<string, Embedding<float>> embeddingClient = new OpenAIClient(credentials, options).GetEmbeddingClient("text-embedding-3-small")
    .AsIEmbeddingGenerator();

var embedding = await embeddingClient.GenerateVectorAsync("Hello world");

Console.WriteLine($"Embedding Dimensions: {embedding.Span.Length}");

foreach (var value in embedding.Span)
{
    Console.Write("{0:0.00} ", value);
}

// Compare multiple embeddings using cosine similarity.
var catVector = await embeddingClient.GenerateVectorAsync("cat");
var dogVector = await embeddingClient.GenerateVectorAsync("dog");
var kittenVector = await embeddingClient.GenerateVectorAsync("kitten");

Console.WriteLine($"cat - dog similarity {TensorPrimitives.CosineSimilarity(catVector.Span, dogVector.Span):F2}");
Console.WriteLine($"cat - kitten similarity {TensorPrimitives.CosineSimilarity(catVector.Span, kittenVector.Span):F2}");
Console.WriteLine($"dog - kitten similarity {TensorPrimitives.CosineSimilarity(dogVector.Span, kittenVector.Span):F2}");