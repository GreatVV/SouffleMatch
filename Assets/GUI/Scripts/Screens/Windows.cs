using UnityEngine;

public class Windows : MonoBehaviour
{
    public Gamefield Gamefield;
    
    public Gameplay GuiGameplay;

    public static Windows instance { get; private set; }

    public void Awake()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    void OnDestroy()
    {
        if (this == instance)
        {
            instance = null;
        }
    }

    public static Window GamefieldScreen(SerializedLevel  level)
    {
        instance.Gamefield.gameObject.SetActive(true);
        instance.Gamefield.StartGame(level);
        return instance.GuiGameplay;
    }
}