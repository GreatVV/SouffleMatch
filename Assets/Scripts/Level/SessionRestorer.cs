using UnityEngine;

[RequireComponent(typeof (LevelManager))]
public class SessionRestorer : MonoBehaviour
{
    public static SessionRestorer Instance;
    public Gamefield Gamefield;
    public Gameplay Gameplay;
    public int lastPlayedLevel;

    public Tutorial tutorialPrefab;

    private LevelManager levelManager;

    #region Events Subscribers

    public void OnWindowChanged(Window currentActiveWindow)
    {
        Debug.Log("Window changed: " + currentActiveWindow);
        Gamefield.IsPause = !PanelManager.IsCurrent(Instance.Gameplay);
    }

    private void OnPause(bool pause)
    {
        //Tutorial.SetActive(!pause);
    }

    private void OnLevelsAreReady()
    {
        StartLevel(lastPlayedLevel);
    }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        levelManager = GetComponent<LevelManager>();

        lastPlayedLevel = PlayerPrefs.GetInt("LastPlayedLevel", 0);

        levelManager.LevelsAreReady += OnLevelsAreReady;

        PanelManager.WindowChanged += OnWindowChanged;


        Gamefield.Paused += OnPause;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            levelManager.LevelsAreReady -= OnLevelsAreReady;
        }
    }

    private void OnApplicationPause()
    {
        PlayerPrefs.SetInt("LastPlayedLevel", lastPlayedLevel);
        PlayerPrefs.Save();
    }

    #endregion

    public void StartLevel(int index)
    {
        lastPlayedLevel = index;
        Gamefield.StartGame(levelManager[index]);
        PanelManager.Show(Gameplay, true);
        
        /*
        if (index == 0)
        {
            if (!Tutorial.Instance)
            {
                Instantiate(tutorialPrefab);
            }
            Tutorial.Begin();
        }
        else
        {
            if (Tutorial.isActive)
            {
                Tutorial.End();
            }
        }*/
    }

    public void PlayNextLevel()
    {
        if (lastPlayedLevel < levelManager.LoadedLevels.Count - 1)
        {
            lastPlayedLevel++;
        }
        StartLevel(lastPlayedLevel);
    }

    public void Restart()
    {
        iTween.Stop();
        StartLevel(lastPlayedLevel);
    }
}