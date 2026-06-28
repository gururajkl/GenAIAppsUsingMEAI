using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Connectors.InMemory;
using OllamaSharp;
using VectorSearchOllama;

// Create embeddings client.
IEmbeddingGenerator<string, Embedding<float>> embeddingClient =
    new OllamaApiClient(new Uri("http://localhost:11434/"),
    "all-minilm");

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

var query = "I have to do my space project ASAP, suggest a better movie for this..";
var queryEmbedding = await embeddingClient.GenerateVectorAsync(query);

var searchResults = moviesStore.SearchAsync(queryEmbedding, 2);

await foreach (var result in searchResults)
{
    Console.WriteLine($"Title: {result.Record.Title}");
    Console.WriteLine($"Description: {result.Record.Description}");
    Console.WriteLine($"Score: {result.Score}\n");
}