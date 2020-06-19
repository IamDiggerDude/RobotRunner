using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AutoLogin
{
    public class AutoLoginData
    {
        private const string TrueValue = "True";
        private const string FalseValue = "False";
        private const string AutoLoginPref = "isAutoLogin";
        private const string UsernamePref = "Username";
        private const string PasswordPref = "Password";
        public const string DefaultPref = "";

        // Start is called before the first frame update
        public bool AutoLogin
        {
            get
            {
                return CheckAutoLoginAvailability();
            }
            set
            {
                if(value == true)
                {
                    PlayerPrefs.SetString(AutoLoginPref, TrueValue);
                }
                else
                {
                    PlayerPrefs.SetString(AutoLoginPref, FalseValue);
                }
            }
        }
        public string Username
        {
            get
            {
                return PlayerPrefs.GetString(UsernamePref,DefaultPref);
            }
            set
            {
                PlayerPrefs.SetString(UsernamePref, value);
            }
        }
        public string Password
        {
            get
            {
                return PlayerPrefs.GetString(PasswordPref,DefaultPref);
            }
            set
            {
                PlayerPrefs.SetString(PasswordPref, value);
            }
        }
        private bool CheckAutoLoginAvailability()
        {
            Debug.LogFormat("Is auto log is on : {0}. Is Password set : {1}. Is Username set : {2}", PlayerPrefs.GetString(AutoLoginPref) == TrueValue, Password != DefaultPref, Username != DefaultPref);
            return PlayerPrefs.GetString(AutoLoginPref) == TrueValue && Password != DefaultPref && Username != DefaultPref;
        }

    }
}