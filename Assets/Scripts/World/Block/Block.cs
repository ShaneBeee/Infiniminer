namespace World.Block {
    public class Block {

        private readonly string key;
        private readonly bool solid;
        private readonly byte[] textureIds = new byte[6];

        internal Block(string key, bool solid, byte textureId) {
            this.key = key;
            this.solid = solid;
            for (int i = 0; i < 6; i++) {
                this.textureIds[i] = textureId;
            }
        }

        internal Block(string key, bool solid, byte topId, byte bottomId, byte sideId) {
            this.key = key;
            this.solid = solid;
            this.textureIds[(int)VoxelData.Side.TOP] = topId;
            this.textureIds[(int)VoxelData.Side.BOTTOM] = bottomId;
            this.textureIds[(int)VoxelData.Side.BACK] = sideId;
            this.textureIds[(int)VoxelData.Side.FRONT] = sideId;
            this.textureIds[(int)VoxelData.Side.LEFT] = sideId;
            this.textureIds[(int)VoxelData.Side.RIGHT] = sideId;
        }

        public string GetKey() {
            return this.key;
        }

        public bool IsSolid() {
            return this.solid;
        }

        public byte GetTextureId(VoxelData.Side side) {
            return this.textureIds[(int)side];
        }

    }

}