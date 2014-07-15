//using UnityEditor;
using UnityEngine;

namespace Game
{
    public class Profile : MonoBehaviour
    {
        public const string defaultProfileName = "default";
        public static Profile Instance;

        public string Current;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(this);
            Instance = this;
        }

        private void Start()
        {
            var lastUsedProfile = PlayerPrefs.GetString("lastUsedProfile", defaultProfileName);
            Load(lastUsedProfile);
        }

        public bool Load(string profileName)
        {
            Current = profileName;

            // Set current user profile
            var prefix = GetPrefix(profileName);

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
                Debug.Log("Unserialize player: "+player);
                Player.Instance.Unserialize(new JSONObject(player));
            }

            // Is profile exists?
            return PlayerPrefs.HasKey(string.Format("{0}_profile", prefix));
        }

        public static string GetPrefix(string profileName)
        {
            return string.Format("profile_{0}_", profileName);
        }

        public static string CurrentPrefix
        {
            get { return Instance == null? GetPrefix(defaultProfileName) : GetPrefix(Instance.Current); }
        }

        public void Save()
        {
            PlayerPrefs.SetString(CurrentPrefix+"_profile", Current);
            SaveEconomy(CurrentPrefix);
            SavePlayer(CurrentPrefix,Player.Instance.Serialize());

            PlayerPrefs.GetString("lastUsedProfile", Current);

            PlayerPrefs.Save();
        }

        public static void SavePlayer(string prefix, JSONObject playerJsonObject)
        {
            PlayerPrefs.SetString(string.Format("{0}_player", prefix), playerJsonObject.ToString());
        }

        private static void SaveEconomy(string prefix)
        {
            PlayerPrefs.SetString(string.Format("{0}_economy", prefix), Economy.Instance.Serialize().ToString());
        }

        private void OnApplicationQuit()
        {
            Save();
        }
    }
}