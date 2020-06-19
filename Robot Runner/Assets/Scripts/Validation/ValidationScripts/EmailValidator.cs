using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Validator
{
    public class EmailValidator : IValidate
    {
        string email;
        EmailValidationData data;
        TextMeshProUGUI emailerror;

        public EmailValidator(string Email, EmailValidationData Data, TextMeshProUGUI emailerror)
        {
            email = Email;
            data = Data;
            this.emailerror = emailerror;
        }

        public bool isValid()
        {
            return CheckLength() && CheckMail();
        }
        bool CheckLength()
        {
            if (email.Length < data.MaxCharacterCount)
                return true;
            else
            {
                emailerror.text = "Email is too long(" + data.MaxCharacterCount + " characters minimum)";
                return false;
            }
        }
        bool CheckMail()
        {
            try
            {
                System.Net.Mail.MailAddress mail = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                emailerror.text = "Wrong email format";
                return false;
            }
        }
    }
}
