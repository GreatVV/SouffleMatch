using Game.Data;
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

    public static Window GamefieldScreen(LevelDescription  levelDescription)
    {
        instance.Gamefield.gameObject.SetActive(true);
        instance.Gamefield.StartGame(levelDescription);
        return instance.GuiGameplay;
    }
}