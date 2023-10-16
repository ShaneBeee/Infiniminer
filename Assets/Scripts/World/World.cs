using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using World.Block;
using Random = UnityEngine.Random;

namespace World {
    public class World : MonoBehaviour {

        [SerializeField] private int seed;
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject loadingScreenCanvas;
        [SerializeField] private GameObject debugScreenCanvas;
        [SerializeField] private GameObject menuScreenCanvas;
        [SerializeField] private Slider slider;
        [SerializeField] internal Material material;

        private Text debugText;
        private int activeChunks;
        private Vector3 spawnPosition;

        private readonly Chunk.Chunk[,] chunks =
            new Chunk.Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];

        // Unity Methods
        private void Start() {
            Random.InitState(seed);
            loadingScreenCanvas.SetActive(true);
            debugScreenCanvas.SetActive(false);
            menuScreenCanvas.SetActive(false);
            debugText = debugScreenCanvas.GetComponentInChildren<Text>();
            if (debugText == null) {
                Debug.LogError("Debug text == null");
            }

            StartCoroutine(SetupWorld());
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.F3)) {
                debugScreenCanvas.SetActive(!debugScreenCanvas.activeSelf);
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                var menuActive = menuScreenCanvas.activeSelf;
                menuScreenCanvas.SetActive(!menuActive);
                Cursor.lockState = !menuActive ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = !menuActive;
            }
        }

        // Internal Methods
        private IEnumerator SetupWorld() {
            yield return StartCoroutine(GenerateChunks());
            StartCoroutine(SpawnInPlayer());
        }

        public void GenChunksForEditor() {
            StartCoroutine(GenerateChunks());
        }

        public void ClearChunksFromEditor() {
            Debug.Log("Preparing to destroy chunks!");
            foreach (var chunk in chunks) {
                if (chunk != null) {
                    DestroyImmediate(chunk.chunkObject);
                }
            }

            Array.Clear(chunks, 0, chunks.Length);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator GenerateChunks() {
            var loadedChunks = 0;
            var size = VoxelData.WorldSizeInChunks;
            var chunkCount = size * size * 2;
            for (int x = 0; x < size; x++) {
                for (int z = 0; z < size; z++) {
                    CreateNewChunk(x, z);
                    activeChunks++;
                    slider.value = (float)loadedChunks++ / chunkCount;
                    yield return new WaitForSeconds(0.001f);
                }
            }

            foreach (var chunk in chunks) {
                slider.value = (float)loadedChunks++ / chunkCount;
                chunk.chunkRenderer.RenderChunk();
                yield return new WaitForSeconds(0.001f);
            }

        }

        private IEnumerator SpawnInPlayer() {
            SetSpawn();
            slider.value = 1.0f;
            yield return new WaitForSeconds(0.5f);
            player.SetActive(true);
            player.transform.position = spawnPosition;
            loadingScreenCanvas.SetActive(false);
        }

        private void CreateNewChunk(int x, int z) {
            chunks[x, z] = new Chunk.Chunk(x, z, this);
        }

        private void SetSpawn() {
            var spawn = new Vector3(16, 0, 16);
            for (int i = VoxelData.ChunkHeight - 1; i > 0; i--) {
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
            var chunkX = Mathf.FloorToInt(pos.x) >> 4;
            var chunkZ = Mathf.FloorToInt(pos.z) >> 4;
            var chunk = GetChunk(chunkX, chunkZ);
            if (chunk == null) {
                return null;
            }

            var x = Mathf.FloorToInt(pos.x) % 16;
            var y = Mathf.FloorToInt(pos.y);
            var z = Mathf.FloorToInt(pos.z) % 16;

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
            var sizeInChunks = VoxelData.WorldSizeInChunks - 1;
            return x > 0 && x < sizeInChunks && z > 0 && z < sizeInChunks;
        }

        public bool IsBlockInWorld(Vector3Int pos) {
            const int maxBlocks = VoxelData.WorldSizeInBlocks;
            const int chunkHeight = VoxelData.ChunkHeight;
            var x = pos.x;
            var y = pos.y;
            var z = pos.z;
            return x is >= 0 and < maxBlocks && y is >= 0 and < chunkHeight && z is >= 0 and < maxBlocks;
        }

        public bool CheckForBlock(Vector3Int pos) {
            var block = GetBlock(pos);
            return block != null && block.IsSolid();
        }

    }

}