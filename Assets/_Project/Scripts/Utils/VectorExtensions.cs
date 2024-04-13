using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class VectorExtensions
{
    public static Vector2 RotateBy(this Vector2 v, float a, bool bUseRadians = false)
    {
        if (!bUseRadians) a *= Mathf.Deg2Rad;
        var ca = System.Math.Cos(a);
        var sa = System.Math.Sin(a);
        var rx = v.x * ca - v.y * sa;

        return new Vector2((float)rx, (float)(v.x * sa + v.y * ca));
    }

    public static Vector2 ToXZ(this Vector3 v) => new(v.x, v.z);
    public static Vector2 ToXY(this Vector3 v) => new(v.x, v.y);
    public static Vector2 ToYZ(this Vector3 v) => new(v.y, v.z);

    public static Vector3 SetX(this Vector3 v, float x)
    {
        v.x = x;
        return v;
    }

    public static Vector3 SetY(this Vector3 v, float y)
    {
        v.y = y;
        return v;
    }

    public static Vector3 SetZ(this Vector3 v, float z)
    {
        v.z = z;
        return v;
    }

    public static Vector3Int SetZ(this Vector3Int v, int z)
    {
        v.z = z;
        return v;
    }
        public static Vector3Int SetX(this Vector3Int v, int x)
    {
        v.x = x;
        return v;
    }

    public static Vector3Int SetY(this Vector3Int v, int y)
    {
        v.y = y;
        return v;
    }

    public static Vector2Int ToVector2Int(this Vector3Int v) => new Vector2Int(v.x, v.y);

    public static Vector3Int ToVector3Int(this Vector2Int v) => new Vector3Int(v.x, v.y, 0);

    public static Vector3 ToVector3(this Vector2 v) => new Vector3(v.x, v.y, 0);

    public static Vector3 ToVector3(this Vector2Int v) => new Vector3(v.x, v.y, 0);


    public static Color SetIntensity(this Color c, float intensity)
    {
        // linear space
        return c * Mathf.Pow(2.0F, intensity);
    }
}
