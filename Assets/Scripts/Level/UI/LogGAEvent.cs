using UnityEngine;

public class LogGAEvent : MonoBehaviour
{
    public bool AddLevelName;
    public string EventString;

    public void Log()
    {
        string eventName = "";
        if (AddLevelName)
        {
            eventName = string.Format("Game:{0}:{1}", SessionRestorer.Instance.Gamefield.Level.LevelName, EventString);
        }
        else
        {
            eventName = EventString;
        }

        GA.API.Design.NewEvent(eventName);
    }
}