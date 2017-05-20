using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Analytical : MonoBehaviour {

    int myPoints = 10;
    int birthYear = 1991;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PressTheButton()
    {
        Analytics.CustomEvent("ButtonPress", new Dictionary<string, object>
        {
            {"points ", myPoints }
        });
    }

    public void GiveMeMoney()
    {
        Analytics.Transaction("GiveMoneys", 0.5m, "AUD", null, null);
    }

    public void Birthday()
    {
        Analytics.SetUserBirthYear(birthYear);
        Analytics.SetUserGender(Gender.Male);
    }
}
