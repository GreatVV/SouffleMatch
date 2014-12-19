using System;

namespace Assets.UnityTestTools.Assertions.Comparers
{
    [Serializable]
    public abstract class ComparerBaseGeneric<T> : ComparerBaseGeneric<T, T>
    {
    }
}