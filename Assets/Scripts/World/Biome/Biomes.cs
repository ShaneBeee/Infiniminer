using System.Diagnostics.CodeAnalysis;
using World.Block;

namespace World.Biome {

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Biomes {

        public static readonly Biome DEFAULT = GetDefault();
        public static readonly Biome TNT = GetTNTBiome();
        public static readonly Biome STONE = GetStoneBiome();

        private static Biome GetDefault() {
            return new Biome("default",
                    new BiomeProperties()
                        .SolidGroundHeight(42)
                        .TerrainHeight(12)
                        .TerrainScale(0.25f))
                .AddOreBlob(new OreBlob("diamond", Blocks.DIAMOND)
                    .MinHeight(10)
                    .MaxHeight(20)
                    .Scale(0.1f)
                    .Threshold(0.5f)
                    .NoiseOffset(0f)
                );
        }

        private static Biome GetTNTBiome() {
            return new Biome("tnt",
                    new BiomeProperties()
                        .SolidGroundHeight(42)
                        .TerrainHeight(12)
                        .TerrainScale(0.25f)
                        .GroundBlock(Blocks.TNT))
                .AddOreBlob(new OreBlob("diamond", Blocks.DIAMOND)
                    .MinHeight(10)
                    .MaxHeight(20)
                    .Scale(0.1f)
                    .Threshold(0.5f)
                    .NoiseOffset(0f)
                );
        }
        
        private static Biome GetStoneBiome() {
            return new Biome("stone",
                    new BiomeProperties()
                        .SolidGroundHeight(42)
                        .TerrainHeight(12)
                        .TerrainScale(0.25f)
                        .GroundBlock(Blocks.ROCK))
                .AddOreBlob(new OreBlob("diamond", Blocks.DIAMOND)
                    .MinHeight(10)
                    .MaxHeight(20)
                    .Scale(0.1f)
                    .Threshold(0.5f)
                    .NoiseOffset(0f)
                );
        }
    }

}