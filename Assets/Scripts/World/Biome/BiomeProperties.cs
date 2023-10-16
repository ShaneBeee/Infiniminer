using World.Block;

namespace World.Biome {
    public class BiomeProperties {

        private int _solidGroundHeight = 32; // below this value is always solid ground (think SeaLevel)
        private int _terrainHeight = 64; // solid to highest ground
        private float _terrainScale = 0.1f;
        private Block.Block groundBlock = Blocks.DIRT;

        internal BiomeProperties() {
        }

        public BiomeProperties SolidGroundHeight(int height) {
            _solidGroundHeight = height;
            return this;
        }

        public int GetSolidGroundHeight() {
            return _solidGroundHeight;
        }

        public BiomeProperties TerrainHeight(int height) {
            _terrainHeight = height;
            return this;
        }

        public int GetTerrainHeight() {
            return _terrainHeight;
        }

        public BiomeProperties TerrainScale(float scale) {
            _terrainScale = scale;
            return this;
        }

        public float GetTerrainScale() {
            return _terrainScale;
        }

        public BiomeProperties GroundBlock(Block.Block block) {
            this.groundBlock = block;
            return this;
        }

        public Block.Block GetGroundBlock() {
            return this.groundBlock;
        }
        
    }
    
}