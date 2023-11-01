using System;

namespace Scrubby.Web.Models.Shim;

public class SSVec : IEquatable<SSVec>
{
    public SSVec(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public bool Equals(SSVec other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }
}