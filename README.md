# GenAI

A .NET sample solution for trying common generative AI workflows with `Microsoft.Extensions.AI`, GitHub Models/OpenAI-compatible endpoints, Ollama, embeddings, and in-memory vector search.

The repository is organized as a set of small console applications. Each project focuses on one concept so it can be run and modified independently.

## Projects

| Project | Provider | What it demonstrates |
| --- | --- | --- |
| `TextCompletion` | GitHub Models | Basic chat completion, streaming responses, structured output into C# types, and an interactive chat loop. |
| `TextCompletionOllama` | Ollama | Local streaming chat completion using an Ollama model. |
| `Embeddings` | GitHub Models | Generating embedding vectors and comparing them with cosine similarity. |
| `VectorSearch` | GitHub Models | Creating embeddings for movie descriptions, storing them in an in-memory vector store, and searching by semantic similarity. |
| `VectorSearchOllama` | Ollama | Local embedding generation with Ollama and semantic search over the same movie dataset. |

## Tech Stack

- .NET `net10.0`
- C# console applications
- `Microsoft.Extensions.AI`
- `Microsoft.Extensions.AI.OpenAI`
- `Microsoft.SemanticKernel.Connectors.InMemory`
- `OllamaSharp`
- `System.Numerics.Tensors`
- GitHub Models endpoint: `https://models.github.ai/inference`
- Ollama local endpoint: `http://localhost:11434`

## Repository Structure

```text
GenAI/
├── Embeddings/
│   ├── Embeddings.csproj
│   └── Program.cs
├── TextCompletion/
│   ├── CarDetails.cs
│   ├── CarListingType.cs
│   ├── Program.cs
│   └── TextCompletion.csproj
├── TextCompletionOllama/
│   ├── Program.cs
│   └── TextCompletionOllama.csproj
├── VectorSearch/
│   ├── Movie.cs
│   ├── Program.cs
│   └── VectorSearch.csproj
├── VectorSearchOllama/
│   ├── Movie.cs
│   ├── Program.cs
│   └── VectorSearchOllama.csproj
├── GenAI.slnx
├── LICENSE.txt
└── README.md
```

## Prerequisites

- .NET SDK that supports `net10.0`
- A GitHub Models token for the GitHub Models based samples
- Ollama installed and running for the Ollama based samples

## GitHub Models Setup

The GitHub Models samples read a user secret named `GithubModelToken`.

Set the token for each GitHub Models project:

```powershell
dotnet user-secrets set "GithubModelToken" "<your-token>" --project .\TextCompletion\TextCompletion.csproj
dotnet user-secrets set "GithubModelToken" "<your-token>" --project .\Embeddings\Embeddings.csproj
dotnet user-secrets set "GithubModelToken" "<your-token>" --project .\VectorSearch\VectorSearch.csproj
```

These projects use the OpenAI-compatible GitHub Models endpoint:

```csharp
https://models.github.ai/inference
```

Models used in the current code:

- Chat: `openai/gpt-4o-mini`
- Embeddings in `Embeddings`: `text-embedding-3-small`
- Embeddings in `VectorSearch`: `openai/text-embedding-3-small`

## Ollama Setup

Start Ollama locally:

```powershell
ollama serve
```

Pull the models used by the Ollama samples:

```powershell
ollama pull smollm
ollama pull all-minilm
```

The Ollama projects expect Ollama to be available at:

```text
http://localhost:11434
```

Models used in the current code:

- Chat: `smollm`
- Embeddings: `all-minilm`

## Restore and Build

```powershell
dotnet restore .\GenAI.slnx
dotnet build .\GenAI.slnx
```

## Run the Samples

### Text Completion

```powershell
dotnet run --project .\TextCompletion\TextCompletion.csproj
```

This sample demonstrates:

- A simple prompt and response
- Token usage output
- Streaming text completion
- Structured output into `CarDetails`
- A terminal chat assistant that keeps chat history

Note: this sample ends with an interactive `while (true)` chat loop. Stop it with `Ctrl+C` when finished.

### Text Completion with Ollama

```powershell
dotnet run --project .\TextCompletionOllama\TextCompletionOllama.csproj
```

This sample connects to local Ollama with `OllamaApiClient` and streams responses for prompts entered in the terminal.

### Embeddings

```powershell
dotnet run --project .\Embeddings\Embeddings.csproj
```

This sample:

- Generates an embedding for `Hello world`
- Prints the embedding dimensions and values
- Generates embeddings for `cat`, `dog`, and `kitten`
- Compares those embeddings with cosine similarity

### Vector Search

```powershell
dotnet run --project .\VectorSearch\VectorSearch.csproj
```

This sample:

- Generates embeddings for a small movie dataset
- Stores the movies in Semantic Kernel's in-memory vector store
- Generates an embedding for a search query
- Returns the top two semantically similar movie results with scores

### Vector Search with Ollama

```powershell
dotnet run --project .\VectorSearchOllama\VectorSearchOllama.csproj
```

This sample performs the same in-memory vector search flow as `VectorSearch`, but uses local Ollama embeddings from `all-minilm`.

## Data Model Notes

`TextCompletion` includes a structured output example that maps generated JSON into:

- `CarDetails`
- `CarListingType`

`VectorSearch` and `VectorSearchOllama` use a shared movie-style data model with:

- `Key` as the vector store key
- `Title` and `Description` as vector store data
- `Vector` as a cosine-distance vector field

The vector field is currently annotated with `VectorStoreVector(384, DistanceFunction = DistanceFunction.CosineDistance)`, which matches common local embedding models such as `all-minilm`. If you change embedding models, make sure the vector dimension in `Movie.cs` matches the model output.

## Troubleshooting

### `GithubModelToken is not set in user secrets`

Set the token for the project you are running:

```powershell
dotnet user-secrets set "GithubModelToken" "<your-token>" --project .\TextCompletion\TextCompletion.csproj
```

Repeat for `Embeddings` or `VectorSearch` if you run those projects.

### Ollama connection errors

Make sure Ollama is running and the required model is pulled:

```powershell
ollama serve
ollama pull smollm
ollama pull all-minilm
```

### Vector dimension errors

If a vector search sample fails because of vector dimensions, update the `VectorStoreVector(...)` dimension in the relevant `Movie.cs` file or use an embedding model that returns the expected number of dimensions.

## License

This project includes an MIT license file in `LICENSE.txt`.
