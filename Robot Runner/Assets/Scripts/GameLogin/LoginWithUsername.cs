using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayFabLogin
{
    public class LoginWithUsername :Logger
    {
        bool fail = false;

        const string mainSceneName = "Main";
        string username;
        string password;
        OnLoginSuccess loginSuccessExternalHandler = () => { };
        OnLoginFailure loginFailureExternalHandler = () => {};


        public LoginWithUsername(string Username, string Password)
        {
            username = Username;
            password = Password;
        }

        public LoginWithUsername(string username, string password, OnLoginSuccess loginLoginExternalHandler, OnLoginFailure loginFailureExternalHandler) : this(username, password)
        {
            this.loginSuccessExternalHandler = loginLoginExternalHandler;
            this.loginFailureExternalHandler = loginFailureExternalHandler;
        }

        public override void Login()
        {
            fail = false;
            LoginWithPlayFabRequest request = new LoginWithPlayFabRequest { Username = username, Password = password };
            PlayFabClientAPI.LoginWithPlayFab(request,OnLoginSuccessHandler,OnLoginFailureHandler);
        }
        void OnLoginSuccessHandler(LoginResult result)
        {
            PlayerPrefs.SetString("Password", password);

            PlayfabData.SharedInstance.SessionToken = result.SessionTicket;
            PlayfabData.SharedInstance.PlayfabUserId = result.PlayFabId;
            loginSuccessExternalHandler();
            SceneManager.LoadScene(mainSceneName);            
        }
        void OnLoginFailureHandler(PlayFabError error)
        {
            fail = true;
            loginFailureExternalHandler();
            Debug.Log("LoginFail");
        }
    }
}
