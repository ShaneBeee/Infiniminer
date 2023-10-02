using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using World.Block;

namespace World {
    public class World : MonoBehaviour {

        [SerializeField] private int seed;
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject loadingScreenCanvas;
        [SerializeField] private GameObject debugScreenCanvas;
        [SerializeField] private GameObject menuScreenCanvas;
        [SerializeField] private Slider slider;

        public Material material;
        public PhysicsMaterial2D physicsMaterial;
        public Block.Block[] BlockTypes;

        private Text debugText;


        private readonly Chunk.Chunk[,] _chunks =
            new Chunk.Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];

        private int activeChunks;

        private void Start() {
            Random.InitState(seed);
            loadingScreenCanvas.SetActive(true);
            debugScreenCanvas.SetActive(false);
            menuScreenCanvas.SetActive(false);
            debugText = debugScreenCanvas.GetComponentInChildren<Text>();
            if (debugText == null) {
                Debug.LogError("Debug text == null");
            }

            StartCoroutine(GenerateChunks());
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

        public int Seed => seed;
        private Vector3 spawnPosition;

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

            foreach (var chunk in _chunks) {
                slider.value = (float)loadedChunks++ / chunkCount;
                chunk.chunkRenderer.RenderChunk();
                yield return new WaitForSeconds(0.001f);
            }

            StartCoroutine(SpawnInPlayer());
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
            _chunks[x, z] = new Chunk.Chunk(x, z, this);
        }

        private void SetSpawn() {
            var spawn = new Vector3(16, 0, 16);
            for (int i = VoxelData.ChunkHeight - 1; i > 0; i--) {
                spawn.y = i;
                if (GetBlock(spawn) == Blocks.AIR) continue;
                spawn.y += 1.5f;
                spawnPosition = spawn;
                return;
            }
        }

        public Vector3 GetSpawnPosition() {
            return spawnPosition;
        }

        public Chunk.Chunk GetChunk(int x, int z) {
            if (x >= _chunks.GetLength(0) || z >= _chunks.GetLength(1)) {
                return null;
            }

            if (x < 0 || z < 0) {
                return null;
            }

            return _chunks[x, z];
        }

        public Block.Block GetBlock(Vector3 pos) {
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

        public void SetBlock(Vector3 pos, Block.Block block) {
            var chunkX = Mathf.FloorToInt(pos.x / 16);
            var chunkZ = Mathf.FloorToInt(pos.z / 16);
            var chunk = GetChunk(chunkX, chunkZ);
            if (chunk == null) {
                return;
            }
            var x = Mathf.FloorToInt(pos.x) % 16;
            var y = Mathf.FloorToInt(pos.y);
            var z = Mathf.FloorToInt(pos.z) % 16;
            chunk.SetBlock(new Vector3(x,y,z), block);
        }

        private bool IsChunkInWorld(int x, int z) {
            var sizeInChunks = VoxelData.WorldSizeInChunks - 1;
            if (x > 0 && x < sizeInChunks && z > 0 && z < sizeInChunks) {
                return true;
            }

            return false;
        }

        public bool IsBlockInWorld(Vector3 pos) {
            var maxBlocks = VoxelData.WorldSizeInBlocks;
            var chunkHeight = VoxelData.ChunkHeight;
            var x = pos.x;
            var y = pos.y;
            var z = pos.z;
            if (x >= 0 && x < maxBlocks && y >= 0 && y < chunkHeight && z >= 0 && z < maxBlocks) {
                return true;
            }

            return false;
        }

        public bool CheckForBlock(Vector3 pos) {
            return CheckForBlock(pos.x, pos.y, pos.z);
        }

        public bool CheckForBlock(float x, float y, float z) {
            var block = GetBlock(new Vector3(x, y, z));
            return block != null && block.IsSolid();
        }

    }

}