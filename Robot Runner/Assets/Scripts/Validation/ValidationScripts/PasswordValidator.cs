using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Validator
{
    public class PasswordValidator : IValidate
    {
        string[] digits = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
        string password;
        PasswordValidationData data;
        TextMeshProUGUI passworderror;

        public PasswordValidator(string password, PasswordValidationData data, TextMeshProUGUI passworderror)
        {
            this.password = password;
            this.data = data;
            this.passworderror = passworderror;
        }

        public bool isValid()
        {
            //   Debug.LogFormat("Checking Password: {0}",(CheckLength() && CheckCapital()));
            return CheckLength() && CheckCapital() && CheckDigits();
        }
        bool CheckLength()
        {
            //   Debug.LogFormat("Checking Length: {0}", (password.Length < data.MaxCharactersCount && password.Length > data.MinCharactersCount));

            if (password.Length > data.MaxCharactersCount)
            {
                passworderror.text = "Password is too long(" + data.MaxCharactersCount + " characters maximum)";
                return false;
            }
            else if (password.Length < data.MinCharactersCount)
            {
                passworderror.text = "Password is too short(" + data.MinCharactersCount + " characters minimum)";
                return false;
            }
            else
                return true;
        }
        bool CheckCapital()
        {
            if (data.RequestCapitalLetters)
            {

                if (!(password == password.ToLower()))
                    return true;
                else
                {
                    passworderror.text = "Password requires at least 1 Uppercase character";
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        bool CheckDigits()
        {
            if (data.RequestDigits)
            {
                foreach (string digit in digits)
                {
                    if (password.Contains(digit))
                        return true;
                }
                passworderror.text = "Password requires at least 1 digit";
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}