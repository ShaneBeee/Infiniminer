using System.Diagnostics.CodeAnalysis;

namespace World.Block {

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class Blocks {

        public static readonly Block AIR = register("air", false, 0);
        public static readonly Block DIRT = register("dirt", true, 2);
        public static readonly Block ROCK = register("rock", true, 1);
        public static readonly Block DIAMOND = register("diamond", true, 0);
        public static readonly Block TNT = register("tnt", true, 3);
        public static readonly Block RANDO = register("rando", true, 0, 2, 1);

        private static Block register(string name, bool solid, byte textureID) {
            return new Block(name, solid, textureID);
        }

        private static Block register(string name, bool solid, byte topId, byte bottomId, byte sideId) {
            return new Block(name, solid, topId, bottomId, sideId);
        }

    }

}