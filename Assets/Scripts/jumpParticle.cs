using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumpParticle : MonoBehaviour
{
    private SpriteRenderer particleSpriteRenderer;

    public GameObject jumpParticleRef;
    private GameObject jumpParticleInstance;

    private int randNum;
    System.Random rnd = new System.Random();

    // Start is called before the first frame update
    void Awake()
    {
        particleSpriteRenderer = GetComponent<SpriteRenderer>();

        randNum = rnd.Next(0,2);
        if (randNum == 0) particleSpriteRenderer.flipX = true;
        else particleSpriteRenderer.flipX = false;

        Destroy(this.gameObject, 0.33f);
    }
}
