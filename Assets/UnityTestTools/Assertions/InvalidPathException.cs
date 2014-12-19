using System;

namespace Assets.UnityTestTools.Assertions
{
    public class InvalidPathException : Exception
    {
        public InvalidPathException(string path)
            : base("Invalid path part " + path)
        {
        }
    }
}
