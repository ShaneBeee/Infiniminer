using System.Diagnostics.CodeAnalysis;

namespace World.Block {

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Blocks {

        public static readonly Block AIR = get("air", false, 0, 0);
        public static readonly Block DIRT = get("dirt", true, 1, 2);
        public static readonly Block ROCK = get("rock", true, 2, 1);
        public static readonly Block DIAMOND = get("diamond", true, 3, 0);
        public static readonly Block TNT = get("tnt", true, 4, 3);

        private static Block get(string name, bool solid, byte id, byte textureID) {
            return new Block(name, solid, id, new[] {textureID});
        }
        

    }

}