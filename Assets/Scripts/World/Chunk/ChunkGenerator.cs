using UnityEngine;
using Util;
using World.Biome;
using World.Block;

namespace World.Chunk {
    public class ChunkGenerator {
        private static FastNoiseLite noiseGenerator;
        private readonly Chunk chunk;
        private const float scale = 10;

        public ChunkGenerator(Chunk chunk) {
            this.chunk = chunk;
            if (noiseGenerator != null) return;
            noiseGenerator = new FastNoiseLite(chunk.World.GetSeed());
            noiseGenerator.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            noiseGenerator.SetFractalType(FastNoiseLite.FractalType.FBm);
            noiseGenerator.SetFractalOctaves(2);
            noiseGenerator.SetFrequency(0.1f);
            noiseGenerator.SetFractalLacunarity(5);
        }

        public void PopulateBlocks() {
            var coord = chunk.Coord;
            var chunkWorldX = coord.GetX() * 16;
            var chunkWorldZ = coord.GetZ() * 16;


            var biome = Biomes.DEFAULT;

            for (var y = 0; y < VoxelData.chunkHeight; y++) {
                for (var x = 0; x < 16; x++) {
                    for (var z = 0; z < 16; z++) {
                        var block = Blocks.ROCK;

                        /* INITIAL PASS - SURFACE */

                        var terrainHeight = GetTerrainHeight(x, z, biome);

                        if (y == terrainHeight) {
                            block = biome.Properties.GetGroundBlock();
                        } else if (y < terrainHeight && y > terrainHeight - 4) {
                            block = Blocks.DIRT;
                        } else if (y > terrainHeight) {
                            block = Blocks.AIR;
                        }

                        /* SECONDARY PASS - ORE BLOBS */

                        var worldPos = new Vector3(x + chunkWorldX, y, z + chunkWorldZ);
                        if (block == Blocks.ROCK) {
                            foreach (var oreBlob in biome.lodes) {
                                if (y <= oreBlob.minHeight || y >= oreBlob.maxHeight) continue;
                                if (Noise.Get3DPerlin(worldPos, oreBlob)) {
                                    block = oreBlob.Block;
                                }
                            }
                        }

                        chunk.SetBlockType(x, y, z, block);
                    }
                }
            }
        }

        private int GetTerrainHeight(int x, int z, Biome.Biome biome) {
            var chunkWorldX = chunk.Coord.GetX() * 16;
            var chunkWorldZ = chunk.Coord.GetZ() * 16;
            var biomeTerrainHeight = biome.Properties.GetTerrainHeight();
            var biomeSolidHeight = biome.Properties.GetSolidGroundHeight();
            var noise = (noiseGenerator.GetNoise((x + chunkWorldX)/ scale, (z + chunkWorldZ) / scale) + 1) * biomeTerrainHeight;
            return Mathf.FloorToInt(noise) + biomeSolidHeight;
        }
    }
}