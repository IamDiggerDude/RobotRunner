using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginRegisterLayersChange : MonoBehaviour
{
    public Button ToSignUp;
    public Button ToSignIn;
    public GameObject SignInLayer;
    public GameObject SignUpLayer;

    private void Start()
    {
        ToSignUp.onClick.AddListener(OnToSignUp);
        ToSignIn.onClick.AddListener(OnToSignIn);
    }
    public void OnToSignUp()
    {
        SignInLayer.SetActive(false);
        SignUpLayer.SetActive(true);
    }
    public void OnToSignIn()
    {
        SignInLayer.SetActive(true);
        SignUpLayer.SetActive(false);
    }
}
