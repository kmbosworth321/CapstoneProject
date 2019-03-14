using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabController : MonoBehaviour
{
    public static PlayFabController PFC;

    private string userEmail;
    private string userPassword;
    private string username;
    public GameObject loginPanel;
    public GameObject addLoginPanel;
    public GameObject recoverButton;

    private void OnEnable() {
        if (PlayFabController.PFC == null) {
            PlayFabController.PFC = this;
        }
        else { 
            if (PlayFabController.PFC != this) {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void Start()
    {
        //Used currently to counteract the autologin setting
        PlayerPrefs.DeleteAll();

        //Note: Setting title Id here can be skipped if you have set the value in Editor Extensions already.
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "E5D9";
        }
        //var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
        //PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);


        if (PlayerPrefs.HasKey("EMAIL"))
        {
            userEmail = PlayerPrefs.GetString("EMAIL");
            userPassword = PlayerPrefs.GetString("PASSWORD");
            var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }
        else {
#if UNITY_ANDROID
            var requestAndroid = new LoginWithAndroidDeviceIDRequest { AndroidDeviceId = returnMobileID(), CreateAccount = true };
            PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid, OnLoginMobileSuccess, OnLoginMobileFailure);
#endif
#if UNITY_IOS
            var requestIOS = new LoginWithIOSDeviceIDRequest { DeviceId = returnMobileID(), CreateAccount = true };
            PlayFabClientAPI.LoginWithIOSDeviceID(requestIOS, OnLoginMobileSuccess, OnLoginMobileFailure);
#endif
        }
              
    }

    #region Login
    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login Success!");
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        loginPanel.SetActive(false);
        recoverButton.SetActive(false);
        getStats();
    }

    private void OnLoginMobileSuccess(LoginResult result)
    {
        Debug.Log("Mobile Login Success!");
        getStats();
        loginPanel.SetActive(false);
    }

    private void onRegisterSuccess(RegisterPlayFabUserResult result) {
        Debug.Log("Registration Success!");
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        getStats();
        loginPanel.SetActive(false);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        var registerRequest = new RegisterPlayFabUserRequest { Email = userEmail, Password = userPassword, Username = username};
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, onRegisterSuccess, onRegisterFailure);
    }

    private void OnLoginMobileFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    private void onRegisterFailure(PlayFabError error) {
        Debug.LogError(error.GenerateErrorReport());
    }

    public void GetUserEmail(string emailIn) {
        userEmail = emailIn;
    }

    public void getUserPassword(string passwordIn) {
        userPassword = passwordIn;
    }

    public void getUsername(string usernameIn) {
        username = usernameIn;
    }

    public void onClickLogin() {
        var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    public static string returnMobileID() {
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        return deviceID;
    }

    public void openAddLogin() {
        addLoginPanel.SetActive(true);
    }

    public void onClickAddLogin() {
        var addLoginRequest = new AddUsernamePasswordRequest { Email = userEmail, Password = userPassword, Username = username };
        PlayFabClientAPI.AddUsernamePassword(addLoginRequest, onAddLoginSuccess, onRegisterFailure);
    }

    private void onAddLoginSuccess(AddUsernamePasswordResult result)
    {
        Debug.Log("Login Success!");
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        getStats();
        addLoginPanel.SetActive(false);
    }
    #endregion Login

    public int playerLevel;
    public int gameLevel;

    public int playerHealth;
    public int playerDamage;

    public int playerHighScore;

    #region PlayerStats

    public void setStats() { 
        PlayFabClientAPI.UpdatePlayerStatistics( new UpdatePlayerStatisticsRequest {
            // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
            Statistics = new List<StatisticUpdate> {
            new StatisticUpdate { StatisticName = "PlayerLevel", Value = playerLevel },
            new StatisticUpdate { StatisticName = "GameLevel", Value = gameLevel },
            new StatisticUpdate { StatisticName = "PlayerHealth", Value = playerHealth },
            new StatisticUpdate { StatisticName = "PlayerDamage", Value = playerDamage },
            new StatisticUpdate { StatisticName = "PlayerHighScore", Value = playerHighScore },
            }
        },
        result => { Debug.Log("User statistics updated"); },
        error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    void getStats() {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStatistics,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        Debug.Log("Received the following Statistics:");
        foreach (var eachStat in result.Statistics)
        {
            Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
            switch(eachStat.StatisticName) {
                case "PlayerLevel":
                    playerLevel = eachStat.Value;
                    break;
                case "GameLevel":
                    gameLevel = eachStat.Value;
                    break;
                case "PlayerHealth":
                    playerHealth = eachStat.Value;
                    break;
                case "PlayerDamage":
                    playerDamage = eachStat.Value;
                    break;
                case "PlayerHighScore":
                    playerHighScore = eachStat.Value;
                    break;
            }
        }
    }

    #endregion PlayerStats
}