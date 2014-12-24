using System;

namespace UnityTestTools.Assertions.Comparers
{
    [Serializable]
    public abstract class ComparerBaseGeneric<T> : ComparerBaseGeneric<T, T>
    {
    }
}