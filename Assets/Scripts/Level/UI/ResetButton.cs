using UnityEngine;

public class ResetButton : MonoBehaviour
{
    public GameObject pauseButton;
    public SessionRestorer sessionRestorer;

    public void Restart()
    {
        sessionRestorer.Restart();
        pauseButton.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
    }
}
