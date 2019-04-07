using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayFabController : MonoBehaviour
{
    public static GamePlayFabController GPFC;
    public SceneManager SM;

    private int playerKillCountThisGame;
    private int playerTotalKills;

    private void OnEnable()
    {
        if (GamePlayFabController.GPFC == null)
        {
            GamePlayFabController.GPFC = this;
        }
        else
        {
            if (GamePlayFabController.GPFC != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void Start()
    {
        //Note: Setting title Id here can be skipped if you have set the value in Editor Extensions already.
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "E5D9";
        }

        playerKillCountThisGame = 0;
        GetStats();
    }

    #region PlayerStats

    //Receive stats from database
    void GetStats()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStatistics,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    //Log each stat to the user
    void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        Debug.Log("Received the following Statistics:");
        foreach (var eachStat in result.Statistics)
        {
            Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
            switch (eachStat.StatisticName)
            {
                case "PlayerKillCount":
                    playerTotalKills = eachStat.Value;
                    break;
            }
        }
    }

    // Build the request object and access the API
    public void StartCloudUpdatePlayerStats()
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdatePlayerStats", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new { pKillCount = playerTotalKills + playerKillCountThisGame }, // The parameter provided to your function
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, OnCloudUpdateStats, OnErrorShared);
    }


    private static void OnCloudUpdateStats(ExecuteCloudScriptResult result)
    {
        // Cloud Script returns arbitrary results, so you have to evaluate them one step and one parameter at a time
        Debug.Log(JsonWrapper.SerializeObject(result.FunctionResult));
        //JsonObject jsonResult = (JsonObject)result.FunctionResult;
        //object messageValue;
        //jsonResult.TryGetValue("messageValue", out messageValue); // note how "messageValue" directly corresponds to the JSON values set in Cloud Script
        //Debug.Log((string)messageValue);
    }

    private static void OnErrorShared(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    public void IncrementKillCount()
    {
        Debug.Log("Incrementing Kill Count...");
        playerKillCountThisGame++;
        GetStats();
        StartCloudUpdatePlayerStats();

    }

    #endregion PlayerStats
}
