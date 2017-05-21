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
    public Transform canvasUI;

    public int points;
    public Text pointDisplay;
    public Text timeDisplay;
    public Text countdownText;
    public bool isCountingDown;

    public GameObject gameOverScreen;
    public Text finalPoints;
    public Text gameOverTitleText;
    private bool didWin = false;
    private bool isGameComplete = false;

    public Text pausedText;
    public bool isPaused = false;

    public GameObject shopWindow;
    public bool isShopOpen = false;

    public GameObject starPrefab;
    public Transform starContainer;

    public GameObject clickPrefab;

    private bool isGameActive = false;
    private float timeElapsed;
    private IEnumerator starCoroutine;
    private IEnumerator countdownCoroutine;

    public bool biggerClick = false;
    public bool lightningBolt = false;
    public GameObject lightningBoltButton;
    public bool extraLife = false;

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
                FinishGame();
            }
            if (Input.GetMouseButtonDown(0))
            {
                RectTransform newClick = Instantiate(clickPrefab).GetComponent<RectTransform>();
                newClick.parent = canvasUI;
                if (biggerClick)
                {
                    newClick.sizeDelta = new Vector2(40, 40);
                }
                newClick.transform.position = Input.mousePosition;
            }
        }
	}

    public void NewGame()
    {
        points = 0;
        gameOverScreen.SetActive(false);
        countdownText.gameObject.SetActive(true);
        gameScreen.raycastTarget = false;
        countdownCoroutine = StartCountdown();
        StartCoroutine(countdownCoroutine);
        timeElapsed = 10.0f;
    }

    public void RestartGame()
    {
        if (isShopOpen)
        {
            return;
        }
        if (isPaused)
        {
            PauseGame();
            StopGame();
            NewGame();
        }
        else
        {
            StopGame();
            NewGame();
        }
    }

    public void StopGame()
    {
        if(starCoroutine != null)
            StopCoroutine(starCoroutine);
        if (isCountingDown)
            StopCoroutine(countdownCoroutine);
        isGameActive = false;
        gameScreen.raycastTarget = true;
        timeElapsed = 0;     
        foreach(Transform star in starContainer)
        {
            star.GetComponent<Star>().DisappearThisStar();
        }
    }

    public void FinishGame()
    {
        StopGame();
        isGameComplete = true;
        googleAnalytics.LogEvent("Score", "Submit Score", points.ToString(), 1);
        Analytics.CustomEvent("ButtonPress", new Dictionary<string, object>
        {
            {"Final Score ", points }
        });
        finalPoints.text = points.ToString();
        gameOverTitleText.text = didWin ? "You Won!" : "Great Work";
        gameOverScreen.SetActive(true);        
    }

    public void PauseGame()
    {
        if (isShopOpen || isGameComplete)
        {
            return;
        }
        if (isPaused)
        {
            isPaused = false;
            pausedText.gameObject.SetActive(false);
            Time.timeScale = 1;
            if (isCountingDown)
            {
                countdownText.gameObject.SetActive(true);
            }
        }
        else
        {
            isPaused = true;
            pausedText.gameObject.SetActive(true);
            Time.timeScale = 0;
            if (isCountingDown)
            {
                countdownText.gameObject.SetActive(false);
            }
        }      
    }

    public void UseLighting()
    {
        if (lightningBolt)
        {
            lightningBolt = false;
            lightningBoltButton.SetActive(false);
            foreach (Transform star in starContainer)
            {
                star.GetComponent<Star>().DestroyThisStar();
            }
        }
    }

    public void PressShopButton()
    {
        if (isShopOpen)
        {
            CloseShop();
        }
        else
        {
            OpenShop();
        }
    }

    private void OpenShop()
    {
        googleAnalytics.LogScreen("Shop Screen");
        if (isCountingDown)
        {
            countdownText.gameObject.SetActive(false);
        }
        shopWindow.SetActive(true);
        isShopOpen = true;
        Time.timeScale = 0;
        if (isPaused)
            pausedText.gameObject.SetActive(false);
        if(isGameComplete)
            gameOverScreen.SetActive(false);
    }

    private void CloseShop()
    {
        if (isCountingDown)
        {
            countdownText.gameObject.SetActive(true);
        }
        shopWindow.SetActive(false);
        isShopOpen = false;
        if (!isPaused)
            Time.timeScale = 1;
        else
            pausedText.gameObject.SetActive(true);
        if (isGameComplete)
        {
            gameOverScreen.SetActive(true);
        }
    }

    public void BuyBiggerClick()
    {
        if (!biggerClick)
        {
            googleAnalytics.LogItem("12345", "Bigger Click", "Click_SKU", "Powerups", 100.00, 1);
            googleAnalytics.LogTransaction("12345", "In-Game Store", 100.00, 0, 0);
            Analytics.Transaction("CLICK", 1, "AUD");
            biggerClick = true;
            CloseShop();
        }
    }

    public void BuyLightningBolt()
    {
        if (!lightningBolt)
        {
            googleAnalytics.LogItem("12345", "Lightning Bolt", "Lightning_SKU", "Powerups", 200.00, 1);
            googleAnalytics.LogTransaction("12345", "In-Game Store", 200.00, 0, 0);
            Analytics.Transaction("BOLT", 2, "AUD");
            lightningBolt = true;
            lightningBoltButton.SetActive(true);
            CloseShop();
        }
    }

    public void BuyExtraLife()
    {
        if (!extraLife)
        {
            googleAnalytics.LogItem("12345", "Extra Life", "Life_SKU", "Powerups", 300.00, 1);
            googleAnalytics.LogTransaction("12345", "In-Game Store", 300.00, 0, 0);
            Analytics.Transaction("LIFE", 3, "AUD");
            extraLife = true;
            CloseShop();
        }
    }

    public void Buy10Points()
    {
        googleAnalytics.LogItem("12345", "10 Points", "10Points_SKU", "Points", 500.00, 1);
        googleAnalytics.LogTransaction("12345", "In-Game Store", 500.00, 0, 0);
        Analytics.Transaction("10POINTS", 5, "AUD");
        points += 10;
        CloseShop();
    }

    public void BuyWin()
    {
        googleAnalytics.LogItem("12345", "Win Game", "Win_SKU", "Points", 1000.00, 1);
        googleAnalytics.LogTransaction("12345", "In-Game Store", 1000.00, 0, 0);
        Analytics.Transaction("WIN", 10, "AUD");
        didWin = true;
        CloseShop();
        FinishGame();
    }

    public void ShootStar()
    {
        Star newStar = Instantiate<GameObject>(starPrefab).GetComponent<Star>();
        if (biggerClick)
        {
            newStar.GetComponent<SphereCollider>().radius = 1.5f;
        }
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
        isCountingDown = true;
        isGameComplete = false;
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
        isCountingDown = false;
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
