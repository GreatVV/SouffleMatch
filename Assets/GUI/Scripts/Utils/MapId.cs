using System;
using System.Globalization;
using UnityEngine;

[ExecuteInEditMode]
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

    public void Update()
    {
#if UNITY_EDITOR
        Index = Convert.ToInt32(name);
        FirstStar = transform.Search("1", gameObject).gameObject;
        SecondStar = transform.Search("2", gameObject).gameObject;
        ThirdStar = transform.Search("3", gameObject).gameObject;
        transform.Search("Label", gameObject).GetComponent<UILabel>().text = (Index + 1).ToString(CultureInfo.InvariantCulture);
#endif
    }


    public void RefreshNumberOfStars()
    {
        if (Index >= UI.Instance.GuiLevelList.LoadedLevels.Count)
            return;

        var level = UI.Instance.GuiLevelList.LoadedLevels[Index];
        Debug.Log("PL: "+Player.Instance);
        Debug.Log("Level name: "+level.Name);
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