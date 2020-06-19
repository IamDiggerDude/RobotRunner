using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PlayFabRegistration {
    public abstract class Registrator
    {
        public Registrator(OnRegistrationSuccess successHandler, OnRegistrationFail failHandler)
        {

        }

        public delegate void OnRegistrationSuccess();
        public delegate void OnRegistrationFail();
        public abstract void Registration(TextMeshProUGUI PlayfabErrorField);
    }
}
