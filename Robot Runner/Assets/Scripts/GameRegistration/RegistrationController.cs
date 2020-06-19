using PlayFab;
using PlayFab.ClientModels;
using PlayFabRegistration;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Validator;

public class RegistrationController : MonoBehaviour
{
    public TMP_InputField UsernameInput;
    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput;
    public TMP_InputField PasswordConfirmInput;

    public TextMeshProUGUI UsernameError;
    public TextMeshProUGUI EmailError;
    public TextMeshProUGUI PasswordError;
    public TextMeshProUGUI PasswordConfirmError;

    public Button SignUpButton;

    private TMPFieldColorizer colorizer;

    [SerializeField]
    private UsernameValidationData usernameValidationData;
    [SerializeField]
    private EmailValidationData emailValidationData;
    [SerializeField]
    private PasswordValidationData passwordValidationData;
    [SerializeField]
    private ColorizerData colorizerData;
    [SerializeField]
    private LoginRegisterLayersChange LayerNavigator;

    // Start is called before the first frame update
    void Start()
    {
        SignUpButton.onClick.AddListener(OnSignUpClick);
        colorizer = new TMPFieldColorizer(colorizerData);
        colorizer.AddOnSelectHandler(UsernameInput);
        colorizer.AddOnSelectHandler(EmailInput);
        colorizer.AddOnSelectHandler(PasswordInput);
        colorizer.AddOnSelectHandler(PasswordConfirmInput);
    }
    private void OnEnable()
    {
        UsernameError.text = "";
        EmailError.text = "";
        PasswordError.text = "";
        PasswordConfirmError.text = "";
    }
    void OnSignUpClick()
    {
        SignUpButton.enabled = false;
        if (CheckRegistrationDataValid())
        {
            Debug.Log("Hey");
            Registrator registration = new RegistrationWithEmail(EmailInput.text, UsernameInput.text, PasswordInput.text, OnSuccesfulRegistration, () => SignUpButton.enabled = true);
            registration.Registration(UsernameError);
        }
        SignUpButton.enabled = true;
    }
    bool CheckRegistrationDataValid()
    {
        UsernameError.text = "";
        EmailError.text = "";
        PasswordError.text = "";
        PasswordConfirmError.text = "";

        IValidate UsernameValidator = new UsernameValidator(UsernameInput.text, usernameValidationData, UsernameError);
        IValidate EmailValidator = new EmailValidator(EmailInput.text, emailValidationData, EmailError);
        IValidate PasswordValidator = new PasswordValidator(PasswordInput.text, passwordValidationData, PasswordError);

        bool isUsernameValid = UsernameValidator.isValid();
        bool isEmailValid = EmailValidator.isValid();
        bool isPasswordValid = PasswordValidator.isValid();

        ColorField(UsernameInput, isUsernameValid);
        ColorField(EmailInput, isEmailValid);

        isPasswordValid = CheckPassword(isPasswordValid);



        return isUsernameValid && isEmailValid && isPasswordValid;
    }

    bool CheckPassword(bool isValid)
    {
        if (!isValid)
        {
            ColorField(PasswordInput, isValid);
            ColorField(PasswordConfirmInput, isValid);
            return false;
        }
        else
        {
            if (PasswordInput.text != PasswordConfirmInput.text)
            {
                ColorField(PasswordInput, false);
                ColorField(PasswordConfirmInput, false);
                PasswordConfirmError.text = "Password do not match";
                return false;
            }
            return true;
        }
    }
    void ColorField(TMP_InputField field, bool result)
    {
        Debug.LogFormat("Result is {0} for {1}", result, field.name);
        colorizer.ColorInputField(field, result);
    }
    void OnSuccesfulRegistration()
    {
        PlayFabLogin.Logger logger = new PlayFabLogin.LoginWithUsername(UsernameInput.text, PasswordInput.text);
        logger.Login();
    }
}
