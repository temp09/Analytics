using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickIndicator : MonoBehaviour {

    private Image rend;
    private float timeElapsed = 0;
    private float startAlpha;
    private float lossPerSecond;

    public float fadeOutTime = 0.5f;

    private void Awake()
    {
        rend = gameObject.GetComponent<Image>();
    }

    // Use this for initialization
    void Start () {
        startAlpha = rend.color.a;
        lossPerSecond = startAlpha / fadeOutTime;
	}

    private void Update()
    {
        float step = Time.deltaTime * lossPerSecond;
        timeElapsed += Time.deltaTime;
        if (timeElapsed > fadeOutTime)
            Destroy(gameObject);
        else
        {
            Color color = rend.color;
            color.a -= step;
            rend.color = color;
        }
    }
}
