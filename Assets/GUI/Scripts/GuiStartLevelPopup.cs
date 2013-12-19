#region

using UnityEngine;

#endregion

public class GuiStartLevelPopup : Window
{
    public static Phrase AttemptsString = new Phrase("Попыток: {0}", "StartLevelPopup_Attempts");
    public static Phrase TaskString = new Phrase("{0}", "StartLevelPopup_Task");
    public static Phrase BestScoreString = new Phrase("Лучший: {0}", "StartLevelPopup_BestScore");
    public static Phrase LevelNumberString = new Phrase("Level {0}", "StartLevelPopup_LevelNumber");
    public static Phrase Star1TargetString = new Phrase("1 звезда: {0}", "StartLevelPopup_1StarTarget");
    public static Phrase Star2TargetString = new Phrase("2 звезды: {0}", "StartLevelPopup_2StarTarget");
    public static Phrase Star3TargetString = new Phrase("3 звезды: {0}", "StartLevelPopup_3StarTarget");

    public UILabel BestScoreLabel;
    public SerializedLevel CurrentLevel;
    public LevelInfo CurrentLevelInfo;
    
    public UILabel TaskLabel;

    public PopupStar FirstStar;
    public PopupStar SecondStar;
    public PopupStar ThirdStar;
    public UILabel StarTarget;
    public UILabel TitleLabel;

    #region Event Handlers

    private void OnEnable()
    {
        TitleLabel.text = LocalizationStrings.GetString(LevelNumberString, CurrentLevelInfo.Number);
        BestScoreLabel.text = LocalizationStrings.GetString(BestScoreString, CurrentLevelInfo.BestScore);
        TaskLabel.text = LocalizationStrings.GetString(TaskString, GameModeToString.GetString(GameModeFactory.CreateGameMode(CurrentLevel.GameMode)));
        transform.localPosition = new Vector3(0, -800, -5);
        iTween.MoveTo(gameObject, new Vector3(0, 0, -0.01f), 0.5f);
    }

    protected override bool OnClose()
    {
        Debug.Log("onclose");
        iTween.MoveTo(gameObject,
            iTween.Hash("x", 0, "y", 2, "z", -0.01f, "time", 0.5f,
                "oncomplete", "OnCloseAnimationComplete", "oncompletetarget", gameObject, "oncompleteparams", 0));

        return false;
    }

    public void OnCloseAnimationComplete()
    {
        Disable();
    }

    public override void OnCloseButton()
    {
        Close();
    }

    public void OnStartButton()
    {
        UI.Instance.TryStartLevel(CurrentLevel);
    }

    #endregion

    public void Show(SerializedLevel level)
    {
        CurrentLevel = level;
        CurrentLevelInfo = Player.Instance.GetLevelInfo(level.Name);

        if (CurrentLevelInfo.IsCompleted)
        {
            var numberOfStars = CurrentLevel.NumberOfStarForScore(CurrentLevelInfo.BestScore);

            if (numberOfStars >= 1)
            {
                FirstStar.Show(true);
                if (numberOfStars == 1)
                {
                    StarTarget.text = LocalizationStrings.GetString(Star2TargetString, CurrentLevel.Star2Score);
                }
            }

            if (numberOfStars >= 2)
            {
                SecondStar.Show(true);
                if (numberOfStars == 2)
                {
                    StarTarget.text = LocalizationStrings.GetString(Star3TargetString, CurrentLevel.Star3Score);
                }
            }

            if (numberOfStars >= 3)
            {
                ThirdStar.Show(true);
                if (numberOfStars == 3)
                {
                    StarTarget.text = string.Empty;
                }
            }
            BestScoreLabel.gameObject.SetActive(true);
        }
        else
        {
            FirstStar.Show(false);
            SecondStar.Show(false);
            ThirdStar.Show(false);

            StarTarget.text = LocalizationStrings.GetString(Star1TargetString, CurrentLevel.Star1Score);
            BestScoreLabel.gameObject.SetActive(false);
        }

        Show();
    }
}