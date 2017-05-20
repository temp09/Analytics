using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarExplosion : MonoBehaviour {

    private float timeElapsed = 0;

    public ParticleSystem gold;
    public ParticleSystem silver;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > 2.0f)
            Destroy(this.gameObject);
	}
}
