using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Qdrant.Client;
using Shared;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

#region createClients
var qClient = new QdrantClient("localhost");

IEmbeddingGenerator<string, Embedding<float>> generator =
    new OllamaEmbeddingGenerator(new Uri("http://localhost:11434/"), "nomic-embed-text");

IChatClient chatClient =
    new OllamaChatClient(new Uri("http://localhost:11434/"), "llama3.2:latest");

Console.WriteLine("Ask Away:");
#endregion

while (true)
{
    Console.WriteLine();

    // Create chat history
    List<ChatMessage> chatHistory = new();

    // Get user prompt
    var userPrompt = Console.ReadLine();

    // Create an embedding version of the prompt
    var promptEmbedding = (await generator.GenerateAsync(new List<string>() { userPrompt }))[0].Vector.ToArray();

    // Run a vector search using the prompt embedding
    var searchResult = await qClient.SearchAsync(
        collectionName: "custom-rag-database",
        vector: promptEmbedding,
        limit: 25
    );
    // Sort by score descending and take the top 25 results for context
    var returnedLocations = searchResult.OrderByDescending(r => r.Score).Take(25).ToList();
    
    // Use this for generic chat
    //chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

    // Use this for grounded chat
    // Add the returned records from the vector search to the prompt
    var builder = new StringBuilder();
    foreach (var location in returnedLocations)
    {
        builder.AppendLine($"{location.Payload["name"].StringValue}: {location.Payload["description"].StringValue}.");
    }

    // Assemble the full prompt to the chat AI model using instructions,
    // the original user prompt, and the retrieved relevant data
    chatHistory.Add(new ChatMessage(ChatRole.User,
        $@"You are a helpful assistant with a customer service attitude. Use the information from the RAG database (provided as context) to answer the user's questions as accurately and helpfully as possible.

        [Context]
        {builder}

        [User Question]
        {userPrompt}

        [Instructions]
        - Reference specific facts, details, or data from the context when possible.
        - If the context does not fully answer the question, politely let the user know and provide your best possible answer based on the available information.
        - Maintain a friendly, professional, and customer-focused tone in all responses."
    ));

    // Stream the AI response and add to chat history
    Console.WriteLine("LLM Response:");
    await foreach (var item in
        chatClient.CompleteStreamingAsync(chatHistory))
        {
            Console.Write(item.Text);
        }
    Console.WriteLine();
}
