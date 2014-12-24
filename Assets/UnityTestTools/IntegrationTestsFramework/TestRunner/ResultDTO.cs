using System;
using System.Collections.Generic;
using UnityEngine;
using UnityTestTools.Common;

namespace UnityTestTools.IntegrationTestsFramework.TestRunner
{
    [Serializable]
    public class ResultDTO
    {
        public MessageType messageType;
        public int levelCount;
        public int loadedLevel;
        public string loadedLevelName;
        public string testName;
        public float testTimeout;
        public ITestResult testResult;

        private ResultDTO(MessageType messageType)
        {
            this.messageType = messageType;
            levelCount = Application.levelCount;
            loadedLevel = Application.loadedLevel;
            loadedLevelName = Application.loadedLevelName;
        }

        public enum MessageType
        {
            Ping,
            RunStarted,
            RunFinished,
            TestStarted,
            TestFinished,
            RunInterrupted
        }

        public static ResultDTO CreatePing()
        {
            var dto = new ResultDTO(MessageType.Ping);
            return dto;
        }

        public static ResultDTO CreateRunStarted()
        {
            var dto = new ResultDTO(MessageType.RunStarted);
            return dto;
        }

        public static ResultDTO CreateRunFinished(List<TestResult> testResults)
        {
            var dto = new ResultDTO(MessageType.RunFinished);
            return dto;
        }

        public static ResultDTO CreateTestStarted(TestResult test)
        {
            var dto = new ResultDTO(MessageType.TestStarted);
            dto.testName = test.FullName;
            dto.testTimeout = test.TestComponent.timeout;
            return dto;
        }

        public static ResultDTO CreateTestFinished(TestResult test)
        {
            var dto = new ResultDTO(MessageType.TestFinished);
            dto.testName = test.FullName;
            dto.testResult = GetSerializableTestResult(test);
            return dto;
        }

        private static ITestResult GetSerializableTestResult(TestResult test)
        {
            var str = new SerializableTestResult();

            str.resultState = test.ResultState;
            str.message = test.messages;
            str.executed = test.Executed;
            str.name = test.Name;
            str.fullName = test.FullName;
            str.id = test.id;
            str.isSuccess = test.IsSuccess;
            str.duration = test.duration;
            str.stackTrace = test.stacktrace;

            return str;
        }
    }

    #region SerializableTestResult

    #endregion
}
