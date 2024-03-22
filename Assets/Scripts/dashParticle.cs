using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class dashParticle : MonoBehaviour
{
    public GameObject dashParticleRef;

    public float particleDuration = 0.5f;
    private GameObject dashParticleInstance;

    // Start is called before the first frame update
    void Awake()
    {
        Destroy(this.gameObject, particleDuration);
    }
}
