namespace UnityTestTools.Assertions
{
    public interface IAssertionComponentConfigurator
    {
        /// <summary>
        /// If the assertion is evaluated in Update, after how many frame should the evaluation start. Deafult is 1 (first frame)
        /// </summary>
        int UpdateCheckStartOnFrame { set; }
        /// <summary>
        /// If the assertion is evaluated in Update and UpdateCheckRepeat is true, how many frame should pass between evaluations
        /// </summary>
        int UpdateCheckRepeatFrequency { set; }
        /// <summary>
        /// If the assertion is evaluated in Update, should the evaluation be repeated after UpdateCheckRepeatFrequency frames
        /// </summary>
        bool UpdateCheckRepeat { set; }

        /// <summary>
        /// If the assertion is evaluated after a period of time, after how many seconds the first evaluation should be done
        /// </summary>
        float TimeCheckStartAfter { set; }
        /// <summary>
        /// If the assertion is evaluated after a period of time and TimeCheckRepeat is true, after how many seconds should the next evaluation happen
        /// </summary>
        float TimeCheckRepeatFrequency { set; }
        /// <summary>
        /// If the assertion is evaluated after a period, should the evaluation happen again after TimeCheckRepeatFrequency seconds
        /// </summary>
        bool TimeCheckRepeat { set; }

        AssertionComponent Component { get; }
    }
}