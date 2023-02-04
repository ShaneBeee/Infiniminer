namespace World.Chunk {
    
    public class ChunkCoord {
        private int _x;
        private int _z;

        public ChunkCoord(int x, int z) {
            _x = x;
            _z = z;
        }

        public int GetX() {
            return _x;
        }

        public int GetZ() {
            return _z;
        }
        
    }
    
}