using System;

public readonly struct CategoryKey : IEquatable<CategoryKey>
{
    public readonly GroupId Group;
    public readonly BranchId Branch;

    public CategoryKey(GroupId group, BranchId branch)
    {
        Group = group;
        Branch = branch;
    }

    public bool Equals(CategoryKey other)
    {
        return Group == other.Group && Branch == other.Branch;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)Group, (int)Branch);
    }
}