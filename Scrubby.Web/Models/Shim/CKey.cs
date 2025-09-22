using System;
using System.Text.RegularExpressions;

namespace Scrubby.Web.Models.Shim;

public class CKey : IEquatable<CKey>, IComparable<CKey>
{
    public CKey(string raw)
    {
        Raw = raw;
        Cleaned = Clean(raw);
    }

    public string Raw { get; }
    public string Cleaned { get; }

    public int CompareTo(CKey other) => Cleaned.CompareTo(other.Cleaned);

    public bool Equals(CKey other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        if (GetType() != other.GetType()) return false;

        return Cleaned.Equals(other.Cleaned);
    }

    public static string Clean(string raw)
    {
        var badchars = @"[^A-Za-z0-9]";

        if (raw != null)
            return Regex.Replace(raw, badchars, "").ToLower();
        return null;
    }

    public override bool Equals(object obj) => Equals(obj as CKey);

    public static bool operator ==(CKey lhs, CKey rhs)
    {
        if (lhs is null) return rhs is null;

        return lhs.Equals(rhs);
    }

    public static bool operator !=(CKey lhs, CKey rhs) => !(lhs == rhs);

    public override string ToString() => Cleaned;

    public override int GetHashCode() => Cleaned.GetHashCode();

    public static implicit operator CKey(string input) => new(input);
}