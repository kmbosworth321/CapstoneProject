﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private static string Name;
    private static string AvatarChoice;

    void Start()
    {
        Debug.Log("Created PlayerData object");
        DontDestroyOnLoad(this.gameObject);
        AvatarChoice = "";
    }

    void Update()
    {
        
    }

    public string GetName()
    {
        return Name;
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public string GetAvatarChoice()
    {
        return AvatarChoice;
    }

    public void SetAvatarChoice(int avatarChoice)
    {
        Debug.Log("SetAvatarChoice test");
        if(avatarChoice == 0)//use polymorphic behavior for better results. something like ...
        {
            AvatarChoice = "KyleRobot";
        }
        else if (avatarChoice == 1)//use polymorphic behavior for better results. something like ...
        {
            AvatarChoice = "UnityChan";
        }
        else
        {
            Debug.Log("Error: avatar not found");
        }
        Debug.Log("SetAvatarChoice avatar Choice: " + AvatarChoice);
       
    }

}
