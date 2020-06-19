using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PlayFabLogin
{
    public abstract class Logger
    {
        public delegate void OnLoginSuccess();
        public delegate void OnLoginFailure();
        public abstract void Login();
    }
}