using UnityEngine;
using World.Block;

namespace World.Chunk {
    public static class ChunkGenerator {

        internal static void PopulateBlocks(Chunk chunk) {
            var coord = chunk.Coord;
            var chunkWorldX = coord.GetX() * 16;
            var chunkWorldZ = coord.GetZ() * 16;

            for (var y = 0; y < VoxelData.ChunkHeight; y++) {
                for (var x = 0; x < 16; x++) {
                    for (var z = 0; z < 16; z++) {

                        var biome = Biome.Biomes.DEFAULT;
                        var block = Blocks.ROCK;

                        var biomeTerrainHeight = biome.Properties.GetTerrainHeight();
                        var biomeTerrainScale = biome.Properties.GetTerrainScale();
                        var biomeSolidHeight = biome.Properties.GetSolidGroundHeight();

                        /* INITIAL PASS */

                        var pos = new Vector2(x + chunkWorldX, z + chunkWorldZ);
                        var terrainHeight =
                            Mathf.FloorToInt(biomeTerrainHeight * Noise.Get2DPerlin(pos, 0, biomeTerrainScale)) +
                            biomeSolidHeight;

                        if (y == terrainHeight) {
                            block = Blocks.DIRT;
                        } else if (y < terrainHeight && y > terrainHeight - 4) {
                            block = Blocks.DIRT;
                        } else if (y > terrainHeight) {
                            block = Blocks.AIR;
                        }

                        /* SECOND PASS */

                        var pos3 = new Vector3(x + chunkWorldX, y, z + chunkWorldZ);
                        if (block == Blocks.ROCK) {
                            foreach (var lode in biome.lodes) {
                                if (y > lode.minHeight && y < lode.maxHeight) {
                                    if (Noise.Get3DPerlin(pos3, lode)) {
                                        block = lode.Block;
                                    }
                                }
                            }
                        }

                        chunk.SetBlockType(x, y, z, block);
                    }
                }
            }
        }

    }

}