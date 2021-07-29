using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NilanToolKit.HexTileMap {

/// <summary>
/// A easier system for unity hexagon tilemap
/// some code is references by https://www.redblobgames.com/grids/hexagons/
/// </summary>
public struct HexVector : IEquatable<HexVector> {

    public int x;
    public int y;

    public bool IsSingular => y % 2 == 1;

    public HexVector left => DirectionBy(HexDir.Left);

    public HexVector leftTop => DirectionBy(HexDir.LeftTop);

    public HexVector rightTop => DirectionBy(HexDir.RightTop);

    public HexVector right => DirectionBy(HexDir.Right);

    public HexVector rightBottom => DirectionBy(HexDir.RightBottom);

    public HexVector leftBottom => DirectionBy(HexDir.LeftBottom);

    public int magnitude => Distance(zero, this);

    public HexVector(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public HexVector DirectionBy(HexDir dir) {
        return this + GetDirection(IsSingular, dir);
    }

    public IEnumerable<HexVector> Ring(int radius) {
        return HexRing(this, radius);
    }

    public IEnumerable<HexVector> SpiralRing(int radius) {
        return HexSpiralRing(this, radius);
    }

    public static HexVector zero = new HexVector();
    
    public static HexVector operator +(HexVector a, HexVector b) {
        return new HexVector(a.x + b.x, a.y + b.y);
    }

    public static HexVector operator -(HexVector a, HexVector b) {
        return new HexVector(a.x - b.x, a.y - b.y);
    }

    public static bool operator ==(HexVector a, HexVector b) {
        return a.Equals(b);
    }

    public static bool operator !=(HexVector a, HexVector b) {
        return !(a == b);
    }

    public override bool Equals(object obj) {
        return obj is HexVector vector && Equals(vector);
    }
    
    public bool Equals(HexVector other) {
        return x == other.x && y == other.y;
    }

    public override int GetHashCode() {
        return x.GetHashCode() ^ y.GetHashCode() << 2;// copy from UnityEngine.Vector2
    }

    public override string ToString() {
        return $"({x},{y})";
    }

    public static implicit operator Vector2Int(HexVector vec) {
        return new Vector2Int(vec.x, vec.y);
    }

    public static implicit operator HexVector(Vector2Int vec) {
        return new HexVector(vec.x, vec.y);
    }

    public static implicit operator Vector3Int(HexVector vec) {
        return new Vector3Int(vec.x, vec.y, 0);
    }

    public static implicit operator HexVector(Vector3Int vec) {
        return new HexVector(vec.x, vec.y);
    }

    #region Utils
    
    /// <summary>
    /// convert to cube coordinates
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public static Vector3Int ToCube(HexVector hex) {
        var x = hex.x - (hex.y - (hex.y & 1)) / 2;
        var z = hex.y;
        var y = -x - z;
        return new Vector3Int(x, y, z);
    }
    
    /// <summary>
    /// convert to unity hex coordinates
    /// </summary>
    /// <param name="cube"></param>
    /// <returns></returns>
    public static HexVector FromCube(Vector3Int cube) {
        var col = cube.x + (cube.z - (cube.z & 1)) / 2;
        var row = cube.z;
        return new HexVector(col, row);
    }

    public static int Distance(HexVector a, HexVector b) {
        var aC = ToCube(a);
        var bC = ToCube(b);
        var delta = bC - aC;
        return Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y), Mathf.Abs(delta.z));
    }

    public static HexVector GetDirection(bool isSingular, HexDir dir) {
        if (isSingular) {
            switch (dir) {
                case HexDir.Left: return new HexVector(-1, 0);
                case HexDir.LeftTop: return new HexVector(0, 1);
                case HexDir.RightTop: return new HexVector(1, 1);
                case HexDir.Right: return new HexVector(1, 0);
                case HexDir.RightBottom: return new HexVector(1, -1);
                case HexDir.LeftBottom: return new HexVector(0, -1);
                default: throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }
        }
        else {
            switch (dir) {
                case HexDir.Left: return new HexVector(-1, 0);
                case HexDir.LeftTop: return new HexVector(-1,1);
                case HexDir.RightTop: return new HexVector(0,1);
                case HexDir.Right: return new HexVector(1,0);
                case HexDir.RightBottom: return new HexVector(0,-1);
                case HexDir.LeftBottom: return new HexVector(-1, -1);
                default: throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }
        }
    }

    public static IEnumerable<HexVector> HexSpiralRing(HexVector center,int radius) {
        var lst = new List<HexVector>();
        lst.Add(center);
        for (int i = 1; i <= radius; i++) {
            var ring = HexRing(center, i);
            lst.AddRange(ring);
        }
        return lst;
    }

    public static IEnumerable<HexVector> HexRing(HexVector center, int radius) {
        var cubeRing = CubeRing(ToCube(center), radius);
        return cubeRing.Select(FromCube);
    }
    
    public static readonly Vector3Int[] CubeDirs = {
        new Vector3Int(+1, -1, 0), new Vector3Int(+1, 0, -1), new Vector3Int(0, +1, -1),
        new Vector3Int(-1, +1, 0), new Vector3Int(-1, 0, +1), new Vector3Int(0, -1, +1),
    };

    public static IEnumerable<Vector3Int> CubeRing(Vector3Int center, int radius) {
        if (radius == 0) {
            return new[] {center};
        }
        var results = new Vector3Int[radius * 6];
        var dir = CubeDirs[4];
        var cube = center + (dir * radius);
        int index = 0;
        for (int i = 0; i < 6; i++) {
            for (int j = 0; j < radius; j++) {
                results[index] = cube;
                index++;
                cube += CubeDirs[i];
            }
        }
        return results;
    }

    #endregion

}
}