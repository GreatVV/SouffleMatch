using UnityEngine;

public class Profile : MonoBehaviour
{
    public static Profile Instance;

    public string Current;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        var lastUsedProfile = PlayerPrefs.GetString("lastUsedProfile", "default");
        Load(lastUsedProfile);
    }

    public bool Load(string profileName)
    {
        Current = profileName;

        // Set current user profile
        var prefix = string.Format("profile_{0}_", profileName);

        // Load ecomomy progress
        var economy = PlayerPrefs.GetString(string.Format("{0}_economy", prefix), "");
        if (!string.IsNullOrEmpty(economy))
        {
            Economy.Instance.Unserialize(new JSONObject(economy));
        }

        //player
        var player = PlayerPrefs.GetString(string.Format("{0}_player", prefix));
        if (!string.IsNullOrEmpty(player))
        {
            Player.Instance.Unserialize(new JSONObject(player));
        }

        // Is profile exists?
        return PlayerPrefs.HasKey(string.Format("{0}_profile", prefix));
    }

    public void Save()
    {
        // Set current user profile
        var prefix = string.Format("profile_{0}_", Current);

        Debug.Log("Save Profile");

        PlayerPrefs.SetString(string.Format("{0}_profile", prefix), Current);
        PlayerPrefs.SetString(string.Format("{0}_economy", prefix), Economy.Instance.Serialize().ToString());
        PlayerPrefs.SetString(string.Format("{0}_player", prefix), Player.Instance.Serialize().ToString());

        PlayerPrefs.GetString("lastUsedProfile", Current);

        PlayerPrefs.Save();
    }
}