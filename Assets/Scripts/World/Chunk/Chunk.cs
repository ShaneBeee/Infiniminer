using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace World.Chunk {
    [SuppressMessage("ReSharper", "Unity.InefficientMultidimensionalArrayUsage")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Chunk {

        private readonly ChunkCoord coord;
        private readonly GameObject chunkObject;
        internal readonly ChunkRenderer chunkRenderer;

        internal readonly Block.Block[,,] blockMap = new Block.Block[16, VoxelData.ChunkHeight, 16];

        public Chunk(int x, int z, World world) {
            this.coord = new ChunkCoord(x, z);
            this.World = world;
            this.chunkObject = new GameObject();
            this.chunkRenderer = new ChunkRenderer(this, this.chunkObject);
            this.chunkObject.transform.SetParent(world.transform);
            this.chunkObject.transform.position = new Vector3(coord.GetX() * 16, 0f, coord.GetZ() * 16);
            this.chunkObject.name = "Chunk{" + coord.GetX() + "," + coord.GetZ() + "}";

            SetupChunk();
        }

        public World World { get; }

        private void SetupChunk() {
            ChunkGenerator.PopulateBlocks(this);
        }

        public void SetBlock(int x, int y, int z, Block.Block block) {
            if (x < 0 || x > 15 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > 15) {
                throw new ArgumentException($"SetBlock out of range: {x},{y},{z}");
            }

            blockMap[x, y, z] = block;
        }

        public void EditBlock(Vector3 pos, Block.Block block) {
            var x = Mathf.FloorToInt(pos.x);
            var y = Mathf.FloorToInt(pos.y);
            var z = Mathf.FloorToInt(pos.z);

            SetBlock(x, y, z, block);
            this.chunkRenderer.RenderChunk();
            this.chunkRenderer.CheckNeighbour(x, z);
        }

        public Block.Block GetBlock(int x, int y, int z) {
            if (x < 0 || x > 15 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > 15) {
                return null;
            }

            return blockMap[x, y, z];
        }

        public ChunkCoord Coord => coord;

        public bool isActive {
            get => chunkObject.activeSelf;
            set => chunkObject.SetActive(value);
        }

        public Vector3 Position => chunkObject.transform.position;

        private bool IsBlockInChunk(int x, int y, int z) {
            return x >= 0 && x <= 15 && y >= 0 && y <= VoxelData.ChunkHeight - 1 && z >= 0 && z <= 15;
        }


        internal bool IsSolid(Vector3 pos) {
            var x = Mathf.FloorToInt(pos.x);
            var y = Mathf.FloorToInt(pos.y);
            var z = Mathf.FloorToInt(pos.z);

            if (!World.IsBlockInWorld(pos + Position)) {
                return false;
            }

            if (!IsBlockInChunk(x, y, z)) {
                var block = World.GetBlock(new Vector3(x, y, z) + Position);
                if (block != null && block.IsSolid()) {
                    return true;
                }

                return false;
            }

            return blockMap[x, y, z].IsSolid();
        }

    }

}