using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using World.Block;

namespace World.Chunk {
    public class Chunk {

        private ChunkCoord _coord;
        private GameObject _chunkObject;
        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;

        private int _vertexIndex;
        private readonly List<Vector3> _vertices = new();
        private readonly List<int> _triangles = new();
        private readonly List<Vector2> _uvs = new();

        private readonly Block.Block[,,] _blockMap = new Block.Block[16, VoxelData.ChunkHeight, 16];
        private World _world;

        public Chunk(int x, int z, World world) {
            Init(new ChunkCoord(x, z), world);
        }

        public Chunk(ChunkCoord coord, World world) {
            Init(coord, world);
        }

        private void Init(ChunkCoord coord, World world) {
            _coord = coord;
            _world = world;
            _chunkObject = new GameObject();
            _meshFilter = _chunkObject.AddComponent<MeshFilter>();
            _meshRenderer = _chunkObject.AddComponent<MeshRenderer>();
            _meshRenderer.material = world.material;
            _chunkObject.transform.SetParent(world.transform);
            _chunkObject.transform.position = new Vector3(coord.GetX() * 16, 0f, coord.GetZ() * 16);
            _chunkObject.name = "Chunk{" + coord.GetX() + "," + coord.GetZ() + "}";

            SetupChunk();
        }

        public World World => _world;

        private async void SetupChunk() {
            //PopulateBlocks();
            ChunkGenerator.PopulateBlocks(this);
            // Small delay to make sure neighbour chunks
            // are generated before attempting to check
            await Task.Delay(10);
            UpdateChunk();
        }

        public void SetBlock(int x, int y, int z, Block.Block block) {
            if (x < 0 || x > 15 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > 15) {
                throw new ArgumentException($"SetBlock out of range: {x},{y},{z}");
            }
            _blockMap[x, y, z] = block;
        }

        public void EditBlock(Vector3 pos, Block.Block block) {
            var x = Mathf.FloorToInt(pos.x);
            var y = Mathf.FloorToInt(pos.y);
            var z = Mathf.FloorToInt(pos.z);

            SetBlock(x, y, z, block);
            UpdateChunk();

            // Update neighbour chunk TODO
            if (x == 0) {
                UpdateNeighbourChunk(_coord.GetX() - 1, _coord.GetZ());
            }
            if (x == 15) {
                UpdateNeighbourChunk(_coord.GetX() + 1, _coord.GetZ());
            }
            if (z == 0) {
                UpdateNeighbourChunk(_coord.GetX(), _coord.GetZ() - 1);
            }
            if (z == 15) {
                UpdateNeighbourChunk(_coord.GetX(), _coord.GetZ() + 1);
            }
        }

        private void UpdateNeighbourChunk(int x, int z) {
            var chunk = _world.GetChunk(x, z);
            chunk?.UpdateChunk();
        }

        public Block.Block GetBlock(int x, int y, int z) {
            if (x < 0 || x > 15 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > 15) {
                return null;
            }

            return _blockMap[x, y, z];
        }

        public ChunkCoord Coord => _coord;

        private void UpdateChunk() {
            ClearMeshData();
            for (int y = 0; y < VoxelData.ChunkHeight; y++) {
                for (int x = 0; x < 16; x++) {
                    for (int z = 0; z < 16; z++) {
                        var pos = new Vector3(x, y, z);
                        var block = _blockMap[x, y, z];
                        if (block != null && block != Blocks.AIR) {
                            UpdateMeshData(pos);
                        }
                    }
                }
            }
            CreateMesh();
        }

        private void ClearMeshData() {
            _vertexIndex = 0;
            _vertices.Clear();
            _triangles.Clear();
            _uvs.Clear();
        }

        public bool isActive {
            get => _chunkObject.activeSelf;
            set => _chunkObject.SetActive(value);
        }

        public Vector3 Position => _chunkObject.transform.position;

        private bool IsBlockInChunk(int x, int y, int z) {
            return x >= 0 && x <= 15 && y >= 0 && y <= VoxelData.ChunkHeight - 1 && z >= 0 && z <= 15;
        }
        

        private bool IsSolid(Vector3 pos) {
            var x = Mathf.FloorToInt(pos.x);
            var y = Mathf.FloorToInt(pos.y);
            var z = Mathf.FloorToInt(pos.z);

            if (!_world.IsBlockInWorld(pos + Position)) {
                return false;
            }

            if (!IsBlockInChunk(x, y, z)) {
                var block = _world.GetBlock(new Vector3(x, y, z) + Position);
                if (block != null && block.IsSolid()) {
                    return true;
                }

                return false;
            }

            return _blockMap[x, y, z].IsSolid();
        }

        private void UpdateMeshData(Vector3 pos) {
            for (int p = 0; p < 6; p++) {
                // only draw if neighbouring block is not solid
                if (IsSolid(pos + VoxelData.FaceChecks[p])) continue;

                for (int v = 0; v < 4; v++) {
                    _vertices.Add(pos + VoxelData.VoxelVertices[VoxelData.VoxelTriangles[p, v]]);
                }

                var block = _blockMap[Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z)];
                AddTexture(block);

                _triangles.Add(_vertexIndex);
                _triangles.Add(_vertexIndex + 1);
                _triangles.Add(_vertexIndex + 2);
                _triangles.Add(_vertexIndex + 2);
                _triangles.Add(_vertexIndex + 1);
                _triangles.Add(_vertexIndex + 3);
                _vertexIndex += 4;
            }
        }

        private void CreateMesh() {
            var mesh = new Mesh();
            mesh.vertices = _vertices.ToArray();
            mesh.triangles = _triangles.ToArray();
            mesh.uv = _uvs.ToArray();

            mesh.RecalculateNormals();
            _meshFilter.mesh = mesh;
        }

        private void AddTexture(Block.Block block) {
            float textureID = block.GetTextureId();
            var normal = VoxelData.NormalizedBlockTextureSize;
            var size = VoxelData.TextureAtlasSizeInBlocks;

            var y = (size - 1 - Mathf.Floor(textureID / size)) / size;
            var x = (textureID % size) / size;

            _uvs.Add(new Vector2(x, y));
            _uvs.Add(new Vector2(x, y + normal));
            _uvs.Add(new Vector2(x + normal, y));
            _uvs.Add(new Vector2(x + normal, y + normal));
        }

    }
}