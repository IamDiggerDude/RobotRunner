using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Validator
{
    public class UsernameValidator : IValidate
    {
        string username;
        UsernameValidationData data;
        TextMeshProUGUI usernameerror;

        public UsernameValidator(string username, UsernameValidationData data, TextMeshProUGUI usernameerror)
        {
            this.username = username;
            this.data = data;
            this.usernameerror = usernameerror;
        }

        public bool isValid()
        {
            return CheckLength();
        }

        bool CheckLength()
        {
            if (username.Length > data.MaxCharacterCount)
            {
                usernameerror.text = "Username is too long(" + data.MaxCharacterCount + " characters maximum)";
                return false;
            }
            else if (username.Length < data.MinCharacterCount)
            {
                usernameerror.text = "Username is too short(" + data.MinCharacterCount + " characters minimum)";
                return false;
            }
            else
                return true;
        }
    }
}