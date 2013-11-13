using UnityEngine;

public class MapId : MonoBehaviour
{
    public int Index;

    public GameObject FirstStar;
    public GameObject SecondStar;
    public GameObject ThirdStar;

    public void Start()
    {
        GuiLevelList.LevelsLoaded += OnLevelsLoaded;
    }

    public void OnDestroy()
    {
        GuiLevelList.LevelsLoaded -= OnLevelsLoaded;
    }

    private void OnLevelsLoaded(bool obj)
    {
        RefreshNumberOfStars();
    }

    public void OnEnable()
    {
        if (UI.Instance == null)
        {
            return;
        }

        if (UI.Instance.GuiLevelList.IsLevelsLoaded)
        {
            RefreshNumberOfStars();
        }
    }

    public void RefreshNumberOfStars()
    {
        if (Index >= UI.Instance.GuiLevelList.LoadedLevels.Count)
            return;

        var level = UI.Instance.GuiLevelList.LoadedLevels[Index];
        var levelInfo = Player.Instance.GetLevelInfo(level.Name);
        if (levelInfo != null && levelInfo.IsCompleted)
        {   
            var numberOfStars = level.NumberOfStarForScore(levelInfo.BestScore);
            if (numberOfStars >= 1)
            {
                FirstStar.SetActive(true);
            }

            if (numberOfStars >= 2)
            {
                SecondStar.SetActive(true);
            }

            if (numberOfStars >= 3)
            {
                ThirdStar.SetActive(true);
            }
        }
        else
        {
            FirstStar.SetActive(false);
            SecondStar.SetActive(false);
            ThirdStar.SetActive(false);
        }
    }
}