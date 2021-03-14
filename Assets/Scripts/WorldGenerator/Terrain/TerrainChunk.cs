using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk : MonoBehaviour
{
    public const int ChunkSize = 241; // 240 is divisible through 2,4,6,8,10 => good for LOD

    public Vector2 Coordinates;
    public float[,] HeightMap;

    public void Init(int x, int y, float[,] heightMap)
    {
        Coordinates = new Vector2(x, y);
        HeightMap = heightMap;
        transform.position = new Vector3(Coordinates.x * (ChunkSize - 1), 0f, Coordinates.y * (ChunkSize - 1));
    }

    public float GetElevation(Vector2 position)
    {
        return HeightMap[Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y)];
    }
}
