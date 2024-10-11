using System.Diagnostics.CodeAnalysis;

namespace World.Biome {
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class OreBlob {

        public string blobName { get; }
        public Block.Block Block { get; }
        public int minHeight { get; private set; }

        public int maxHeight { get; private set; } = VoxelData.chunkHeight;
        public float scale { get; private set; } = 0.5f;
        public float threshold { get; private set; }
        public float noiseOffset { get; private set; }

        public OreBlob(string blobName, Block.Block block) {
            this.blobName = blobName;
            Block = block;
        }

        public OreBlob MinHeight(int height) {
            this.minHeight = height;
            return this;
        }

        public OreBlob MaxHeight(int height) {
            this.maxHeight = height;
            return this;
        }

        public OreBlob Scale(float scale) {
            this.scale = scale;
            return this;
        }

        public OreBlob Threshold(float threshold) {
            this.threshold = threshold;
            return this;
        }

        public OreBlob NoiseOffset(float offset) {
            this.noiseOffset = offset;
            return this;
        }

    }
    
}