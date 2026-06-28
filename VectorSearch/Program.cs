using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.InMemory;
using OpenAI;
using System.ClientModel;
using VectorSearch;

// Get credentials from user secrets.
IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var credentials = new ApiKeyCredential(config["GithubModelToken"] ?? throw new InvalidOperationException("GithubModelToken is not set in user secrets."));

var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.github.ai/inference")
};

// Create embeddings client.
IEmbeddingGenerator<string, Embedding<float>> embeddingClient = new OpenAIClient(credentials, options).GetEmbeddingClient("openai/text-embedding-3-small")
    .AsIEmbeddingGenerator();

// Create and populate the vector store.
var vectorStore = new InMemoryVectorStore();
var moviesStore = vectorStore.GetCollection<int, Movie>("movies");

await moviesStore.EnsureCollectionExistsAsync();

foreach (var movie in MovieData.Movies)
{
    // Generate the embedding vector for the movie description.
    movie.Vector = await embeddingClient.GenerateVectorAsync(movie.Description);

    // Add the overall movie to the in memory vector store movie collection.
    await moviesStore.UpsertAsync(movie);
}

var query = "Am bored now, i want to see a movie which should make me happy..";
var queryEmbedding = await embeddingClient.GenerateVectorAsync(query);

var searchResults = moviesStore.SearchAsync(queryEmbedding, 2);

await foreach (var result in searchResults)
{
    Console.WriteLine($"Title: {result.Record.Title}");
    Console.WriteLine($"Description: {result.Record.Description}");
    Console.WriteLine($"Score: {result.Score}\n");
}