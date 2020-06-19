using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using TMPro;
using Validator;
using UnityEngine.UI;

public class UserInfo : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Username;
    [SerializeField]
    TMP_InputField Password;
    [SerializeField]
    TextMeshProUGUI Email;
    [SerializeField]
    TMP_InputField EmailChange;
    [SerializeField]
    TextMeshProUGUI EmailError;


    private void Start()
    {
        PlayerProfileViewConstraints playerProfileViewConstraints = new PlayerProfileViewConstraints();
        playerProfileViewConstraints.ShowContactEmailAddresses = true;
        playerProfileViewConstraints.ShowDisplayName = true;
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest { ProfileConstraints = playerProfileViewConstraints },
            ProfileResult =>
            {
                Username.text = ProfileResult.PlayerProfile.DisplayName;
                Email.text = ProfileResult.PlayerProfile.ContactEmailAddresses[0].EmailAddress;
                EmailChange.text = ProfileResult.PlayerProfile.ContactEmailAddresses[0].EmailAddress;
            }, ProfileError => Debug.Log(ProfileError.GenerateErrorReport()));
        Password.text = PlayerPrefs.GetString("Password");
    }
    private void OnEnable()
    {
        EmailError.text = "";
    }
    public void ShowPassword(bool show)
    {
        Password.contentType = show ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        Password.text = "";
        Password.text = PlayerPrefs.GetString("Password");
    }
    public void ChangePassword()
    {
        //Title ID: 8910C
        PlayFabClientAPI.SendAccountRecoveryEmail(
            new SendAccountRecoveryEmailRequest { Email = Email.text },
            SendRecoveryEmailResult => { Debug.Log("Password change email sent"); },
            SendRecoveryEmailError => { Debug.Log(SendRecoveryEmailError.GenerateErrorReport()); }
            );
    }
    public void ChangeEmail()
    {
        EmailError.text = "";
        EmailValidationData emailValidationData = new EmailValidationData();
        emailValidationData.MaxCharacterCount = 30;
        IValidate EmailValidator = new EmailValidator(EmailChange.text, emailValidationData, EmailError);
        if (EmailValidator.isValid())
        {
            Debug.Log("Valid");
            AddOrUpdateContactEmailRequest addOrUpdateContactEmail = new AddOrUpdateContactEmailRequest { EmailAddress = EmailChange.text };
            PlayFabClientAPI.AddOrUpdateContactEmail(addOrUpdateContactEmail,
                EmailResult =>
                {
                    Debug.Log("Email changed");
                    Email.text = EmailChange.text;
                }, EmailError => { Debug.Log(EmailError.GenerateErrorReport()); });
        }
        else
        {
            Debug.Log("InValid");
            StartCoroutine(ErrorColor());
        }
    }
    IEnumerator ErrorColor()
    {
        ColorBlock block = EmailChange.colors;
        block.normalColor = Color.red;
        EmailChange.colors = block;
        yield return new WaitForSeconds(1);
        block.normalColor = Color.white;
        EmailChange.colors = block;
    }
}
