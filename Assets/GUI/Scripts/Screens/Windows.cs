using UnityEngine;

public class Windows : MonoBehaviour
{
    public StartLevelPopup startLevelPopup;

    public Gamefield Gamefield;
    public Gameplay GuiGameplay;
    private static Window buyLivesPopup;

    public static Windows instance { get; private set; }


    public static Window StartLevelPopup(int levelIndex)
    {
        instance.startLevelPopup.Init(levelIndex);
        return instance.startLevelPopup;
    }

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

    public static Window BuyLivesPopup()
    {
        return buyLivesPopup;
    }
}