using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using World.Block;

namespace World.Chunk {
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ChunkRenderer {

        private readonly Chunk chunk;
        private GameObject chunkObject;
        private readonly MeshFilter meshFilter;
        private readonly MeshCollider meshCollider;
        private int vertexIndex;
        private readonly List<Vector3> vertices = new();
        private readonly List<int> triangles = new();
        private readonly List<Vector2> uvs = new();

        public ChunkRenderer(Chunk chunk, GameObject chunkObject) {
            this.chunk = chunk;
            this.chunkObject = chunkObject;
            this.meshFilter = chunkObject.AddComponent<MeshFilter>();
            this.meshCollider = this.meshFilter.AddComponent<MeshCollider>();
            this.meshCollider.material = new PhysicMaterial {
                staticFriction = 0,
                dynamicFriction = 0
            };
            var meshRenderer = chunkObject.AddComponent<MeshRenderer>();
            meshRenderer.material = this.chunk.World.material;
        }

        public void RenderChunk() {
            ClearMeshData();
            for (int y = 0; y < VoxelData.ChunkHeight; y++) {
                for (int x = 0; x < 16; x++) {
                    for (int z = 0; z < 16; z++) {
                        var pos = new Vector3(x, y, z);
                        var block = this.chunk.blockMap[x, y, z];
                        if (block != null && block != Blocks.AIR) {
                            UpdateMeshData(pos);
                        }
                    }
                }
            }

            CreateMesh();
        }

        internal void CheckNeighbour(int x, int z) {
            var chunkCoord = this.chunk.Coord;

            // Update neighbour chunk TODO
            if (x == 0) {
                UpdateNeighbourChunk(chunkCoord.GetX() - 1, chunkCoord.GetZ());
            } else if (x == 15) {
                UpdateNeighbourChunk(chunkCoord.GetX() + 1, chunkCoord.GetZ());
            }

            if (z == 0) {
                UpdateNeighbourChunk(chunkCoord.GetX(), chunkCoord.GetZ() - 1);
            } else if (z == 15) {
                UpdateNeighbourChunk(chunkCoord.GetX(), chunkCoord.GetZ() + 1);
            }
        }

        private void UpdateNeighbourChunk(int x, int z) {
            var neighbourChunk = this.chunk.World.GetChunk(x, z);
            neighbourChunk?.chunkRenderer.RenderChunk();
        }

        private void ClearMeshData() {
            vertexIndex = 0;
            vertices.Clear();
            triangles.Clear();
            uvs.Clear();
        }

        private void UpdateMeshData(Vector3 pos) {
            for (int p = 0; p < 6; p++) {
                // only draw if neighbouring block is not solid
                if (this.chunk.IsSolid(pos + VoxelData.FaceChecks[p])) continue;

                for (int v = 0; v < 4; v++) {
                    vertices.Add(pos + VoxelData.VoxelVertices[VoxelData.VoxelTriangles[p, v]]);
                }

                var block = this.chunk.blockMap[Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y),
                    Mathf.FloorToInt(pos.z)];
                AddTexture(block);

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
                vertexIndex += 4;
            }
        }

        private void CreateMesh() {
            var mesh = new Mesh {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                uv = uvs.ToArray()
            };

            mesh.RecalculateNormals();
            this.meshFilter.mesh = mesh;
            this.meshCollider.sharedMesh = mesh;
        }

        private void AddTexture(Block.Block block) {
            float textureID = block.GetTextureId();
            var normal = VoxelData.NormalizedBlockTextureSize;
            var size = VoxelData.TextureAtlasSizeInBlocks;

            var y = (size - 1 - Mathf.Floor(textureID / size)) / size;
            var x = (textureID % size) / size;

            uvs.Add(new Vector2(x, y));
            uvs.Add(new Vector2(x, y + normal));
            uvs.Add(new Vector2(x + normal, y));
            uvs.Add(new Vector2(x + normal, y + normal));
        }
    }
}