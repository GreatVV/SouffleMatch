using System;
using System.Globalization;
using UnityEngine;
using Object = UnityEngine.Object;

[ExecuteInEditMode]
public class MapId : MonoBehaviour
{
    public int Index;

    public GameObject FirstStar;
    public GameObject SecondStar;
    public GameObject ThirdStar;

    public GameObject currentLevelParticle;
    private GameObject effect;

    public void Awake()
    {
        name = "0";
        //UpdateName();
    }

    public void Start()
    {
        MainMenu.LevelsLoaded += OnLevelsLoaded;
    }

    public void OnDestroy()
    {
        MainMenu.LevelsLoaded -= OnLevelsLoaded;
    }

    private void OnLevelsLoaded(bool obj)
    {
        RefreshNumberOfStars();
    }

    public void OnEnable()
    {
        SetStars(0);
        /*if (UI.Instance && UI.Instance.GuiLevelList.IsLevelsLoaded)
        {
            RefreshNumberOfStars();
        }*/
    }

    public void Update()
    {
#if UNITY_EDITOR
       // UpdateName();
#endif
    }

    public void UpdateName()
    {
        Index = Convert.ToInt32(name)-1;
        FirstStar = transform.Search("1", gameObject).gameObject;
        SecondStar = transform.Search("2", gameObject).gameObject;
        ThirdStar = transform.Search("3", gameObject).gameObject;
        transform.Search("Label", gameObject).GetComponent<UILabel>().text = name;
        //(Index).ToString(CultureInfo.InvariantCulture);
    }

    public void RefreshNumberOfStars()
    {
        //if (Index >= UI.Instance.GuiLevelList.LoadedLevels.Count)
         //   return;

        var level = new SerializedLevel();

        //    UI.Instance.GuiLevelList.LoadedLevels[Index];
        //Debug.Log("PL: "+Player.Instance);
        //Debug.Log("Level name: "+level.Name);
        var levelInfo = Player.Instance.GetLevelInfo(level.Name);
        if (levelInfo != null && levelInfo.IsCompleted)
        {
            var numberOfStars = level.NumberOfStarForScore(levelInfo.BestScore);
            SetStars(numberOfStars);
        }
        else
        {
            FirstStar.SetActive(false);
            SecondStar.SetActive(false);
            ThirdStar.SetActive(false);
        }
    }

    private void SetStars(int numberOfStars)
    {
        FirstStar.SetActive(false);
        SecondStar.SetActive(false);
        ThirdStar.SetActive(false);

        switch (numberOfStars)
        {
            case(1):
            {
                FirstStar.SetActive(true);
                break;
            }
            case(2):
            {
                SecondStar.SetActive(true);
                break;
            }
            case(3):
            {
                ThirdStar.SetActive(true);
                break;
            }
        }
    }

    public void OnClick()
    {
        SessionRestorer.Instance.StartLevel(Index);
    }

    void OnDisable()
    {
        if (effect)
        {
            Destroy(effect);
        }
    }

    public void ShowCurrent()
    {
        effect = (GameObject)Instantiate(currentLevelParticle);
        effect.transform.parent = transform;
        effect.transform.localPosition = Vector3.zero;
        effect.transform.localScale = Vector3.one;
    }
}