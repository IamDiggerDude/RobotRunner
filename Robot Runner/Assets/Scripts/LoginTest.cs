using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;


public class LoginTest : MonoBehaviour
{
    public string UserEmail;
    public string UserName;
    public string UserPassword;
    public string UserDisplayName;
    public void Start()
    {
        Debug.LogFormat("Is title id: {0}",!string.IsNullOrEmpty(PlayFabSettings.TitleId));    
    }
    public void Login()
    {
        var LoginRequest = new LoginWithEmailAddressRequest { Email = UserEmail, Password = UserPassword };
        PlayFabClientAPI.LoginWithEmailAddress(LoginRequest, OnLoginSuccess, OnLoginFailure);
    }
    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you LoggedIn");
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your first API call.  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
    public void RegisterUser()
    {
        var registerRequest = new RegisterPlayFabUserRequest();
        registerRequest.Email = UserEmail;
        registerRequest.Password = UserPassword;
        registerRequest.Username = UserName;
        registerRequest.DisplayName = UserDisplayName;
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterError);
    }
    private void OnRegisterSuccess(RegisterPlayFabUserResult Result)
    {
        Debug.Log("Register success");
    }
    private void OnRegisterError(PlayFabError error)
    {
        Debug.LogFormat("Register error {0}", error.HttpCode);
    }
}
