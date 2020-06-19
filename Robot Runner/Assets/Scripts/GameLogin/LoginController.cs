using UnityEngine;
using PlayFabLogin;
using UnityEngine.UI;
using TMPro;
using Validator;
using AutoLogin;
using System.Collections;

public class LoginController : MonoBehaviour
{
    public TMP_InputField UsernameInput;
    public TMP_InputField PasswordInput;
    public Button SignInButton;
    public Toggle toggle;

    public TextMeshProUGUI UsernameError;
    public TextMeshProUGUI PasswordError;

    public GameObject FailPanel;

    private TMPFieldColorizer colorizer;
    private AutoLoginData autoLoginData;
    [SerializeField]
    private UsernameValidationData usernameValidationData;
    [SerializeField]
    private PasswordValidationData passwordValidationData;
    [SerializeField]
    private ColorizerData colorizerData;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(InitializeStart());
    }
    private void OnEnable()
    {
        UsernameError.text = "";
        PasswordError.text = "";
    }
    private IEnumerator InitializeStart()
    {
        yield return new WaitForEndOfFrame();
        autoLoginData = new AutoLoginData();
        //setting up login panel
        SignInButton.onClick.AddListener(OnSignInClick);
        colorizer = new TMPFieldColorizer(colorizerData);
        colorizer.AddOnSelectHandler(UsernameInput);
        colorizer.AddOnSelectHandler(PasswordInput);
        //Checking if AutoLogin available. If so, it will auto login
        //CheckAutoLogin();

        AddToggleHandler();
    }

    void OnSignInClick()
    {
        SignInButton.enabled = false;
        if (CheckDataValid())
        {
            Login();
        }
        SignInButton.enabled = true;
    }

    private void Login()
    {
        PlayFabLogin.Logger logger = new LoginWithUsername(UsernameInput.text, PasswordInput.text, () => { autoLoginData.Username = UsernameInput.text; autoLoginData.Password = PasswordInput.text; }, () => { SignInButton.enabled = true; FailPanel.SetActive(true); });
        logger.Login();
    }
    //checks if input fields is valid
    bool CheckDataValid()
    {
        UsernameError.text = "";
        PasswordError.text = "";

        IValidate UsernameValidator = new UsernameValidator(UsernameInput.text, usernameValidationData, UsernameError);
        IValidate PasswordValidator = new PasswordValidator(PasswordInput.text, passwordValidationData, PasswordError);

        bool isUsernameValid = UsernameValidator.isValid();
        bool isPasswordValid = PasswordValidator.isValid();

        ColorField(UsernameInput, isUsernameValid);
        ColorField(PasswordInput, isPasswordValid);

        return isUsernameValid && isPasswordValid;
    }
    bool ColorField(TMP_InputField field, bool result)
    {

        Debug.LogFormat("Result is {0} for {1}", result, field.name);
        colorizer.ColorInputField(field, result);
        return result;
    }
    void CheckAutoLogin()
    {
        if (autoLoginData.AutoLogin)
        {

            UsernameInput.text = autoLoginData.Username;
            PasswordInput.text = autoLoginData.Password;
            OnSignInClick();
        }
      
    }

    private void AddToggleHandler()
    {
        var autoLoginController = new AutoLoginController(autoLoginData);
        autoLoginController.AddAutoLoginSwitchHandler(toggle);
        
    }

    public void LoginFail()
    {
        FailPanel.SetActive(true);
    }
}
