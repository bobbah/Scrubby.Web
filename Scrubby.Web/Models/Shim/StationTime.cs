using System;

namespace Scrubby.Web.Models.Shim;

public class StationTime : IEquatable<StationTime>, IComparable<StationTime>
{
    public int _Seconds;
    public int Seconds => _Seconds % 60;
    public int Minutes => _Seconds / 60 % 60;
    public int Hours => _Seconds / 3600 % 24;

    public int CompareTo(StationTime other) => _Seconds.CompareTo(other._Seconds);

    public bool Equals(StationTime other) => _Seconds == other._Seconds;

    public override int GetHashCode() => _Seconds.GetHashCode();

    public static bool operator ==(StationTime left, StationTime right)
    {
        if (left is null) return right is null;

        return left.Equals(right);
    }

    public static bool operator !=(StationTime left, StationTime right) => !(left == right);

    public override string ToString() => $"{Hours: 00}:{Minutes: 00}:{Seconds: 00}";

    public override bool Equals(object obj) => Equals(obj as StationTime);
}