using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class GameManager : MonoBehaviour {

    public GoogleAnalyticsV4 googleAnalytics;


    public static GameManager instance;
    public string playerName;

    public Image gameScreen;

    public int points;
    public Text pointDisplay;
    public Text timeDisplay;
    public Text countdownText;

    public GameObject gameOverScreen;
    public Text finalPoints;

    public GameObject starPrefab;
    public Transform starContainer;

    private bool isGameActive = false;
    private float timeElapsed;
    private IEnumerator starCoroutine;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
        {
            Destroy(this);
        }
    }

    // Use this for initialization
	void Start () {
        points = 0;
	}
	
	// Update is called once per frame
	void Update () {
        pointDisplay.text = points.ToString();
        timeDisplay.text = timeElapsed.ToString("0.00");
        if (isGameActive)
        {
            timeElapsed -= Time.deltaTime;
            if(timeElapsed <= 0.0f)
            {
                StopGame();
            }
        }
	}

    public void NewGame()
    {
        points = 0;
        gameOverScreen.SetActive(false);
        countdownText.gameObject.SetActive(true);
        gameScreen.raycastTarget = false;
        StartCoroutine(StartCountdown());
        timeElapsed = 10.0f;
    }

    public void StopGame()
    {
        googleAnalytics.LogEvent("Score", "Submit Score", "Final ScorE", points);
        Analytics.CustomEvent("ButtonPress", new Dictionary<string, object>
        {
            {"Final Score ", points }
        });
        StopCoroutine(starCoroutine);
        gameOverScreen.SetActive(true);
        isGameActive = false;
        gameScreen.raycastTarget = true;
        timeElapsed = 0;
        finalPoints.text = points.ToString();
        foreach(Transform star in starContainer)
        {
            star.GetComponent<Star>().DisappearThisStar();
        }
    }

    public void ShootStar()
    {
        Star newStar = Instantiate<GameObject>(starPrefab).GetComponent<Star>();
        newStar.transform.parent = starContainer;
        float xPos, zPos;
        float xTarget, zTarget;
        bool spawnOnSide = Random.value < 0.75 ? true : false;

        if (spawnOnSide)
        {
            if (Random.value < 0.5)
                xPos = -4;
            else
                xPos = 4;
            xTarget = -xPos;
            zPos = Random.Range(-5.0f, 5.0f);
            zTarget = Random.Range(-5.0f, 5.0f);
        }
        else
        {
            if (Random.value < 0.5)
                zPos = -6;
            else
                zPos = 6;
            zTarget = -zPos;
            xPos = Random.Range(-3.0f, 3.0f);
            xTarget = Random.Range(-3.0f, 3.0f);
        }

        newStar.transform.position = new Vector3(xPos, 0, zPos);
        newStar.targetPos = new Vector3(xTarget, 0, zTarget);
        newStar.speed = Random.Range(3.0f, 6.0f);
        newStar.rotSpeed = Random.Range(0.1f, 2f);
    }

    public IEnumerator StartCountdown()
    {
        countdownText.text = "Ready?";
        yield return new WaitForSeconds(1);
        countdownText.text = 3.ToString();
        yield return new WaitForSeconds(1);
        countdownText.text = 2.ToString();
        yield return new WaitForSeconds(1);
        countdownText.text = 1.ToString();
        yield return new WaitForSeconds(1);
        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);
        isGameActive = true;
        countdownText.gameObject.SetActive(false);
        starCoroutine = CreateStars();
        StartCoroutine(starCoroutine);
    }

    public IEnumerator CreateStars()
    {
        while (true)
        {
            ShootStar();
            yield return new WaitForSeconds(Random.Range(0.2f, 0.6f));
        }
    }
}
