using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Star : MonoBehaviour, IPointerClickHandler {

    public Collider myCollider;
    public GameManager gameManager;

    public GameObject starExplosion;

    public Vector3 targetPos;
    public float speed;
    public float rotSpeed;

    // Use this for initialization
    void Start () {
        gameManager = GameManager.instance;
    }
	
	// Update is called once per frame
	void Update () {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
        float rotStep = rotSpeed * 180   * Time.deltaTime;
        transform.Rotate(Vector3.forward, rotStep);
	}

    void DestroyThisStar()
    {
        StarExplosion starParticles = Instantiate(starExplosion).GetComponent<StarExplosion>();
        starParticles.transform.position = transform.position;
        ParticleSystem.VelocityOverLifetimeModule goldVoL = starParticles.gold.velocityOverLifetime;
        ParticleSystem.VelocityOverLifetimeModule silvVoL = starParticles.gold.velocityOverLifetime;
        Vector3 myvel = targetPos - transform.position;
        goldVoL.xMultiplier = myvel.x;
        goldVoL.zMultiplier = myvel.z;
        silvVoL.xMultiplier = myvel.x;
        silvVoL.zMultiplier = myvel.z;
        Destroy(gameObject);
    }

    public void DisappearThisStar()
    {
        targetPos = transform.position;
        StartCoroutine(ShrinkThenDestroy());
    }

    IEnumerator ShrinkThenDestroy()
    {
        for (int i = 0; i < 20; i++) {
            transform.localScale -= new Vector3(0.025f, 0.025f, 0.025f);
            yield return new WaitForSeconds(0.05f);
        }

        Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        gameManager.points++;
        DestroyThisStar();
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Boundary")
        {
            Destroy(gameObject);
        }
    }
}
