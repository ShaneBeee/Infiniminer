using System.Collections.Generic;

namespace World.Biome {

    public class Biome {

        private string biomeName;
        public BiomeProperties Properties { get; }
        public Lode[] lodes = new Lode[0];

        public Biome(string name, BiomeProperties properties) {
            biomeName = name;
            Properties = properties;
        }

        public Biome AddLode(Lode lode) {
            var lodeList = new List<Lode>(lodes) {lode};
            lodes = lodeList.ToArray();
            return this;
        }

    }

}