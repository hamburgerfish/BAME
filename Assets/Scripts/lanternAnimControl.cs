using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using controller;
using particleAnimController;

public class lanternAnimControl : MonoBehaviour
{
    Animator anim;
    public GameObject player;
    private newPlayerMovement pm;
    private particleAnimControl pac;

    private SpriteRenderer lanternSpriteRenderer;


    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        pm = player.GetComponent<newPlayerMovement>();
        pac = player.GetComponent<particleAnimControl>();
        lanternSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public bool superDashPotential = false;
    // Update is called once per frame
    void Update()
    {
        if (Physics2D.OverlapCircle(new Vector2(this.transform.position.x, this.transform.position.y), 0.1f, pm.PlayerLayer) && pm.lanternTouch && !superDashPotential)
        {
            superDashPotential = true;
            anim.SetBool("lanternActive", true);
        }

        else if (superDashPotential && !pm.superDashSuccess)
        {
            if (pm.superDashReleased)
            {
                lanternSpriteRenderer.enabled = false;
                superDashPotential = false;
                anim.SetBool("lanternActive", false);
            }
            else if (pm._usedSuperDash)
            {
                superDashPotential = false;
                anim.SetBool("lanternActive", false);
            }
        }
    }
}
