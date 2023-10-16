using UnityEngine;

public static class VoxelData {

    public const int ChunkHeight = 128;
    public const int WorldSizeInChunks = 8;
    public const int WorldSizeInBlocks = WorldSizeInChunks * 16;

    public const int TextureAtlasSizeInBlocks = 2;

    public const float NormalizedBlockTextureSize = 1f / (float) TextureAtlasSizeInBlocks;


    public static readonly Vector3[] VoxelVertices = new Vector3[8] {
        new(0.0f, 0.0f, 0.0f),
        new(1.0f, 0.0f, 0.0f),
        new(1.0f, 1.0f, 0.0f),
        new(0.0f, 1.0f, 0.0f),
        new(0.0f, 0.0f, 1.0f),
        new(1.0f, 0.0f, 1.0f),
        new(1.0f, 1.0f, 1.0f),
        new(0.0f, 1.0f, 1.0f)
    };

    public static readonly Vector3Int[] FaceChecks = new Vector3Int[6] {
        new(0, 0, -1),
        new(0, 0, 1),
        new(0, 1, 0),
        new(0, -1, 0),
        new(-1, 0, 0),
        new(1, 0, 0)
    };

    public static readonly int[,] VoxelTriangles = new int[6, 4] {
        {0, 3, 1, 2}, // Back Face
        {5, 6, 4, 7}, // Front Face
        {3, 7, 2, 6}, // Top Face
        {1, 5, 0, 4}, // Bottom Face
        {4, 7, 0, 3}, // Left Face
        {1, 2, 5, 6} // Right FAce

    };

    public static readonly Vector2[] VoxelUvs = new Vector2[4] {
        new(0.0f, 0.0f),
        new(0.0f, 1.0f),
        new(1.0f, 0.0f),
        new(1.0f, 1.0f)
    };

}