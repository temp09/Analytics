using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class IntroScreen : MonoBehaviour {

    public GoogleAnalyticsV4 googleAnalytics;
    public Text inputName;

    public GameObject introScreen;
    public GameObject gameScreen;

    public Text usernameText;

    // Use this for initialization
    void Start()
    {
        googleAnalytics.LogScreen("Intro Screen");
    }


    // Update is called once per frame
    void Update () {
		
	}

    public void PlayGameButton()
    {
        googleAnalytics.LogScreen("Game Screen");
        googleAnalytics.LogEvent("New Player", "Name Entered", inputName.text, 1);
        Analytics.SetUserId(inputName.text);
        usernameText.text = inputName.text + "!";
        GameManager.instance.playerName = inputName.text;      
        GameManager.instance.NewGame();
        introScreen.SetActive(false);
        gameScreen.SetActive(true);
    }
}
