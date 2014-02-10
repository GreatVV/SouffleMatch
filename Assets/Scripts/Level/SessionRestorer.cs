using UnityEngine;

[RequireComponent(typeof(LevelManager))]
public class SessionRestorer : MonoBehaviour
{
    public int lastPlayedLevel;

    private LevelManager levelManager;

    public Gamefield Gamefield;
    public Gameplay Gameplay;


    void Awake()
    {
        levelManager = GetComponent<LevelManager>();

        lastPlayedLevel = PlayerPrefs.GetInt("LastPlayedLevel", 0);

        levelManager.LevelsAreReady += OnLevelsAreReady;
    }

    private void OnLevelsAreReady()
    {
        StartLevel(lastPlayedLevel);
    }

    public void StartLevel(int index)
    {
        Gamefield.StartGame(levelManager[index]);
        PanelManager.Show(Gameplay, true);
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
        levelManager.LevelsAreReady -= OnLevelsAreReady;
    }

    void OnApplicationPause()
    {
        PlayerPrefs.SetInt("LastPlayedLevel", lastPlayedLevel);
        PlayerPrefs.Save();
    }
}