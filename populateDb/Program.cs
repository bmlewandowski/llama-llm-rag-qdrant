using Microsoft.Extensions.AI;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using Shared;
using System.Text.Json;

#region createClients

// Create Qdrant client
var qClient = new QdrantClient("localhost");

// Create Ollama embedding client
// Use overload (Uri endpoint, string model)
var generator = new OllamaEmbeddingGenerator(new Uri("http://localhost:11434"), "nomic-embed-text");

#endregion

#region loadDataFromFiles

Console.WriteLine($"Loading records...");
var userRecords = new List<UserRecord>();

// Chunk Data
// python chunk_rag_data.py

var dataFileName = "custom-data-optimized.json";
// Resolve path whether running from repo root or project output folder
var possiblePaths = new[]
{
    Path.Combine(AppContext.BaseDirectory, dataFileName),
    Path.Combine(Directory.GetCurrentDirectory(), dataFileName),
    Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location) ?? string.Empty, dataFileName)
};
var dataPath = possiblePaths.FirstOrDefault(File.Exists) ?? dataFileName;
if (!File.Exists(dataPath))
{
    Console.WriteLine($"Data file '{dataFileName}' not found in any known location. Checked: {string.Join(", ", possiblePaths)}");
    return; // abort gracefully
}
string usersRaw = File.ReadAllText(dataPath);
userRecords.AddRange(JsonSerializer.Deserialize<List<UserRecord>>(usersRaw) ?? new List<UserRecord>());

#endregion

#region vectorizeLoadedData

// Create qdrant collection
var qdrantRecords = new List<PointStruct>();

foreach (var item in userRecords)
{
    // Create and assign an embedding for each record with basic retry
    try
    {
        // Use both name and description fields for embedding
        var embeddingInput = $"{item.Id}: {item.Name}: {item.Description}";
        var embedding = (await generator.GenerateAsync(new List<string>() { embeddingInput }))[0];
        item.Embedding = embedding.Vector.ToArray();
    }
    catch (Exception ex)
    {
        // Skip this record if embedding can't be generated
        Console.WriteLine($"Failed to generate embedding for '{item.Name}': {ex.Message}");
        continue;
    }

    // Add each record and its embedding to the list that will be inserted into the databsae
    qdrantRecords.Add(new PointStruct()
    {
        Id = new PointId((uint)new Random().Next(0, 10000000)),
        Vectors = item.Embedding,
        Payload =
        {
            ["name"] = item.Name,
            ["description"] = item.Description
        }
    });
}
#endregion

#region insertDataIntoQdrantDB

// Create the db collection
await qClient.CreateCollectionAsync("custom-rag-database", new VectorParams { Size = 768, Distance = Distance.Cosine });

// Insert the records into the database
await qClient.UpsertAsync("custom-rag-database", qdrantRecords);
Console.WriteLine("Finished inserting records!");

#endregion
