using System.Diagnostics.CodeAnalysis;

namespace World.Block {

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class Blocks {

        public static readonly Block AIR = get("air", false, 0);
        public static readonly Block DIRT = get("dirt", true, 2);
        public static readonly Block ROCK = get("rock", true, 1);
        public static readonly Block DIAMOND = get("diamond", true, 0);
        public static readonly Block TNT = get("tnt", true, 3);

        private static Block get(string name, bool solid, byte textureID) {
            return new Block(name, solid, new[] { textureID });
        }

    }

}