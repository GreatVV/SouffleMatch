using System;

namespace Assets.UnityTestTools.Assertions.Comparers
{
    public abstract class ActionBaseGeneric<T> : ActionBase
    {
        protected override bool Compare(object objVal)
        {
            return Compare((T)objVal);
        }
        protected abstract bool Compare(T objVal);

        public override Type[] GetAccepatbleTypesForA()
        {
            return new[] { typeof(T) };
        }

        public override Type GetParameterType()
        {
            return typeof(T);
        }
        protected override bool UseCache { get { return true; } }
    }
}