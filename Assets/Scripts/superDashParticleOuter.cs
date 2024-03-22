using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class superDashParticleOuter : MonoBehaviour
{

    public GameObject superDashParticleOuterRef;
    private GameObject superDashParticleOuterInstance;

    public float particleMoveSpeed = 0.04f;
    private float rot;
    private float moveDirX;
    private float moveDirY;

    // Start is called before the first frame update
    void Awake()
    {
        rot = this.transform.eulerAngles.z;
        moveDirY = -Mathf.Cos(2 * Mathf.PI * rot / 360);
        moveDirX = Mathf.Sin(2 * Mathf.PI * rot / 360);
        Destroy(this.gameObject, 0.33f);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(transform.position.x + (particleMoveSpeed * moveDirX), transform.position.y + (particleMoveSpeed * moveDirY), 0);
    }
}