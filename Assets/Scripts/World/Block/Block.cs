namespace World.Block {
    public class Block {
        private readonly string _blockName;
        private readonly bool _solid;
        private readonly byte _id;
        private readonly byte[] _textureId;

        internal Block(string blockName, bool solid, byte id, byte[] textureId) {
            _blockName = blockName;
            _solid = solid;
            _id = id;
            _textureId = textureId;
        }

        public string GetBlockName() {
            return _blockName;
        }

        public bool IsSolid() {
            return _solid;
        }

        public byte GetId() {
            return _id;
        }

        public byte GetTextureId() {
            return _textureId[0];
        }
        
    }

}