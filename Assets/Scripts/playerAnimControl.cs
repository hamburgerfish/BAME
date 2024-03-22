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
        if (!pm._active)
        {
            anim.SetInteger("jumpState", 4);
            if (pm.rightSplat)
            {
                anim.SetBool("isWallSplat", true);
                return;
            }
            else if (pm.leftSplat)
            {
                anim.SetBool("isWallSplat", true);
                playerSpriteRenderer.flipX = true;
                return;
            }
            else
            {
                anim.SetBool("isSplat", true);
                return;
            }
        }
        else 
        {
            anim.SetBool("isSplat", false);
            anim.SetBool("isWallSplat", false);
            anim.SetBool("isDashing", false);
            anim.SetBool("isCrouching", false);
            anim.SetBool("isGliding", false);
            anim.SetBool("isWallSliding", false);
        }

        if (pm._frameVelocity.x != 0)
            {
                if (pm._frameVelocity.x < 0) playerSpriteRenderer.flipX = true;
                else playerSpriteRenderer.flipX = false;
            }

        if (pm._dashing || pm.superDashSuccess || pm._time <= pm._timeDashed + pm.dashTime + 0.2f)
        {
            anim.SetBool("isDashing", true);
            return;
        }
        else
        {        
            if (pm._grounded)
            {
                anim.SetInteger("jumpState", 0);
                if (pm._frameVelocity.x != 0)  anim.SetBool("isWalking", true);
                else 
                {
                    anim.SetBool("isWalking", false);
                    if (pm._frameInput.move.y < 0) anim.SetBool("isCrouching", true);
                }
            }

            else
            {
                if (pm._wallSlide)
                {
                    anim.SetBool("isWallSliding", true);
                    if (pm._wallJumpDir == -1) playerSpriteRenderer.flipX = true;
                    else playerSpriteRenderer.flipX = false;
                    anim.SetInteger("jumpState", 4);
                    return;
                }

                if (pm.canUseGlide && pm._frameInput.glide) anim.SetBool("isGliding", true);

                if (pm._frameVelocity.y >= jumpUpMinVelocity) anim.SetInteger("jumpState", 1);
                else if (pm._frameVelocity.y <= -jumpDownMinVelocity) anim.SetInteger("jumpState", 3);
                else anim.SetInteger("jumpState", 2);
            }
        }
    }
}
