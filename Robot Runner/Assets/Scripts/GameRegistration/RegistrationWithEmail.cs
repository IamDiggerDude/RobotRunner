using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using TMPro;

namespace PlayFabRegistration
{
    public class RegistrationWithEmail : Registrator
    {
        string email;
        string username;
        string password;
        string refererUsername;
        readonly string SatoshiStatField;
        IIncreaseUserStat SuperSatoshiChanger;
        protected OnRegistrationSuccess onRegistrationSuccess;
        protected OnRegistrationFail onRegistrationFail;

        TextMeshProUGUI PlayfabErrorField;



        public RegistrationWithEmail(string email, string username, string password) : base(null, null)
        {
            this.email = email;
            this.username = username;
            this.password = password;
        }

        public RegistrationWithEmail(string email, string username, string password, OnRegistrationSuccess onRegistrationSuccess, OnRegistrationFail onRegistrationFail) : this(email, username, password)
        {
            this.onRegistrationSuccess = onRegistrationSuccess;
            this.onRegistrationFail = onRegistrationFail;
        }

        public RegistrationWithEmail(string email, string username, string password, string refererUsername, string satoshiStatField, IIncreaseUserStat superSatoshiChanger, OnRegistrationSuccess onRegistrationSuccess, OnRegistrationFail onRegistrationFail) : this(email, username, password)
        {
            this.refererUsername = refererUsername;
            SatoshiStatField = satoshiStatField;
            SuperSatoshiChanger = superSatoshiChanger;
            this.onRegistrationSuccess = onRegistrationSuccess;
            this.onRegistrationFail = onRegistrationFail;
        }

        public override void Registration(TextMeshProUGUI PlayfabErrorField)
        {
            var request = new RegisterPlayFabUserRequest();
            request.DisplayName = username;
            request.Username = username;
            request.Email = email;
            request.Password = password;

            this.PlayfabErrorField = PlayfabErrorField;

            PlayFabClientAPI.RegisterPlayFabUser(request, OnPlayfabRegistrationSuccess, OnPlayfabRegistrationFailure);
        }
        void OnPlayfabRegistrationSuccess(RegisterPlayFabUserResult result)
        {
            if (SuperSatoshiChanger != null)
            {
                SuperSatoshiChanger.IncreaseStat(refererUsername, SatoshiStatField, 4.ToString());
            }

            AddOrUpdateContactEmailRequest addOrUpdateContactEmail = new AddOrUpdateContactEmailRequest { EmailAddress = email };
            PlayFabClientAPI.AddOrUpdateContactEmail(addOrUpdateContactEmail, EmailResult => { Debug.Log("Email adjusted"); }, EmailError => { Debug.Log(EmailError.GenerateErrorReport()); });
            PlayerPrefs.SetString("Password",password);

            onRegistrationSuccess();

            PlayerPrefs.SetString("Skin", "Classic");

            PlayerPrefs.DeleteKey("Day");
            PlayerPrefs.DeleteKey("Month");

            Debug.Log("Success");
        }
        void OnPlayfabRegistrationFailure(PlayFabError error)
        {
            onRegistrationFail();
            Debug.Log("Error, cannot register user " + error.GenerateErrorReport());
            if(error.Error == PlayFabErrorCode.NameNotAvailable || error.Error == PlayFabErrorCode.UsernameNotAvailable)
            {
                PlayfabErrorField.text = "Name is already taken";
            }
        }
    }
}
