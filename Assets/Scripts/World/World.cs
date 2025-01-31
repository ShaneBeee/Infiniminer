using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Screens;
using UnityEngine;
using World.Block;
using World.Entity;
using Random = UnityEngine.Random;

namespace World {
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    public class World : MonoBehaviour {

        [Header("World Variables")] [SerializeField, Tooltip("Size of world in chunks"), Min(2)]
        internal int worldSize;

        [SerializeField] private int seed;

        [Header("Game Objects")] [SerializeField]
        private Player player;

        [SerializeField] private ScreenManager screenManager;
        [SerializeField] internal Material material;

        private int activeChunks;
        private Vector3 spawnPosition;

        private Chunk.Chunk[,] chunks;

        public int GetSeed() {
            return seed;
        }

        // Unity Methods
        private void Start() {
            Random.InitState(seed);
            StartCoroutine(SetupWorld());
        }

        // Internal Methods
        private IEnumerator SetupWorld() {
            yield return StartCoroutine(GenerateChunks());
            StartCoroutine(SpawnInPlayer());
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator GenerateChunks() {
            chunks ??= new Chunk.Chunk[worldSize, worldSize];
            var loadedChunks = 0;
            var size = worldSize;
            var chunkCount = size * size * 2;
            for (var x = 0; x < size; x++) {
                for (var z = 0; z < size; z++) {
                    CreateNewChunk(x, z);
                    activeChunks++;
                    screenManager.UpdateProgressBar((float)loadedChunks++ / chunkCount);
                    yield return new WaitForEndOfFrame();
                }
            }

            foreach (var chunk in chunks) {
                screenManager.UpdateProgressBar((float)loadedChunks++ / chunkCount);
                chunk.chunkRenderer.RenderChunk();
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator SpawnInPlayer() {
            SetSpawn();
            screenManager.UpdateProgressBar(1.0f);
            yield return new WaitForSeconds(0.5f);
            player.gameObject.SetActive(true);
            player.Teleport(spawnPosition);

            screenManager.LoadingScreenEnabled(false);
        }

        private void CreateNewChunk(int x, int z) {
            chunks[x, z] = new Chunk.Chunk(x, z, this);
        }

        private void SetSpawn() {
            var x = Random.Range(0, worldSize * 16);
            var z = Random.Range(0, worldSize * 16);
            var spawn = new Vector3(x, 0, z);
            for (var i = VoxelData.chunkHeight - 1; i > 0; i--) {
                spawn.y = i;
                if (GetBlock(spawn.ToVector3Int()) == Blocks.AIR) continue;
                spawn.y += 1.5f;
                spawnPosition = spawn;
                return;
            }
        }

        public Vector3 GetSpawnPosition() {
            return spawnPosition;
        }

        public Chunk.Chunk GetChunk(int x, int z) {
            if (x >= chunks.GetLength(0) || z >= chunks.GetLength(1)) {
                return null;
            }

            if (x < 0 || z < 0) {
                return null;
            }

            return chunks[x, z];
        }

        public Block.Block GetBlock(Vector3Int pos) {
            var chunkX = pos.x >> 4;
            var chunkZ = pos.z >> 4;
            var chunk = GetChunk(chunkX, chunkZ);
            if (chunk == null) {
                return null;
            }

            var x = pos.x % 16;
            var y = pos.y;
            var z = pos.z % 16;

            return chunk.GetBlock(x, y, z);
        }

        public void SetBlock(Vector3Int pos, Block.Block block) {
            var chunkX = Mathf.FloorToInt((float)pos.x / 16);
            var chunkZ = Mathf.FloorToInt((float)pos.z / 16);
            var chunk = GetChunk(chunkX, chunkZ);
            if (chunk == null) {
                return;
            }

            var x = pos.x % 16;
            var y = pos.y;
            var z = pos.z % 16;
            chunk.SetBlock(new Vector3Int(x, y, z), block);
        }

        private bool IsChunkInWorld(int x, int z) {
            var sizeInChunks = worldSize - 1;
            return x > 0 && x < sizeInChunks && z > 0 && z < sizeInChunks;
        }

        public bool IsBlockInWorld(Vector3Int pos) {
            var maxBlocks = worldSize * 16;
            const int chunkHeight = VoxelData.chunkHeight;
            var x = pos.x;
            var y = pos.y;
            var z = pos.z;
            return x >= 0 && x < maxBlocks && y >= 0 && y < chunkHeight && z >= 0 && z < maxBlocks;
        }

        public bool CheckForBlock(Vector3Int pos) {
            var block = GetBlock(pos);
            return block != null && block.IsSolid();
        }

    }

}