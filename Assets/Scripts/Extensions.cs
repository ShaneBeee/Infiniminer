using UnityEngine;

internal static class Extensions {

    public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 0) {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++) {
            multiplier *= 10f;
        }

        return new Vector3(
            Mathf.Round(vector3.x * multiplier) / multiplier,
            Mathf.Round(vector3.y * multiplier) / multiplier,
            Mathf.Round(vector3.z * multiplier) / multiplier);
    }

    /// <summary>
    /// Convert to a Vector3Int
    /// </summary>
    /// <param name="vector3"></param>
    /// <returns></returns>
    public static Vector3Int ToVector3Int(this Vector3 vector3) {
        var x = Mathf.FloorToInt(vector3.x);
        var y = Mathf.FloorToInt(vector3.y + 0.0001f);
        var z = Mathf.FloorToInt(vector3.z);
        return new Vector3Int(x, y, z);
    }

}