using Game;
using UnityEngine;

public class ResetButton : MonoBehaviour
{
    public GameObject pauseButton;
    public SessionRestorer sessionRestorer;

    public void Restart()
    {
        sessionRestorer.Restart();
        pauseButton.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
        GA.API.Design.NewEvent(string.Format("UI:{0}:Reset:Menu", sessionRestorer.Gamefield.Level.Serialized.Name));
    }
}
