using UnityEngine;
using World.Biome;

namespace World.Chunk {
    public static class Noise {

        private static readonly float SeedX = Random.value;
        private static readonly float SeedY = Random.value;
        private static readonly float SeedZ = Random.value;

        public static float Get2DPerlin(Vector2 position, float offset, float scale) {
            var x = (position.x + 0.1f) / 16 * scale + offset + SeedX;
            var y = (position.y + 0.1f) / 16 * scale + offset + SeedZ;
            return Mathf.PerlinNoise(x, y);
        }

        public static bool Get3DPerlin(Vector3 position, Lode lode) {
            return Get3DPerlin(position, lode.noiseOffset, lode.scale, lode.threshold);
        }

        public static bool Get3DPerlin(Vector3 position, float offset, float scale, float threshold) {
            // no longer a space... fuck off Sparky
            var x = (position.x + offset + 0.1f) * scale + SeedX;
            var y = (position.y + offset + 0.1f) * scale + SeedY;
            var z = (position.z + offset + 0.1f) * scale + SeedZ;

            var AB = Mathf.PerlinNoise(x, y);
            var BC = Mathf.PerlinNoise(y, z);
            var AC = Mathf.PerlinNoise(x, z);
            var BA = Mathf.PerlinNoise(y, x);
            var CB = Mathf.PerlinNoise(z, y);
            var CA = Mathf.PerlinNoise(z, x);

            return (AB + BC + AC + BA + CB + CA) / 6f > threshold;
        }
        
    }
    
}