using System;
using UnityEngine;

namespace Assets.UnityTestTools.IntegrationTestsFramework.TestRunner
{
    public interface ITestComponent : IComparable<ITestComponent>
    {
        void EnableTest(bool enable);
        bool IsTestGroup();
        GameObject gameObject { get; }
        string Name { get; }
        ITestComponent GetTestGroup();
        bool IsExceptionExpected(string exceptionType);
        bool ShouldSucceedOnException();
        double GetTimeout();
        bool IsIgnored();
        bool ShouldSucceedOnAssertions();
        bool IsExludedOnThisPlatform();
    }
}