using System.Diagnostics.CodeAnalysis;
using UnityEngine;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class VoxelData {
    
    public enum Side {
        BACK,
        FRONT,
        TOP,
        BOTTOM,
        LEFT,
        RIGHT
    }

    public const int chunkHeight = 128;
    public const int textureAtlasWidth = 2;
    public const float normalizedBlockTextureSize = 1f / textureAtlasWidth;


    public static readonly Vector3[] VoxelVertices = {
        new(0.0f, 0.0f, 0.0f),
        new(1.0f, 0.0f, 0.0f),
        new(1.0f, 1.0f, 0.0f),
        new(0.0f, 1.0f, 0.0f),
        new(0.0f, 0.0f, 1.0f),
        new(1.0f, 0.0f, 1.0f),
        new(1.0f, 1.0f, 1.0f),
        new(0.0f, 1.0f, 1.0f)
    };

    public static readonly Vector3Int[] FaceChecks = {
        new(0, 0, -1),
        new(0, 0, 1),
        new(0, 1, 0),
        new(0, -1, 0),
        new(-1, 0, 0),
        new(1, 0, 0)
    };

    public static readonly int[,] VoxelTriangles = {
        { 0, 3, 1, 2 }, // Back Face
        { 5, 6, 4, 7 }, // Front Face
        { 3, 7, 2, 6 }, // Top Face
        { 1, 5, 0, 4 }, // Bottom Face
        { 4, 7, 0, 3 }, // Left Face
        { 1, 2, 5, 6 } // Right FAce
    };

    public static readonly Vector2[] VoxelUvs = {
        new(0.0f, 0.0f),
        new(0.0f, 1.0f),
        new(1.0f, 0.0f),
        new(1.0f, 1.0f)
    };

}