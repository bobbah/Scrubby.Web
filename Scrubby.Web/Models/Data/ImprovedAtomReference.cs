using System.Text;
using Scrubby.Web.Models.Shim;

namespace Scrubby.Web.Models.Data;

public class ImprovedAtomReference
{
    public string Name { get; set; }
    public string TypePath { get; set; }
    public int? X { get; set; }
    public int? Y { get; set; }
    public int? Z { get; set; }
    public SSVec LocationVec => X.HasValue && Y.HasValue && Z.HasValue ? new SSVec(X.Value, Y.Value, Z.Value) : null;

    public static ImprovedAtomReference FromByondAtom(ByondAtom source)
    {
        if (source == null)
            return null;

        return new ImprovedAtomReference
        {
            Name = source.Name,
            TypePath = source.TypePath,
            X = source.Location?.X,
            Y = source.Location?.Y,
            Z = source.Location?.Z
        };
    }

    public override string ToString()
    {
        var result = new StringBuilder(Name);
        if (TypePath != null)
            result.Append($" ({TypePath})");
        if (LocationVec != null)
            result.Append($" {LocationVec}");
        return result.ToString().Trim();
    }
}