using System;
using System.Collections.Generic;

namespace World.Biome {

    public class Biome {

        private string biomeName;
        public BiomeProperties Properties { get; }
        public OreBlob[] lodes = Array.Empty<OreBlob>();

        public Biome(string name, BiomeProperties properties) {
            biomeName = name;
            Properties = properties;
        }

        public Biome AddOreBlob(OreBlob oreBlob) {
            var lodeList = new List<OreBlob>(lodes) { oreBlob };
            lodes = lodeList.ToArray();
            return this;
        }

    }

}