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

        public static bool Get3DPerlin(Vector3 position, OreBlob oreBlob) {
            return Get3DPerlin(position, oreBlob.noiseOffset, oreBlob.scale, oreBlob.threshold);
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

        public static float[,] GenerateNoiseMap(int worldSize, float scale, int octaves, float persistence, float lacunarity) {
            var mapSize = worldSize * 16;
            var noiseMap = new float[mapSize, mapSize];

            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++) {
                float offsetX = Random.Range(-100000, 100000);
                float offsetY = Random.Range(-100000, 100000);
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }
            
            if (scale <= 0) scale = 0.0001f;

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            for (int x = 0; x < mapSize; x++) {
                for (int y = 0; y < mapSize; y++) {

                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;
                    
                    for (int i = 0; i < octaves; i++) {
                        float sampleX = x / scale * frequency + octaveOffsets[i].x;
                        float sampleY = y / scale * frequency + octaveOffsets[i].y;

                        float perlinVal = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinVal * amplitude;

                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }

                    if (noiseHeight > maxNoiseHeight) {
                        maxNoiseHeight = noiseHeight;
                    } else if (noiseHeight < minNoiseHeight) {
                        minNoiseHeight = noiseHeight;
                    }
                    noiseMap[x, y] = noiseHeight;
                }
            }

            for (int x = 0; x < mapSize; x++) {
                for (int y = 0; y < mapSize; y++) {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
            }

            return noiseMap;
        }
        
    }
    
}