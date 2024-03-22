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

    private bool superDashPotential;
    public float superDashLanternCooldown = 3.0f;
    private float _timeLanternUsed;
    // Update is called once per frame
    void Update()
    {
        if (pm._time >= _timeLanternUsed + superDashLanternCooldown)
        {
            lanternSpriteRenderer.enabled = true;
            if (pm._dashing && Physics2D.OverlapCircle(new Vector2(this.transform.position.x, this.transform.position.y), 0.1f, pm.PlayerLayer) && !superDashPotential)
            {
                pm.lanternTouch = true;
                pm._usedSuperDash = false;
                pm._timeHitLantern = pm._time;
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
                    _timeLanternUsed = pm._time;
                }
                else if (pm._usedSuperDash)
                {
                    superDashPotential = false;
                    anim.SetBool("lanternActive", false);
                }
            }
        }
    }
}
