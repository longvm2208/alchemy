using System;

public readonly struct MergeKey : IEquatable<MergeKey>
{
    public readonly ItemId A;
    public readonly ItemId B;

    public MergeKey(ItemId a, ItemId b)
    {
        if (a <= b)
        {
            A = a;
            B = b;
        }
        else
        {
            A = b;
            B = a;
        }
    }

    public bool Equals(MergeKey other)
    {
        return A == other.A && B == other.B;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)A, (int)B);
    }
}