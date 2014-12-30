//using UnityEditor;

using Game.Utility;
using UnityEngine;
using Utils;

namespace Game.Player
{
    public class Profile : MonoBehaviour
    {
        public const string defaultProfileName = "default";

        public string Current;

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

            // Is profile exists?
            return PlayerPrefs.HasKey(string.Format("{0}_profile", prefix));
        }

        public static string GetPrefix(string profileName)
        {
            return string.Format("profile_{0}_", profileName);
        }

        public string CurrentPrefix
        {
            get { return GetPrefix(Current); }
        }

        public void Save()
        {
            PlayerPrefs.SetString(CurrentPrefix+"_profile", Current);

            PlayerPrefs.GetString("lastUsedProfile", Current);

            PlayerPrefs.Save();
        }

    

        private void OnApplicationQuit()
        {
            Save();
        }
    }
}