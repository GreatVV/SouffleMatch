using System;
using System.Collections.Generic;
using Facebook;
using UnityEngine;

public class FacebookIntegration : MonoBehaviour
{
    #if UNITY_ANDROID || UNITY_EDITOR
    public string AccessToken;
    public bool FbInited;

    public List<object> Friends;
    public Dictionary<string, string> Profile;
    public Texture ProfilePicture;
    public string Username;

    #region Event Handlers

    private void OnHideUnity(bool isUnityShown)
    {
    }

    private void OnInit()
    {
        Debug.Log("Fb inited");
        FbInited = true;
        //FB.GetAuthResponse(OnLogin);
    }

    private void OnLogin(FBResult result)
    {
        AccessToken = FB.AccessToken;
        Debug.Log("Login: " + result.Text + result.Error);
        Debug.Log("call login: " + FB.UserId);

        if (result.Error != null)
        {   
            Debug.LogError(result.Error);
            return;
        }                    
                 
        RequestProfile();
    }

    private void RequestProfile()
    {
        AccessToken = FB.AccessToken;
        FB.API("/me?fields=id,first_name", HttpMethod.GET, OnPlayerInfo, new Dictionary<string, string> {{"access_token", AccessToken}});
        //FB.API("/me/picture?width=128&height=128&access_token=" + AccessToken, Facebook.HttpMethod.GET, OnPicture);
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

    #endregion

    private void Start()
    {
        if (FB.IsLoggedIn && !string.IsNullOrEmpty(FB.AccessToken))
        {
            OnLogin(new FBResult("0"));
        }
        else
        {
            if(!FbInited)
            {
                FB.Init(OnInit, OnHideUnity);
                Debug.Log("fb not inited");
            }
            else
            {
                FB.Login("email,publish_actions", OnLogin);
            }
        }
       
    }

    public void OnFBLoginClick()
    {
        Debug.Log("Fb connect clicked");
   
        if (!FB.IsLoggedIn)
        {
            Debug.Log("Try login");
            FB.Login("email,publish_actions", OnLogin);
        }
        else
        {
            Debug.Log("Logged! " + FB.UserId);
            RequestProfile();
        }
    }    
    #endif   
}
      