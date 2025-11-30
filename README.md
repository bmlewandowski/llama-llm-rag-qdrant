# Llama LLM RAG Qdrant Project

This project demonstrates a Retrieval-Augmented Generation (RAG) pipeline using a Large Language Model (LLM) and Qdrant as the vector database. It consists of three main components:

- **Data Optimization**: Prepares and optimizes JSON data for ingestion.
- **Database Population**: Loads the optimized data into the Qdrant vector database.
- **RAG Application**: Provides a chat interface to communicate with the LLM using the populated database.

## Project Structure

```
localrag.sln
populateDb/
    chunk_rag_data.py
    custom-data.json
    old=custom-data-optimized.json
    PopulateDB.csproj
    Program.cs
RagApp/
    Chat.csproj
    Program.cs
SharedLib/
    OllamaResponse.cs
    Shared.csproj
    UserRecord.cs
vault/
```

---

## 1. Optimizing the JSON Data

Before populating the database, the raw data in `custom-data.json` should be optimized for efficient chunking and retrieval.

### Steps:
1. **Edit or update** your source data in `populateDb/custom-data.json`.
2. **Run the optimization script**:

   > **Note:** If your system uses Python 3.x, you may need to use `python3` instead of `python`.

   ```bash
   cd populateDb
   python chunk_rag_data.py
   # or
   python3 chunk_rag_data.py
   ```

   This script processes `custom-data.json` and outputs an optimized version (e.g., `custom-data-optimized.json`).

---

## 2. Populating the RAG Database

The optimized JSON data is loaded into the Qdrant vector database using the C# population tool.

### Steps:
1. **Build the population tool**:
   
   ```bash
   dotnet build PopulateDB.csproj
   ```
2. **Run the population tool**:
   
   ```bash
   dotnet run --project PopulateDB.csproj
   ```
   
   This will read the optimized JSON and populate the Qdrant database with vectorized data.

---

## 3. Running the RAG Application

The RAG application provides a chat interface to interact with the LLM, retrieving relevant context from the Qdrant database.

### Steps:
1. **Build the chat application**:
   
   ```bash
   dotnet build RagApp/Chat.csproj
   ```
2. **Run the chat application**:
   
   ```bash
   dotnet run --project RagApp/Chat.csproj
   ```
3. **Interact with the LLM**:
   
   Follow the prompts in the console to ask questions. The app will retrieve relevant context from Qdrant and use the LLM to generate responses.

---

## Requirements
- [.NET 9.0 or 10.0 SDK](https://dotnet.microsoft.com/download)
- [Python 3.x](https://www.python.org/downloads/) (for data optimization)
- Qdrant instance running (local or remote)
  
   To quickly set up a local Qdrant instance using Docker, run:
  
   ```bash
   docker run -p 6333:6333 -p 6334:6334 -d --name qdrant qdrant/qdrant
   ```
- LLM endpoint (e.g., Ollama, OpenAI, etc.)

---

## Notes
- Update connection strings and API keys as needed in the configuration files.
- The `SharedLib` project contains shared models and logic used by both the population and chat applications.
- For advanced usage, modify the chunking logic in `chunk_rag_data.py` or extend the C# projects as needed.

---

## References

- [YouTube: Simple RAG with Qdrant & Ollama](https://www.youtube.com/watch?v=d60njrs_raY)
- [GitHub: alex-wolf-ps/simple-rag-local](https://github.com/alex-wolf-ps/simple-rag-local)

---

MIT License
