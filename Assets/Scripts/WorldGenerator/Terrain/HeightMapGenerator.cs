using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapGenerator
{
    private float Scale;
    private int Octaves;
    private float Persistance;
    private float Lacunarity;
    private HeightCurveType HeightCurve;
    private float MinNoiseHeight;
    private float MaxNoiseHeight;
    private float HeightMultiplier;

    private Vector2[] OctaveOffsets;

    public HeightMapGenerator(
        float scale = 500f,
        int octaves = 5,
        float persistance = 0.5f,
        float lacunarity = 2f,
        HeightCurveType heightCurveType = HeightCurveType.Exponential,
        float minNoiseHeight = -1.5f,
        float maxNoiseHeight = 1.5f,
        float heightMultiplier = 200f)
    {
        if (scale <= 0) scale = 0.0001f;
        Scale = scale;
        Octaves = octaves;
        Persistance = persistance;
        Lacunarity = lacunarity;
        HeightCurve = heightCurveType;
        MinNoiseHeight = minNoiseHeight;
        MaxNoiseHeight = maxNoiseHeight;
        HeightMultiplier = heightMultiplier;

        OctaveOffsets = new Vector2[Octaves];
        for (int i = 0; i < Octaves; i++)
        {
            float offsetX = Random.Range(-10000, 10000);
            float offsetY = Random.Range(-10000, 10000);
            OctaveOffsets[i] = new Vector2(offsetX, offsetY);
        }
    }

    public float[,] GetHeightMap(float chunkCoordX, float chunkCoordY)
    {
        float[,] noiseMap = new float[TerrainChunk.ChunkSize, TerrainChunk.ChunkSize];

        // Create noise map
        for (int y = 0; y < noiseMap.GetLength(1); y++)
        {
            for (int x = 0; x < noiseMap.GetLength(0); x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                // Generate value for each pixel going through n octaves with x persistance and y lacunarity
                for (int i = 0; i < Octaves; i++)
                {
                    float sampleX = (float)(chunkCoordX * (TerrainChunk.ChunkSize - 1) + x) / Scale * frequency + OctaveOffsets[i].x;
                    float sampleY = (float)(chunkCoordY * (TerrainChunk.ChunkSize - 1) + y) / Scale * frequency + OctaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= Persistance;
                    frequency *= Lacunarity;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        // Normalize noise map to [0,1]
        for (int y = 0; y < noiseMap.GetLength(1); y++)
        {
            for (int x = 0; x < noiseMap.GetLength(0); x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(MinNoiseHeight, MaxNoiseHeight, noiseMap[x, y]);
            }
        }

        // Height curve operation
        for (int y = 0; y < noiseMap.GetLength(1); y++)
        {
            for (int x = 0; x < noiseMap.GetLength(0); x++)
            {
                noiseMap[x, y] = HeightCurveOperation.InterpolationByOperation(HeightCurve, 0f, 1f, noiseMap[x, y]);
                noiseMap[x, y] *= HeightMultiplier;
            }
        }


        return noiseMap;
    }


    /*
    public static float[,] GenerateHeightMapWithOwnAlgorithm(int mapChunkSize, WorldMap wm)
    {
        // Init values
        int worldWidth = ((mapChunkSize - 1) * wm.WorldMapWidth) + 1;
        int worldHeight = ((mapChunkSize - 1) * wm.WorldMapHeight) + 1;
        float[,] noiseMap = new float[worldWidth, worldHeight];

        // fill random noise seed with [0,1]
        float[,] noiseSeed = new float[worldWidth * 2, worldHeight * 2];
        for (int y = 0; y < worldHeight; y++)
        {
            for (int x = 0; x < worldWidth; x++)
            {
                noiseSeed[x, y] = Random.value;
            }
        }

        // create perlin noise
        for (int y = 0; y < worldHeight; y++)
        {
            for (int x = 0; x < worldWidth; x++)
            {
                float value = 0f;

                float frequency = 1;
                float amplitude = 1;
                float amplitudeSum = 0f;

                for (int o = 0; o < wm.Octaves; o++)
                {
                    int segmentLengthX = (int)(worldWidth / frequency);
                    int segmentLengthY = (int)(worldHeight / frequency);
                    if (segmentLengthX < 1) segmentLengthX = 1;
                    if (segmentLengthY < 1) segmentLengthY = 1;

                    int nSampleX1 = (x / segmentLengthX) * segmentLengthX;
                    int nSampleY1 = (y / segmentLengthY) * segmentLengthY;

                    int nSampleX2 = (nSampleX1 + segmentLengthX); // add % worldWith to tessalate
                    int nSampleY2 = (nSampleY1 + segmentLengthY); // add % worldWith to tessalate

                    float fBlendX = (float)(x - nSampleX1) / (float)segmentLengthX; // position within pitch [0,1]
                    float fBlendY = (float)(y - nSampleY1) / (float)segmentLengthY; // position within pitch [0,1]

                    float fSampleT = (1f - fBlendX) * noiseSeed[nSampleX1, nSampleY1] + fBlendX * noiseSeed[nSampleX2, nSampleY1];
                    float fSampleB = (1f - fBlendX) * noiseSeed[nSampleX1, nSampleY2] + fBlendX * noiseSeed[nSampleX2, nSampleY2];

                    float octaveValue = (fBlendY * (fSampleB - fSampleT) + fSampleT) * amplitude;

                    value += octaveValue;
                    amplitudeSum += amplitude;

                    frequency *= wm.Lacunarity;
                    amplitude *= wm.Persistance;
                }

                noiseMap[x, y] = value / amplitudeSum;
            }
        }

        return noiseMap;
    }
    */
}
