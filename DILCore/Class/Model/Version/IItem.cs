using System;

namespace DILCore.Class.Model.Version;

public interface IItem : IComparable
{
    bool IsNull();
}