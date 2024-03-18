using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using controller;

public class playerAnimControl : MonoBehaviour
{
    Animator anim;

    private SpriteRenderer playerSpriteRenderer;
    public newPlayerMovement pm;

    [Tooltip("minimum vertical velocity for jump_up_anim")]
    public float jumpUpMinVelocity = 0.5f;

    [Tooltip("minimum vertical velocity for jump_down_anim")]
    public float jumpDownMinVelocity = 0.7f;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (pm._dashing || pm.superDashSuccess) anim.SetBool("isDashing", true);
        else
        {
            anim.SetBool("isDashing", false);
        
            if (pm._grounded)
            {
                anim.SetInteger("jumpState", 0);
                if (pm._frameVelocity.x != 0)  anim.SetBool("isWalking", true);
                else anim.SetBool("isWalking", false);
            }

            else
            {
                if (pm._frameVelocity.y >= jumpUpMinVelocity) anim.SetInteger("jumpState", 1);
                else if (pm._frameVelocity.y <= -jumpDownMinVelocity) anim.SetInteger("jumpState", 3);
                else anim.SetInteger("jumpState", 2);
            }

            if (pm._frameVelocity.x != 0)
            {
                if (pm._frameVelocity.x < 0) playerSpriteRenderer.flipX = true;
                else playerSpriteRenderer.flipX = false;
            }
        }
    }
}
