using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace controller
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class newPlayerMovement : MonoBehaviour
    {
        public GameObject player;

        [Header("LAYERS")] [Tooltip("Set this to the layer your player is on")]
        public LayerMask PlayerLayer;
        public LayerMask LanternLayer;

        [Header("INPUT")] [Tooltip("Makes all Input snap to an integer. Prevents gamepads from walking slowly. Recommended value is true to ensure gamepad/keybaord parity.")]
        public bool SnapInput = true;

        [Tooltip("Minimum input required before you mount a ladder or climb a ledge. Avoids unwanted climbing using controllers"), Range(0.01f, 0.99f)]
        public float VerticalDeadZoneThreshold = 0.3f;

        [Tooltip("Minimum input required before a left or right is recognized. Avoids drifting with sticky controllers"), Range(0.01f, 0.99f)]
        public float HorizontalDeadZoneThreshold = 0.1f;

        [Header("MOVEMENT")] [Tooltip("The top horizontal movement speed")]
        public float MaxSpeed = 2;

        [Tooltip("The player's capacity to gain horizontal speed")]
        public float Acceleration = 20;

        [Tooltip("The pace at which the player comes to a stop")]
        public float GroundDeceleration = 10;

        [Tooltip("Deceleration in air only after stopping input mid-air")]
        public float AirDeceleration = 3;

        [Tooltip("A constant downward force applied while grounded. Helps on slopes"), Range(0f, -10f)]
        public float GroundingForce = -1.8f;

        [Tooltip("The detection distance for grounding and roof detection"), Range(0f, 0.5f)]
        public float GrounderDistance = 0.03f;

        [Header("JUMP")] [Tooltip("The immediate velocity applied when jumping")]
        public float JumpPower = 2;

        [Tooltip("The maximum vertical movement speed")]
        public float MaxFallSpeed = 3;

        [Tooltip("The player's capacity to gain fall speed. a.k.a. In Air Gravity")]
        public float FallAcceleration = 4;

        [Tooltip("The gravity multiplier added when jump is released early")]
        public float JumpEndEarlyGravityModifier = 3;

        [Tooltip("The time before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge")]
        public float CoyoteTime = .15f;

        [Tooltip("The amount of time we buffer a jump. This allows jump input before actually hitting the ground")]
        public float JumpBuffer = .2f;

        [Tooltip("The maximum vertical movement speed when sliding on wall")]
        public float wallSpeed = 0.2f;

        [Tooltip("Deceleration from sliding on wall")]
        public float wallDeceleration = 3;

        [Tooltip("The immediate vertical velocity applied when wall jumping")]
        public float wallJumpVerticalPower = 1;

        [Tooltip("The immediate horizontal velocity applied when wall jumping")]
        public float wallJumpHorizontalPower = 1;
        
        [Tooltip("The time before player can go towards the wall after wall jumping")]
        public float wallJumpTimer = 0.2f;

        [Tooltip("The time stuck to wall while holding away")]
        public float wallJumpStuckTimer = 0.1f;

        [Tooltip("The time it takes to respawn")]
        public float respawnTimer = 1.0f;

        [Tooltip("The perpendicular velocity against floor or wall to splat")]
        public float splatVelocity = 3;
        
        [Tooltip("The maximum vertical fall velocity while gliding")]
        public float maxGlideFallSpeed = 0.4f;

        [Tooltip("The gravity modifier when using glide when going up")]
        public float upwardGlideGravityModifier = 2;

        [Tooltip("The horizontal deceleration when gliding")]
        public float horizontalGlideDeceleration = 10;

        [Tooltip("Delay between pressing glide key and starting glide")]
        public float glideDelay = 0.2f;

        [Tooltip("Horizontal velocity of dash")]
        public float dashHorizontalVelocity = 4;

        [Tooltip("Vertical velocity during dash")]
        public float dashVerticalVelocity = 10;

        [Tooltip("How long the dash lasts")]
        public float dashTime = 0.2f;

        [Tooltip("Cooldown of dash")]
        public float dashCooldown = 0.1f;

        [Tooltip("The deceleration while maintaining momentum and holding against direction of velocity")]
        public float extremeHorizontalMaxDeceleration = 10;

        [Tooltip("The deceleration while maintaining momentum and holding towards direction of velocity")]
        public float extremeHorizontalMinDeceleration = 3;

        [Tooltip("The deceleration while maintaining momentum and holding towards direction of velocity and gliding")]
        public float extremeHorizontalGlideMinDeceleration = 2;

        [Tooltip("The deceleration while maintaining momentum and holding against direction of velocity and gliding")]
        public float extremeHorizontalGlideMaxDeceleration = 20;

        [Tooltip("Buffer time between update and fixedupdate")]
        public float updateBuffer = 0.05f;

        [Tooltip("upgraded vertical velocity applied when wall jumping")]
        public float upgradedWallJumpVerticalPower = 1.5f;

        [Tooltip("upgraded time before player can move towards the wall after wall jumping")]
        public float upgradedWallJumpTimer = 0.1f;

        [Tooltip("super dash time slow down factor")]
        public float superDashTimescale = 0.02f;

        [Tooltip("super dash vertical release force")]
        public float superDashVerticalReleaseForce = 3;

        [Tooltip("super dash horizontal release force")]
        public float superDashHorizontalReleaseForce = 5;

        [Tooltip("hold dash duration to enter super dash")]
        public float timeDashHeldForSuper = 0.15f;

        [Tooltip("maximum time in slowmo")]
        public float maxSlowMoTime = 0.2f;



        //upgrades
        public bool doubleJumpUnlocked = true;
        public bool splatResistanceUnlocked = true;
        public bool glideUnlocked = true;
        public bool dashUnlocked = true;
        public bool betterWallJumpUnlocked = true;
        public bool superDashUnlocked = true;

    

        //private ScriptableStats _stats;
        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private BoxCollider2D _bcol;
        public FrameInput _frameInput;
        public Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;
        public bool _active = true;
        public Vector2 _respawnPoint;


        public float _time;
        private float fixedDeltaTime;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<CapsuleCollider2D>();
            _bcol = GetComponent<BoxCollider2D>();
            _respawnPoint = player.transform.position;

            if (betterWallJumpUnlocked)
            {
                wallJumpVerticalPower = upgradedWallJumpVerticalPower;
                wallJumpTimer = upgradedWallJumpTimer;
            }

            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;

            this.fixedDeltaTime = Time.fixedDeltaTime;
        }

        // Update is called once per frame
        private void Update()
        {
            _time += Time.deltaTime;
            if (!_active) return;
            GatherInput();
        }

        private bool _dashQueue;
        private void GatherInput()
        {
            _frameInput = new FrameInput
            {
                jumpDown = Input.GetButtonDown("Jump"),
                jumpHeld = Input.GetButton("Jump"),
                move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
                glide = Input.GetKey(KeyCode.L),
                glidePressed = Input.GetKeyDown(KeyCode.L),
                dash = Input.GetKey(KeyCode.K),
                dashPressed = Input.GetKeyDown(KeyCode.K)
            };

            if (SnapInput)
            {
                _frameInput.move.x = Mathf.Abs(_frameInput.move.x) < HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.move.x);
                _frameInput.move.y = Mathf.Abs(_frameInput.move.y) < VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.move.y);
            }

            if (_frameInput.jumpDown && !superDashSuccess)
            {
                _jumpToConsume = true;
                _timeJumpWasPressed = _time;
            }

            if (_frameInput.glidePressed)
            {
                _timeGlideWasPressed = _time;
            }
            
            if (_frameInput.dashPressed)
            {
                _dashQueue = true;
                _timeGlideWasPressed = _time + dashTime;
            }
            if (_frameInput.dash)
            {
                _timeDashHeld = _time;
            }
            else
            {
                _timeDashHeld = 0.0f;
                _timeHitLantern = float.PositiveInfinity;
            }
        }


        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("pit")) splat();
        }

        private void FixedUpdate()
        {
            if (!_active)
            {
                respawn();
                return;
            }

            CheckCollisions();

            HandleJump();
            HandleDirection();
            HandleGravity();

            ApplyMovement();

            inputPurge();
        }


        public bool camReset=false;
        private void respawn()
        {
            if (_time >= timeOfDeath + respawnTimer)
            {
                player.transform.position = _respawnPoint;
                _active = true;
                camReset = true;
                rightSplat = false;
                leftSplat = false;
                _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }

        private float timeOfDeath;
        private void splat()
        {
            _active = false;
            _coyoteUsable = false;
            _doubleJumpFlag = false;
            _frameVelocity.x = 0;
            _frameVelocity.y = 0;
            _frameInput.move.x = 0;
            _frameInput.move.y = 0;
            _bufferedJumpUseable = false;
            _jumpToConsume = false;
           _rb.constraints = RigidbodyConstraints2D.FreezeAll;
            timeOfDeath = _time;
            _dashing = false;
        }


        #region Collisions

        private float _frameLeftGrounded = float.MinValue;
        public bool _grounded;
        public bool _wallSlide;
        private bool _wallJumpAvailable = false;
        public int _wallJumpDir;
        public bool rightSplat = false;
        public bool leftSplat = false;

        //upgrades
        private bool _doubleJumpUseable;
        private bool _doubleJumpFlag;
        private bool _dashFlag;

        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;

            // Ground and Ceiling
            bool groundHit = Physics2D.BoxCast(_bcol.bounds.center, _bcol.size, 0, Vector2.down, GrounderDistance, PlayerLayer);
            bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, GrounderDistance, PlayerLayer);

            // Walls
            bool wallHitRight = Physics2D.BoxCast(_bcol.bounds.center, _bcol.size, 0, Vector2.right, 0.02f, PlayerLayer);
            bool wallHitLeft = Physics2D.BoxCast(_bcol.bounds.center, _bcol.size, 0, Vector2.left, 0.02f, PlayerLayer);


            // Hit Wall
            if (wallHitRight)
            {
                if (!splatResistanceUnlocked && _frameVelocity.x >= splatVelocity)
                {
                    splat();
                    rightSplat = true;
                    return;
                }
                _frameVelocity.x = Mathf.Min(0, _frameVelocity.x);
                _wallJumpDir = -1;
                if (!_grounded && !_dashing) facingDir = -1;
                _wallSlide = true;
                unstick = false;
                if (_frameVelocity.y <= wallSpeed) _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -wallSpeed, wallDeceleration * Time.fixedDeltaTime);
            }

            else if (wallHitLeft) 
            {
                if (!splatResistanceUnlocked && _frameVelocity.x <= -splatVelocity)
                {
                    splat();
                    leftSplat = true;
                    return;
                }
                _frameVelocity.x = Mathf.Max(0, _frameVelocity.x);
                _wallJumpDir = 1;
                if (!_grounded && !_dashing) facingDir = 1;
                _wallSlide = true;
                unstick = false;
                if (_frameVelocity.y <= wallSpeed) _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -wallSpeed, wallDeceleration * Time.fixedDeltaTime);
            }
            else
            {
                _wallSlide = false;
                _wallJumpDir = 0;
                unstick = true;
            }

            // Hit Ceiling
            if (ceilingHit)
            {
                _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);
                if (!splatResistanceUnlocked && _frameVelocity.y >= splatVelocity)
                {
                    splat();
                    return;
                }
            }

            // Landed on the Ground
            if (!_grounded && groundHit)
            {
                if (!splatResistanceUnlocked && _frameVelocity.y <= -splatVelocity)
                {
                    splat();
                    return;
                }

                _grounded = true;
                _coyoteUsable = true;
                _bufferedJumpUseable = true;
                _wallSlide = false;
                _wallJumpAvailable = false;
                _doubleJumpUseable = false;
                _doubleJumpFlag = true;
                _usedSuperDash = false;
            }

            // Left the Ground
            else if (_grounded && !groundHit)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                _doubleJumpUseable = true;
            }

            else if (!_grounded && !groundHit && _wallSlide)
            {
                _coyoteUsable = false;
                _bufferedJumpUseable = true;
                _wallJumpAvailable = true;
                _doubleJumpUseable = false;
            }

            else if (!_grounded && !groundHit && !_wallSlide)
            {
                _wallJumpAvailable = false;
                _doubleJumpUseable = true;
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }

        #endregion



        #region Jumping

        private bool _jumpToConsume;
        private bool _bufferedJumpUseable;
        public bool _endedJumpEarly;
        private bool _coyoteUsable;
        public float _timeJumpWasPressed;
        private float _frameWallJumped;
        private bool unstick = true;
        private bool hasBufferedJump => _bufferedJumpUseable && _time < _timeJumpWasPressed + JumpBuffer;
        private bool canUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + CoyoteTime;
        public bool createJumpDust;

        //upgrades
        private bool canUseDoubleJump => doubleJumpUnlocked && _doubleJumpUseable && _doubleJumpFlag;
        public float _timeDoubleJumped;
        public bool _dashing = false;

        private void HandleJump()
        {
            if (!_endedJumpEarly && !_grounded && !_frameInput.jumpHeld && _rb.velocity.y > 0 && !_usedSuperDash && !_dashing) _endedJumpEarly = true;
            if (_rb.velocity.y < 0) _endedJumpEarly = false;

            if (!_jumpToConsume && !hasBufferedJump || superDashSuccess) return;

            if (_grounded || canUseCoyote) executeJump();

            else if (_wallJumpAvailable && !_dashing) executeWallJump();

            else if (canUseDoubleJump) executeDoubleJump();

            _jumpToConsume = false;
        }

        private void executeJump()
        {
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUseable = false;
            _coyoteUsable = false;
            _frameVelocity.y = JumpPower;
            if (_dashing) _dashing = false;
            _usedSuperDash = false;
            createJumpDust = true;
        }

        private void executeDoubleJump()
        {
            _frameVelocity.y = 0;
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUseable = false;
            _coyoteUsable = false;
            _frameVelocity.y = JumpPower;
            _frameWallJumped = _time - wallJumpTimer;
            _doubleJumpFlag = false;
            if (_dashing) _dashing = false;
            _usedSuperDash = false;
            _timeDoubleJumped = _time;
        }

        private void executeWallJump()
        {
            _endedJumpEarly = false;
            _frameWallJumped = _time;
            _timeJumpWasPressed = 0;
            _bufferedJumpUseable = false;
            _frameVelocity.y = wallJumpVerticalPower;
            _frameVelocity.x = wallJumpHorizontalPower * _wallJumpDir;
            _usedSuperDash = false;
            unstick = true;
        }

        #endregion

        #region Horizontal

        private float timeHeldRight;
        private float timeHeldLeft;
        private int heldDir;
        public int facingDir = 1;

        //upgrades
        public bool canUseGlide => glideUnlocked && !_grounded && !_wallSlide && _time > _timeGlideWasPressed + glideDelay;
        private bool canUseDash => dashUnlocked && _dashFlag;
        public float _timeDashed;
        private float _timeDashWasPressed;
        public float _timeDashHeld;
        public bool superDashSuccess;
        public bool _usedSuperDash;
        public bool lanternTouch; // used in lanternAnimControl.cs
        public float _timeHitLantern; // used in lanternAnimControl.cs
        public float _timeSuperDashed;
        public bool superDashReleased;

        private void HandleDirection()
        {
            if (_frameInput.move.x > 0 && timeHeldRight == 0)
            {
                heldDir = 1;
                timeHeldRight = _time;
                timeHeldLeft = 0.0f;
            }
            if (_frameInput.move.x < 0 && timeHeldLeft == 0)
            {
                heldDir = -1;
                timeHeldLeft = _time;
                timeHeldRight = 0.0f;
            }

            if (_frameInput.move.x > 0 && !_dashing) facingDir = 1;
            else if (_frameInput.move.x < 0 && !_dashing) facingDir = -1;
            
            if (Mathf.Abs(_frameVelocity.x) <= Mathf.Abs(MaxSpeed) && Mathf.Abs(_frameVelocity.y) <= MaxFallSpeed) superDashReleased = false;

            if (_grounded) _dashFlag = true;

            if ((_dashQueue && canUseDash && _time > _timeDashed + dashTime + dashCooldown) || _dashing)
            {
                if (!_dashing)
                {
                    _timeDashed = _time;
                    _frameVelocity.y = 0;
                    _dashing = true;
                    _dashFlag = false;
                    _dashQueue = false;
                }
                _frameVelocity.x = dashHorizontalVelocity * facingDir;
                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, dashVerticalVelocity * _frameInput.move.y, FallAcceleration * Time.fixedDeltaTime);
                // super dash
                if (_time > _timeDashed + dashTime)
                {
                    _dashing = false;
                }
            }
            else if (superDashUnlocked && (_timeDashHeld - _timeHitLantern >= timeDashHeldForSuper) && !superDashSuccess)
            {
                superDashSuccess = true;
                _timeSuperDashed = _time;
            }
            else if (superDashSuccess)
            {
                if (_time < _timeSuperDashed + maxSlowMoTime && _frameInput.dash)
                {
                    Time.timeScale = Mathf.MoveTowards(Time.timeScale, superDashTimescale, Time.timeScale * 0.1f);
                }
                else
                {
                    superDashSuccess = false;
                    _usedSuperDash = true;
                    lanternTouch = false;
                    _timeHitLantern = float.PositiveInfinity;
                    Time.timeScale = 1;
                    if (_frameInput.move.x != 0 || _frameInput.move.y != 0)
                    {
                        _frameVelocity.x = superDashHorizontalReleaseForce * _frameInput.move.x;
                        _frameVelocity.y = superDashVerticalReleaseForce * _frameInput.move.y;
                        _dashFlag = true;
                        superDashReleased = true;
                    }
                }
                Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
            }

            else
            {
                _dashing = false;
                lanternTouch = false;
                if (_frameInput.move.x == 0 && !_dashing)
                {
                    heldDir = 0;
                    timeHeldLeft = 0.0f;
                    timeHeldRight = 0.0f;
                    var deceleration = _grounded ? GroundDeceleration : AirDeceleration;
                    if (Mathf.Abs(_frameVelocity.x) > Mathf.Abs(MaxSpeed)) deceleration = extremeHorizontalMaxDeceleration;
                    else if (canUseGlide && _frameInput.glide) deceleration = horizontalGlideDeceleration;
                    _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
                }

                else if (_grounded || !_wallSlide || (!_dashing && _wallSlide && (unstick || (heldDir * _time <= -timeHeldLeft - wallJumpStuckTimer || heldDir * _time >= timeHeldRight + wallJumpStuckTimer))))
                {
                    if (_time >= wallJumpTimer + _frameWallJumped || _frameInput.move.x * _wallJumpDir > 0)
                    {
                        var currentAcceleration = Acceleration;
                        if (Mathf.Abs(_frameVelocity.x) > Mathf.Abs(MaxSpeed))
                        {
                            if (_frameVelocity.x * heldDir > 0)
                            {
                                if (canUseGlide && _frameInput.glide) currentAcceleration = extremeHorizontalGlideMinDeceleration;
                                else currentAcceleration = extremeHorizontalMinDeceleration;
                            }
                            else
                            {
                                if (canUseGlide && _frameInput.glide) currentAcceleration = extremeHorizontalGlideMaxDeceleration;
                                else currentAcceleration = extremeHorizontalMaxDeceleration;
                            }
                        }
                        _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.move.x * MaxSpeed, currentAcceleration * Time.fixedDeltaTime);
                    }
                }
                else _frameVelocity.x = 0;
            }
        }

        #endregion

        #region Gravity

        private float _timeGlideWasPressed;

        private void HandleGravity()
        {
            if (_grounded)
            {
                if (!_dashing)
                {
                    _timeGlideWasPressed = float.PositiveInfinity;
                    if (_frameVelocity.y <= 0f)
                    {
                        _frameVelocity.y = GroundingForce;
                    }
                }
            }

            else
            {
                var inAirGravity = FallAcceleration;
                var maxFall = MaxFallSpeed;
                if (_endedJumpEarly && _frameVelocity.y > 0 && !_dashing) inAirGravity *= JumpEndEarlyGravityModifier;
                else if (canUseGlide && _frameInput.glide)
                {
                    if (_frameVelocity.y > 0) inAirGravity *= upwardGlideGravityModifier;
                    _dashing = false;
                    maxFall = maxGlideFallSpeed;
                }
                if (!_dashing) _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -maxFall, inAirGravity * Time.fixedDeltaTime);
            }
        }

        #endregion

        private void inputPurge()
        {
            _dashQueue = false;
        }


        private void ApplyMovement() => _rb.velocity = _frameVelocity;
    }


    public struct FrameInput
    {
        public bool jumpDown;
        public bool jumpHeld;
        public Vector2 move;
        public bool glide;
        public bool glidePressed;
        public bool dash;
        public bool dashPressed;
    }
}