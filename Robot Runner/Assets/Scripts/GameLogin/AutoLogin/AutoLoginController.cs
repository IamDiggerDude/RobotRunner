using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace AutoLogin
{
    public class AutoLoginController 
    {
        AutoLoginData data;

        public AutoLoginController(AutoLoginData data)
        {
            this.data = data;
        }

        public void AddAutoLoginSwitchHandler(Toggle toggle)
        {
            //data.AutoLogin=  toggle.isOn;
            toggle.onValueChanged.AddListener(ToggleAutoLoginHandler);
        }
        public void ToggleAutoLoginHandler(bool isAuto)
        {
            data.AutoLogin = isAuto;
            Debug.LogFormat("Is auto login available {0}", data.AutoLogin);
        }
        public void ResetData()
        {
            data.AutoLogin = false;
            data.Password = AutoLoginData.DefaultPref;
            data.Username = AutoLoginData.DefaultPref;
        }
    }
}
