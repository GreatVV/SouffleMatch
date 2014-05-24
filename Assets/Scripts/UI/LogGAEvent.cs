using Game;
using UnityEngine;

public class LogGAEvent : MonoBehaviour
{
    public bool AddLevelName;
    public string EventString;

    public void Log()
    {
        string eventName = AddLevelName ? string.Format("Game:{0}:{1}", SessionRestorer.Instance.Gamefield.Level.Serialized.Name, EventString) : EventString;

        GA.API.Design.NewEvent(eventName);
    }
}