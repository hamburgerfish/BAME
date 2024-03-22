using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doubleJumpParticle : MonoBehaviour
{

    public GameObject doubleJumpParticleRef;
    private GameObject doubleJumpParticleInstance;
    // Start is called before the first frame update
    void Awake()
    {

        Destroy(this.gameObject, 0.33f);
    }

}
