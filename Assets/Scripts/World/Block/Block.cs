namespace World.Block {
    public class Block {
        private readonly string key;
        private readonly bool solid;
        private readonly byte[] textureId;

        internal Block(string key, bool solid, byte[] textureId) {
            this.key = key;
            this.solid = solid;
            this.textureId = textureId;
        }

        public string GetKey() {
            return key;
        }

        public bool IsSolid() {
            return solid;
        }
        
        public byte GetTextureId() {
            return textureId[0];
        }
        
    }

}