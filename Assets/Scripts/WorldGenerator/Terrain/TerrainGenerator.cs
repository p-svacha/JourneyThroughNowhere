using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator
{
    public HeightMapGenerator HeightMapGenerator;
    public Dictionary<Vector2, TerrainChunk> TerrainChunks;

    public GameObject TerrainObject;

    public TerrainGenerator()
    {
        HeightMapGenerator = new HeightMapGenerator();
        TerrainChunks = new Dictionary<Vector2, TerrainChunk>();

        TerrainObject = new GameObject("Terrain");

        for(int y = -2; y < 3; y++)
        {
            for(int x = -2; x < 3; x++)
            {
                Vector2 coordinates = new Vector2(x, y);
                float[,] heightMap = HeightMapGenerator.GetHeightMap(x, y);
                TerrainChunk chunk = TerrainMeshGenerator.GenerateTerrainMesh(heightMap, 0);
                chunk.transform.SetParent(TerrainObject.transform);
                TerrainChunks.Add(coordinates, chunk);
                chunk.Init(x, -y, heightMap);
            }
        }
    }

    public float GetElevation(Vector2 worldPosition)
    {
        Vector2 adjustedPosition = new Vector2((worldPosition.x + TerrainChunk.ChunkSize / 2), (-worldPosition.y + TerrainChunk.ChunkSize / 2));
        Vector2 chunkCoordinates = new Vector2(Mathf.Floor(adjustedPosition.x / TerrainChunk.ChunkSize), Mathf.Floor(adjustedPosition.y / TerrainChunk.ChunkSize));
        TerrainChunk chunk = TerrainChunks[chunkCoordinates];
        Vector2 positionOnChunk = new Vector2(adjustedPosition.x - chunkCoordinates.x * TerrainChunk.ChunkSize, adjustedPosition.y - chunkCoordinates.y * TerrainChunk.ChunkSize);

        Debug.Log("World Position: " + worldPosition.ToString() + " => Adjusted position: " + adjustedPosition.ToString() + " / Chunk coordinates: " + chunkCoordinates.ToString() + " / Position On Chunk: " + positionOnChunk.ToString());

        return chunk.GetElevation(positionOnChunk);
    }
}
