#region

using UnityEngine;

#endregion

public class GuiWinPanel : Window
{
    public static Phrase TurnsLeftString = new Phrase("Ходов осталось: {0}", "WinPopup_TurnsLeft");
    public static Phrase ScoreString = new Phrase("Счет: {0}", "WinPopup_Score");
    public static Phrase BestScoreString = new Phrase("Лучший: {0}", "WinPopup_BestScore");
    public UILabel BestScore;
    public UILabel Score;

    public PopupStar FirstStar;
    public PopupStar SecondStar;
    public PopupStar ThirdStar;

    #region Event Handlers

    public void OnMapClick()
    {
        UI.Instance.ShowMap();
    }

    private void OnEnable()
    {
        transform.localPosition = new Vector3(0, -800, 0);
        iTween.MoveTo(gameObject, new Vector3(0, 0, 0), 0.5f);
    }

    protected override bool OnClose()
    {
        Debug.Log("onclose");
        iTween.MoveTo(gameObject,
            iTween.Hash("x", 0, "y", 2, "z", 0, "time", 0.5f,
                "oncomplete", "OnCloseAnimationComplete", "oncompletetarget", gameObject, "oncompleteparams", 0));

        return false;
    }

    public void OnCloseAnimationComplete()
    {
        Disable();
    }

    #endregion

    public int TempScore;
    public int TargetScore;

    public LevelInfo LevelInfo;
    public SerializedLevel Level;

    public void Show(int numberOfTurnsLeft, int score, int bestScore, LevelInfo levelInfo, SerializedLevel level)
    {
        TempScore = 0;
        TargetScore = score;

        LevelInfo = levelInfo;
        Level = level;
        
        Score.text = LocalizationStrings.GetString(ScoreString, 0);
        BestScore.text = LocalizationStrings.GetString(BestScoreString, bestScore);

        FirstStar.Show(false);
        SecondStar.Show(false);
        ThirdStar.Show(false);

        Show();
    }

    private void Update()
    {
        if (TempScore < TargetScore)
        {
            TempScore += 10;
            TempScore = Mathf.Clamp(TempScore, 0, TargetScore);
            Score.text = LocalizationStrings.GetString(ScoreString, TempScore);

            if (TempScore > Level.Star1Score && !FirstStar.IsActive)
            {
                ShowStar(1);
            }

            if (TempScore > Level.Star2Score && !SecondStar.IsActive)
            {
                ShowStar(2);
            }

            if (TempScore > Level.Star3Score && !ThirdStar.IsActive)
            {
                ShowStar(3);
            }
        }
    }

    private void ShowStar(int starNumber)
    {
        switch (starNumber)
        {
            case 1:
                FirstStar.Show(true, true);
                break;
            case 2:
                SecondStar.Show(true, true);
                break;
            case 3:
                ThirdStar.Show(true, true);
                break;
        }
    }
}