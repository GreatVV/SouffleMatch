using UnityEngine;

[RequireComponent(typeof(LevelManager))]
public class SessionRestorer : MonoBehaviour
{
    public int lastPlayedLevel;

    private LevelManager levelManager;

    public Gamefield Gamefield;
    public Gameplay Gameplay;
    public static SessionRestorer Instance;

    void Awake()
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

    public void OnWindowChanged(Window currentActiveWindow)
    {
        Gamefield.IsPause = !PanelManager.IsCurrent(SessionRestorer.Instance.Gameplay);
    }

    private void OnPause(bool pause)
    {
        Tutorial.SetActive(!pause);
    }

    private void OnLevelsAreReady()
    {
        StartLevel(lastPlayedLevel);
    }

    public void StartLevel(int index)
    {
        lastPlayedLevel = index;
        Gamefield.StartGame(levelManager[index]);
        PanelManager.Show(Gameplay, true);
        Tutorial.instance.Begin();
    }

    public void PlayNextLevel()
    {
        if (lastPlayedLevel < levelManager.LoadedLevels.Count-1)
        {
            lastPlayedLevel++;
        }
        StartLevel(lastPlayedLevel);
    }

    public void Restart()
    {
        StartLevel(lastPlayedLevel);
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            levelManager.LevelsAreReady -= OnLevelsAreReady;
        }
    }

    void OnApplicationPause()
    {
        PlayerPrefs.SetInt("LastPlayedLevel", lastPlayedLevel);
        PlayerPrefs.Save();
    }
}