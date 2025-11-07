using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class MenuSystem : MonoBehaviour
{

    /* needs a lot:
     * lobby finding
     * lobby making
     * 
     * keybind changing
     * sound, chanigng
     * screen size
     * 
     * statistics, got the stats worked out on paper
     * 
     * tutorial
     * 
     * leaderboard, got the stats worked out on paper
     * 
     * credits
     */





    // Variables

    // GameObjects
    public GameObject gameName;

    public GameObject play;
    public GameObject playBackButton;
    public GameObject hostButton;
    public GameObject clientButton;

    public GameObject settings;
    public GameObject settingsBackButton;
    public GameObject logOut;

    public GameObject statistics;
    public GameObject tutorial;
    public GameObject leaderboard;
    public GameObject credits;
    public GameObject quit;
    // GameObject accessors

    // Other files
    public BackgroundHandler backgroundHandler;
    public LoginSystem loginSystem;
    public GameSystem gameSystem;

    public void DisplayMainMenu(bool booli)
    {
        gameName.SetActive(booli);
        play.SetActive(booli);
        settings.SetActive(booli);
        statistics.SetActive(booli);
        tutorial.SetActive(booli);
        leaderboard.SetActive(booli);
        credits.SetActive(booli);
        quit.SetActive(booli);
    }

    public void Play()
    {
        DisplayMainMenu(false);
        DisplayPlay(true);
    }
    public void DisplayPlay(bool booli)
    {
        playBackButton.SetActive(booli);
        hostButton.SetActive(booli);
        clientButton.SetActive(booli);
    }
    public void HostGame() // CHANGE TO CREATE LOBBY PERHAPS
    {
        backgroundHandler.HideBackground();
        DisplayPlay(false);
        DisplayMainMenu(false);
        gameSystem.StartGame();
    }
    public void JoinGame()
    {
        backgroundHandler.HideBackground();
        DisplayPlay(false);
        DisplayMainMenu(false);
        gameSystem.JoinGame();
    }
    public void PlayBack()
    {
        DisplayPlay(false);
        DisplayMainMenu(true);
    }


    public void Settings()
    {
        DisplaySettings(true);
        DisplayMainMenu(false);
    }
    public void DisplaySettings(bool booli)
    {
        logOut.SetActive(booli);
        settingsBackButton.SetActive(booli);
    }
    public void LogOut()
    {
        logOut.SetActive(false);
        settingsBackButton.SetActive(false);
        backgroundHandler.LoginBackground();
        loginSystem.DisplayMainScreen(true);
        // if stats are fetched then make all null
    }
    public void SettingsBack()
    {
        DisplaySettings(false);
        DisplayMainMenu(true);
    }


    public void Statistics()
    {

    }


    public void Tutorial()
    {

    }


    public void Leaderboard()
    {

    }


    public void Credits()
    {
        // miko for everything
        // kacper for music and documentation colour scheme
        // danny for bullet sound effect moral support
        // danny adin luca for name?
        // luca for skins and battlepass
        // yagiz for helpful sheets + tips and tricks
        // kaon moral support + villain + loser
        // moutaz for good nish tactics
    }

    /*
    public void Quit()
    {
        // Quits game
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }*/
}