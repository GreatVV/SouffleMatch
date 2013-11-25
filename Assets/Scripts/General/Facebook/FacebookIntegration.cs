#region

using System.Collections.Generic;
using Facebook;
using UnityEngine;

#endregion

public class FacebookIntegration : MonoBehaviour
{
#if UNITY_ANDROID || UNITY_EDITOR
    public static FacebookIntegration Instance;
    public string AccessToken;
    public bool FbInited;

    public List<object> Friends;
    public Dictionary<string, string> Profile;
    public Texture ProfilePicture;
    public string Username;

    #region Event Handlers

    private void OnHideUnity(bool isUnityShown)
    {
        FbDebug.Log("OnHideUnity");
        Time.timeScale = !isUnityShown ? 0 : 1;
    }

    private void OnInit()
    {
        Debug.Log("Fb inited");
        FbInited = true;
    }

    private void OnLogin(FBResult result)
    {
        AccessToken = FB.AccessToken;
        Debug.Log("Login: " + result.Text + result.Error);
        Debug.Log("Сall login: " + FB.UserId);

        if (result.Error != null)
        {
            Debug.LogError(result.Error);
            return;
        }

        RequestProfile();
    }

    private void OnPlayerInfo(FBResult result) // handle user profile info
    {
        if (result.Error != null)
        {
            Debug.LogError(result.Error);
            return;
        }

        Profile = Util.DeserializeJSONProfile(result.Text);
        Username = Profile["first_name"];
        //  Friends = Util.DeserializeJSONFriends(result.Text);
    }

    private void OnPicture(FBResult result) // store user profile pic
    {
        if (result.Error != null)
        {
            Debug.LogError(result.Error);
            return;
        }

        ProfilePicture = result.Texture;
    }

    public void OnFBLoginClick()
    {
        Debug.Log("Fb connect clicked");

        if (!FB.IsLoggedIn)
        {
            Debug.Log("Try login");
            FB.Login("publish_stream,publish_actions,user_games_activity,friends_games_activity", OnLogin);
        }
        else
        {
            Debug.Log("Logged! " + FB.UserId);
            RequestProfile();
        }
    }

    private void OnAskLifeClicked()
    {
        //TODO Ask life
    }

    #endregion

    private void RequestProfile()
    {
        AccessToken = FB.AccessToken;
        FB.API("/me?fields=id,first_name", HttpMethod.GET, OnPlayerInfo,
            new Dictionary<string, string> {{"access_token", AccessToken}});
        //FB.API("/me/picture?width=128&height=128&access_token=" + AccessToken, Facebook.HttpMethod.GET, OnPicture);
    }

    private void Start()
    {
        Instance = this;
        if (FB.IsLoggedIn && !string.IsNullOrEmpty(FB.AccessToken))
        {
            OnLogin(new FBResult("0"));
        }
        else
        {
            if (!FbInited)
            {
                FB.Init(OnInit, OnHideUnity);
                Debug.Log("fb not inited");
            }
            else
            {
                FB.Login("publish_actions", OnLogin);
            }
        }
    }

    public static void SendLevelResult(string levelName, int points)
    {
    }

    public static List<ResultDescription> FriendTopForLevel(string levelName, int maxAmount)
    {
        return new List<ResultDescription>
        {
            new ResultDescription
            {
                Id = "0",
                Place = 1,
                Name = "Vova Pupkin",
                Score = 10000
            }
        };
    }

#endif
}

public class ResultDescription
{
    public Texture AvatarTexture;
    public string Id;
    public string Name;
    public int Place;

    public int Score;
}