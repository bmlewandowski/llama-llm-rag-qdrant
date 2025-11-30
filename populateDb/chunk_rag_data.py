import json
import re

def chunk_text(text, max_length=350, overlap=50):
    # Overlapping chunking by character
    start = 0
    chunks = []
    while start < len(text):
        end = min(start + max_length, len(text))
        chunks.append(text[start:end].strip())
        if end == len(text):
            break
        start += max_length - overlap
    return chunks

with open('custom-data.json', 'r', encoding='utf-8') as f:
    data = json.load(f)

optimized = []
for entry in data:
    name = entry.get('name', 'unknown')
    desc = entry.get('description', '')
    chunks = chunk_text(desc)
    for i, chunk in enumerate(chunks, 1):
        optimized.append({
            "id": f"{re.sub(r'[^a-z0-9]+', '-', name.lower()).strip('-')}-{i}",
            "name": name,
            "description": chunk
        })

with open('custom-data-optimized.json', 'w', encoding='utf-8') as f:
    json.dump(optimized, f, ensure_ascii=False, indent=2)