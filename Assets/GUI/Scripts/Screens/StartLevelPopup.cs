using System.Globalization;
using System.Linq;
using UnityEngine;

public class StartLevelPopup : Window
{
    #region Localization

    public static Phrase AttemptsString = new Phrase("Попыток: {0}", "StartLevelPopup_Attempts");
    public static Phrase TaskString = new Phrase("{0}", "StartLevelPopup_Task");
    public static Phrase BestScoreString = new Phrase("Лучший: {0}", "StartLevelPopup_BestScore");
    public static Phrase LevelNumberString = new Phrase("Level {0}", "StartLevelPopup_LevelNumber");
    public static Phrase Star1TargetString = new Phrase("1 звезда: {0}", "StartLevelPopup_1StarTarget");
    public static Phrase Star2TargetString = new Phrase("2 звезды: {0}", "StartLevelPopup_2StarTarget");
    public static Phrase Star3TargetString = new Phrase("3 звезды: {0}", "StartLevelPopup_3StarTarget");

    #endregion

    #region In inspector

    public UILabel LevelNumberTitle;
    public UILabel TaskLabel;

    public UILabel StarLabelTask;

    public UILabel BestScoreLabel;

    public GameObject[] Stars; 

    #endregion

    private LevelInfo info;
    private SerializedLevel level;

    public int Index { get; set; }

    public void Init(int levelIndex)
    {
        Index = levelIndex;
        info = Player.Instance.Info(Index);
        level = MainMenu.Instance.LoadedLevels.First(x => x.Name == (Index+1).ToString(CultureInfo.InvariantCulture));

        LevelNumberTitle.text = LocalizationStrings.GetString(LevelNumberString, info.Number);
        BestScoreLabel.text = LocalizationStrings.GetString(BestScoreString, info.BestScore);

        
        TaskLabel.text = LocalizationStrings.GetString(TaskString, GameModeToString.GetString(GameModeFactory.CreateGameMode(level.GameMode)));

        
    }

    public void OnPlayButtonClick()
    {
        Play();
    }

    private void Play()
    {
        
        bool canStartLevel = Player.Instance.Lifes.HasLife;
        if (canStartLevel)
        {
            PanelManager.Show(Windows.GamefieldScreen(level),true);
        }
        else
        {
            //TODO show buy life popup;
            PanelManager.Show(Windows.BuyLivesPopup());
        }
    }
}
