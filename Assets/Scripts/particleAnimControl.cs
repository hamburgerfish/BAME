using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using controller;

namespace particleAnimController
{
    public class particleAnimControl : MonoBehaviour
    {
        Animator anim;

        private SpriteRenderer particleSpriteRenderer;
        public newPlayerMovement pm;

        public GameObject doubleJumpParticle;
        private GameObject doubleJumpParticleInstance;

        public GameObject jumpDustParticle;
        private GameObject jumpDustParticleInstance1;
        private GameObject jumpDustParticleInstance2;
        private GameObject jumpDustParticleInstance3;

        public GameObject dashParticle;
        private GameObject dashParticleInstance;

        public GameObject superDashParticleInner;
        private GameObject superDashParticleInnerInstance;
        public GameObject superDashParticleOuter;
        private GameObject superDashParticleOuterInstance;


        // Start is called before the first frame update
        void Awake()
        {
            anim = GetComponent<Animator>();
            particleSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        private float _timeJumped;
        public int dashParticleSpawnFrames = 10;
        private float dashParticleSpawnFramesCurrent;
        public int dashFrameTick = 10;
        public bool superDashReleaseThisFrame = false;
        public float superDashParticleRot;
        // Update is called once per frame
        void Update()
        {
            if (pm._dashing || pm.superDashSuccess || pm.superDashReleased || pm._time <= pm._timeDashed + pm.dashTime + 0.2f)
            {
                if (pm.superDashReleased && !superDashReleaseThisFrame)
                {
                    superDashReleaseThisFrame = true;
                    //(0,1)=0   (-1,1)=45   (-1,0)=90   (-1,-1)=135   (0,-1)=180    (1,-1)=225    (1,0)=270    (1,1)=315
                    // - asin(x) + (acos(y))(1-abs(xy)) + atan(xy) Erik's Law
                    superDashParticleRot = (360 * (- Mathf.Asin(pm._frameInput.move.x) + (Mathf.Acos(pm._frameInput.move.y)) * (1-Mathf.Abs(pm._frameInput.move.x * pm._frameInput.move.y)) + Mathf.Atan(pm._frameInput.move.x * pm._frameInput.move.y)))/(2 * Mathf.PI);

                    superDashParticleInnerInstance = Instantiate(superDashParticleInner, new Vector3(pm.player.transform.position.x, pm.player.transform.position.y, 0), Quaternion.Euler(new Vector3 (0, 0, superDashParticleRot)));
                    superDashParticleOuterInstance = Instantiate(superDashParticleOuter, new Vector3(pm.player.transform.position.x, pm.player.transform.position.y, 0), Quaternion.Euler(new Vector3 (0, 0, superDashParticleRot)));
                }
                dashParticleSpawnFramesCurrent = Mathf.Round((1/Time.timeScale) * dashParticleSpawnFrames);
                if (dashFrameTick % dashParticleSpawnFramesCurrent == 0) dashParticleInstance = Instantiate(dashParticle, new Vector3(pm.player.transform.position.x - pm.facingDir * 0.03f, pm.player.transform.position.y, 0), Quaternion.identity);
                dashFrameTick += 1;
            }
            else superDashReleaseThisFrame = false;

            if (pm.createJumpDust && pm._grounded) 
            {
                jumpDust();
                _timeJumped = pm._time;
                pm.createJumpDust = false;
            }

            else if (pm._time == pm._timeDoubleJumped)
            {
                doubleJumpParticleInstance = Instantiate(doubleJumpParticle, new Vector3(pm.player.transform.position.x, pm.player.transform.position.y - 0.15f, 0), Quaternion.identity);
            }
        }

        System.Random rnd = new System.Random();
        private int nDust = 0;
        private Vector3 dustPos1;
        private float dustOffset1;
        private Vector3 dustPos2;
        private float dustOffset2;
        private Vector3 dustPos3;
        private float dustOffset3;
        void jumpDust()
        {
            nDust = rnd.Next(2, 4);
            dustOffset1 = rnd.Next(-4,6)/100.0f;
            dustOffset2 = rnd.Next(-4,6)/100.0f;

            dustPos1 = new Vector3 (pm.player.transform.position.x + dustOffset1, pm.player.transform.position.y - 0.15f, 0);
            dustPos2 = new Vector3 (pm.player.transform.position.x + dustOffset2, pm.player.transform.position.y - 0.15f, 0);

            jumpDustParticleInstance1 = Instantiate(jumpDustParticle, dustPos1, Quaternion.identity);
            jumpDustParticleInstance2 = Instantiate(jumpDustParticle, dustPos2, Quaternion.identity);

            if (nDust == 3)
            {
                dustOffset3 = rnd.Next(-4,6)/100.0f;
                dustPos3 = new Vector3 (pm.player.transform.position.x + dustOffset3, pm.player.transform.position.y - 0.15f, 0);
                jumpDustParticleInstance3 = Instantiate(jumpDustParticle, dustPos3, Quaternion.identity);
            }
            
        }

    }
}