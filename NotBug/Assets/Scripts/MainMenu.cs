﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using System;

public class MainMenu : MonoBehaviour {

    public float fadingTime = 0.8f;
    public Image completedImageTouchscreen;
    public Image completedImageAccelerometer;
    public bool isConnectedToGoogleServices = false;
    public GameObject unableToConnect;
    public Sprite[] toggleBorderSprites;
    public Toggle toggleTutorial;

    private Fading fading;

    private void Start()
    {
        Controller.gameMode = GameMode.Pause;
        fading = gameObject.GetComponentInChildren<Fading>();
        fading.FadeIn(fadingTime);

        if (Controller.showTutorialToggle == 1)
            toggleTutorial.isOn = true;
        else
            toggleTutorial.isOn = false;


        RectTransform[] rectTransforms = GetComponentsInChildren<RectTransform>(true);
        foreach (RectTransform rt in rectTransforms)
        {
            if (rt.name == "MainMenu")
                rt.gameObject.SetActive(true);
            if (rt.name == "PlayMenu")
                rt.gameObject.SetActive(false);
            if (rt.name == "OptionsMenu")
                rt.gameObject.SetActive(false);
            if (rt.name == "Leaderboards")
                rt.gameObject.SetActive(false);

            GoogleServicesInit();
            ConnectToGoogleServices();
        }
    }

    public void onQuitClick()
    {
        Application.Quit();
    }


    public void GoogleServicesInit()
    {
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.DebugLogEnabled = false;
    }

    public bool ConnectToGoogleServices()
    {
        if (!isConnectedToGoogleServices)
        {
            Social.localUser.Authenticate((bool success) =>
            {
                isConnectedToGoogleServices = success;
            });
        }
        return isConnectedToGoogleServices;
    }


    // Main menu

    public void onPlayButtonClick()
    {
        if (Controller.completedTouchscreen == 1)
            completedImageTouchscreen.enabled = true;
        else if (Controller.completedAccelerometer == 1)
            completedImageAccelerometer.enabled = true;
    }


    // Leaderboards

    public void onLeaderboardsClick()
    {
        ConnectToGoogleServices();
        if (Social.localUser.authenticated)
        {
            Social.ShowLeaderboardUI();
        }
        else
        {
            ShowUnableToConnect();
        }
    }

    private void ShowUnableToConnect()
    {
        unableToConnect.SetActive(true);
    }


    // Play menu

    public void onTouchscreenButtonClick()
    {
        Controller.moveControl = MoveControl.Touchscreen;
        Controller.Instance.startFromCheckpoint = true;
        StartCoroutine("ChangeScene", SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void onAccelerometerButtonClick()
    {
        Controller.moveControl = MoveControl.Accelerometer;
        Controller.Instance.startFromCheckpoint = true;
        StartCoroutine("ChangeScene", SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator ChangeScene(int scene)
    {
        fading.FadeOut(fadingTime);
        yield return new WaitForSeconds(fadingTime);
        SceneManager.LoadScene(scene);
    }

    public void onToggleTutorialClick()
    {
        Image[] images = toggleTutorial.GetComponentsInChildren<Image>();
        Image bg = null;
        foreach (Image image in images)
        {
            if (image.name == "Background")
                bg = image;
        }

        if (toggleTutorial.isOn)
        {
            Controller.showTutorialToggle = 1;
            bg.sprite = toggleBorderSprites[0];
        }
        else
        {
            Controller.showTutorialToggle = 0;
            bg.sprite = toggleBorderSprites[1];
        }
    }



    // Options menu

    public void onEffectsVolumeSlider(Slider slider)
    {
        Controller.Instance.GetComponent<AudioManager>().sourceEffects.volume = slider.value;
    }

    public void onOptionsClickUpdateEffectsSlider(Slider effectsSlider)
    {
        effectsSlider.value = Controller.Instance.GetComponent<AudioManager>().sourceEffects.volume;
    }

    public void onMusicVolumeSlider(Slider slider)
    {
        Controller.Instance.GetComponent<AudioManager>().sourceMusic.volume = slider.value;
    } 

    public void onOptionsClickUpdateMusicSlider(Slider musicSlider)
    {      
        musicSlider.value = Controller.Instance.GetComponent<AudioManager>().sourceMusic.volume;
    }

    public void onTrackSlider(Slider slider)
    {
        AudioManager audio = Controller.Instance.GetComponent<AudioManager>();

        float segmentLength = 1.0f / audio.music.Length;
        float segment = 0;
        int track = -1;
        int prevTrack = Controller.Instance.track;

        do
        {
            segment += segmentLength;
            track++;
        }
        while (slider.value >= segment);

        slider.value = segment - segmentLength / 1000;

        if (track != prevTrack)
        {          
            Controller.Instance.track = track;
            audio.PlayMusic(Controller.Instance.track);
        }
    }

    public void onOptionsClickUpdateTrackSlider(Slider trackSlider)
    {
        AudioManager audio = Controller.Instance.GetComponent<AudioManager>();
        float segmentLength = 1.0f / audio.music.Length;

        trackSlider.value = Controller.Instance.track * segmentLength + segmentLength - segmentLength / 1000;
    }



    // Music

    public void onNextTrackClick()
    {
        AudioManager audio = Controller.Instance.GetComponent<AudioManager>();


        if (Controller.Instance.track >= audio.music.Length - 1)
            Controller.Instance.track = 0;
        else
            ++Controller.Instance.track;

        audio.PlayMusic(Controller.Instance.track);
    }

    public void onPreviousTrackClick()
    {
        AudioManager audio = Controller.Instance.GetComponent<AudioManager>();

        if (Controller.Instance.track == 0)
            Controller.Instance.track = audio.music.Length - 1;
        else
            --Controller.Instance.track;

        audio.PlayMusic(Controller.Instance.track);
    }



    // Debug

    //public void onNewGameDebugClick()
    //{
    //    DataManager.SaveLoadNewGame();
    //}
}
