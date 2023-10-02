using World.Block;

namespace World.Biome {

    public static class Biomes {

        public static readonly Biome DEFAULT = GetDefault();

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
    }

}