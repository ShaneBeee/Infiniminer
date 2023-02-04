namespace World.Biome {
    
    public class Lode {

        public string nodeName { get; }
        public Block.Block Block { get; }
        public int minHeight { get; private set; } = 0;

        public int maxHeight { get; private set; } = VoxelData.ChunkHeight;
        public float scale { get; private set; } = 0.5f;
        public float threshold { get; private set; } = 0.0f;
        public float noiseOffset { get; private set; } = 0.0f;

        public Lode(string nodeName, Block.Block block) {
            this.nodeName = nodeName;
            Block = block;
        }

        public Lode MinHeight(int height) {
            minHeight = height;
            return this;
        }

        public Lode MaxHeight(int height) {
            maxHeight = height;
            return this;
        }

        public Lode Scale(float scale) {
            this.scale = scale;
            return this;
        }

        public Lode Threshold(float threshold) {
            this.threshold = threshold;
            return this;
        }

        public Lode NoiseOffset(float offset) {
            noiseOffset = offset;
            return this;
        }

    }
    
}