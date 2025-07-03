using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    // UI
    private Text coinInfo;

    // Coin
    public int coin;

    // Animation
    public Animator animator;

    // Movement
    private float horizontal;
    private float vertical;
    public float speed = 8f;

    // Jump
    public float kekuatanLompat = 16f;
    private float jumpStartTime;
    private float timeInAir;
    private const float fallingThreshold = 1f;

    // Flip face
    private bool hadapKanan = true;

    // RigidBody
    [SerializeField] private Rigidbody2D rigidBody;

    // Custom Collider
    [SerializeField] private Collider2D customCollider2D;

    // Ground Layer
    [SerializeField] private LayerMask groundLayer;
    private bool grounded;

    // Collisions
    private float time;
    public static bool jumpToConsume;
    private bool bufferedJumpUsable;
    private bool endedJumpEarly;
    public static bool coyoteUsable;
    private float timeJumpWasPressed;
    private float frameLeftGrounded = float.MinValue;
    private Vector2 frameVelocity;

    // Stats
    [SerializeField] private ScriptStats stats;

    // Cached Query
    private bool cachedQueryStartInColliders;

    // Frame Input
    private bool jumpDown;
    private bool jumpHeld;
    private Vector2 move;

    void Start()
    {
        // cache untuk collider groundcheck(groundhit dan ceilingHit dari capsulecast)
        // Mengambil component text untuk coin
        cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        coinInfo = GameObject.Find("UI_coin").GetComponent<Text>();
    }

    void Update()
    {
        time += Time.deltaTime;
        UpdateUI();
        GatherInput();
        HandleJump();
        FlipFace();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        CheckCollisions();
        HandleDirection();
        HandleGravity();
        ApplyMovement();
    }

    // Handler update UI
    private void UpdateUI()
    {
        coinInfo.text = $"Koin: {coin}";
    }

    // Method mengembalikan true atau false berdasarkan capsulecast
    private bool IsGrounded()
    {
        bool groundHit = Physics2D.CapsuleCast(customCollider2D.bounds.center, customCollider2D.bounds.size, CapsuleDirection2D.Vertical, 0, Vector2.down, stats.GrounderDistance, ~stats.PlayerLayer);
        grounded = groundHit;
        return grounded;
    }

    // Handler flipface
    private void FlipFace()
    {   
        if (!PauseMenuScript.isPause)
        {
            if ((horizontal > 0 && !hadapKanan) || (horizontal < 0 && hadapKanan))
            {
                hadapKanan = !hadapKanan;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }
    }

    // Handler mendengar input
    private void GatherInput()
    {
        if (!PauseMenuScript.isPause)
        {
            // 2 tipe varible lompat untuk saat tombol dipencent dan di tekan lama
            jumpDown = Input.GetButtonDown("Jump");
            jumpHeld = Input.GetButton("Jump");

            // 
            move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            horizontal = move.x;
            vertical = move.y;

            // Mengatasi stickdrift dalam controller
            if (stats.SnapInput)
            {
                move.x = Mathf.Abs(move.x) < stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(move.x);
                move.y = Mathf.Abs(move.y) < stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(move.y);
            }

            if (jumpDown)
            {
                jumpToConsume = true;
                timeJumpWasPressed = time;
            }
        }
    }

    // Handler animator
    private void UpdateAnimator()
    {
        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        if (!grounded)
        {
            timeInAir += Time.deltaTime;
            animator.SetBool("Falling", timeInAir >= fallingThreshold);
        }
        else
        {
            timeInAir = 0;
            animator.SetBool("Falling", false);
        }

        if (grounded && animator.GetBool("Jumping") && Time.time > jumpStartTime + 0.1f)
        {
            Debug.Log("Jumping set to false");
            animator.SetBool("Jumping", false);
        }
    }

    // Handler collisions
    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        grounded = IsGrounded();

        // Capsuecast untuk posisi atas karakter
        bool ceilingHit = Physics2D.CapsuleCast(customCollider2D.bounds.center, customCollider2D.bounds.size, CapsuleDirection2D.Vertical, 0, Vector2.up, stats.GrounderDistance, groundLayer);

        if (ceilingHit)
        {
            frameVelocity.y = Mathf.Min(0, frameVelocity.y);
        }

        if (!grounded)
        {
            frameLeftGrounded = time;
        }

        Physics2D.queriesStartInColliders = cachedQueryStartInColliders;
    }

    // Tidak dipakai
    private bool HasBufferedJump()
    {
        return bufferedJumpUsable && time < timeJumpWasPressed + stats.JumpBuffer;
    }

    private bool CanUseCoyote()
    {
        return coyoteUsable && time < frameLeftGrounded + stats.CoyoteTime;
    }

    // Handler input jump
    private void HandleJump()
    {
        if (jumpToConsume && (grounded || CanUseCoyote()))
        {
            ExecuteJump();
            animator.SetBool("Jumping", true);
            jumpStartTime = Time.time;
        }   
        else if (!jumpToConsume && !HasBufferedJump() && !grounded && !jumpHeld && rigidBody.velocity.y > 0)
        {
            endedJumpEarly = true;
        }

        jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        endedJumpEarly = false;
        timeJumpWasPressed = 0;
        bufferedJumpUsable = false;
        coyoteUsable = false;

        frameVelocity.y = stats.JumpPower;
        animator.SetBool("Jumping", true);
        animator.SetBool("Falling", false);
    }

    // Handler pergerakan
    private void HandleDirection()
    {
        // Deceleration saat player tidak memberi input
        if (move.x == 0)
        {
            float deceleration = grounded ? stats.GroundDeceleration : stats.AirDeceleration;
            frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, move.x * stats.MaxSpeed, stats.Acceleration * Time.fixedDeltaTime);
        }
    }

    // Handler gravity / kecepatan karakter jatuh
    private void HandleGravity()
    {
        if (grounded && frameVelocity.y <= 0f)
        {
            frameVelocity.y = stats.GroundingForce;
        }
        else
        {
            float inAirGravity = stats.FallAcceleration;
            if (endedJumpEarly && frameVelocity.y > 0)
            {
                inAirGravity *= stats.JumpEndEarlyGravityModifier;
            }
            frameVelocity.y = Mathf.MoveTowards(frameVelocity.y, -stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    private void ApplyMovement()
    {
        rigidBody.velocity = frameVelocity;
    }

    // Method to handle jumping from the orb
    public void JumpFromOrb()
    {
        if (rigidBody.velocity.y <= 0)
        {
            ExecuteJump();
        }
        else if (rigidBody.velocity.y > 0)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, stats.JumpPower);
        }
    }
}
