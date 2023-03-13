using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class PlayerController : MonoBehaviour, IAttacker
{
    #region Move
    public float MovingSpeed
    {
        get
        {
            if (!CanMove)
            {
                return 0;
            }
            if (touchingDirections.IsOnWall)
            {
                return 0;
            }
            float temp = Math.Min(baseSpeed * (1 + speedMulti) + extraSpeed, maxSpeed);
            return temp > 0 ? temp : 0f;
        }
    }

    public bool CanMove => animator.GetBool(AnimationStrings.canMove);

    public float JumpImpulse => jumpBaseImpulse * jumpAbility;

    [SerializeField] 
    private float jumpBaseImpulse = 8f;

    private float jumpAbility = 1f;
    private float baseSpeed = 5f;

    [SerializeField] 
    private float extraSpeed = 0f;

    [SerializeField] 
    private float speedMulti = 0f;

    public float maxSpeed = 10f;

    private Vector2 moveInput;

    [SerializeField]
    private uint canJumpTimes = 2;

    private uint _jumpTimes;
    public uint JumpTimes
    {
        get => _jumpTimes;
        set => _jumpTimes = Math.Min(value, canJumpTimes);
    }

    [SerializeField]
    private bool _isMoving = false;

    public bool IsMoving
    {
        get => _isMoving;
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    [SerializeField]
    private bool _isRunning = false;

    public bool IsRunning
    {
        get => _isRunning;
        set
        {
            _isRunning = value;
            animator.SetBool(AnimationStrings.isRunning, value);
        }
    }

    public bool _IsFacingRight = true;

    public bool IsFacingRight
    {
        get => _IsFacingRight;
        private set
        {
            if (_IsFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }

            _IsFacingRight = value;
        }
    }

    public bool LockVelocity => animator.GetBool(AnimationStrings.lockVelocity);
    #endregion

    #region Attack

    private BaseDamageProperty damageProperty = new BaseDamageProperty(10, 0, 0, 0, 1);

    public BaseDamageProperty DamageProperty() => damageProperty;

    #endregion

    public bool IsAlive => animator.GetBool(AnimationStrings.isAlive);

    private Rigidbody2D rb;
    private Animator animator;
    private TouchingDirections touchingDirections;
    [SerializeField]
    private Vector2 knockVector2PerFrame = Vector2.zero;   //因为与物理相关的函数，是每次在FixedUpdate中调用的，因此需要在这个地方保存一下集中处理

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        StateMachineBehaviour[] behaviours = animator.GetBehaviours(Animator.StringToHash("Base Layer.GroundStates"), 0);
        foreach (StateMachineBehaviour behaviour in behaviours)
        {
            if (behaviour.GetType() == typeof(SetEventBehaviour))
            {
                SetEventBehaviour temp = (SetEventBehaviour)behaviour;
                temp.eventList.AddListener(OnBackToGround);
                break;
            }
        }
    }

    void Start()
    {
        JumpTimes = canJumpTimes;
    }

    void Update()
    {
        if (touchingDirections.IsOnGround)
        {
            transform.position = transform.position + new Vector3(Time.deltaTime * MovingSpeed * moveInput.x, 0, 0);
            //rb.velocity = new Vector2(moveInput.x * MovingSpeed, rb.velocity.y);
        }
    }

    private void FixedUpdate()
    {
        if (!touchingDirections.IsOnGround)
        {
            rb.velocity = new Vector2(moveInput.x * MovingSpeed, rb.velocity.y);
        }
        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
        if (knockVector2PerFrame != Vector2.zero)
        {
            Debug.Log(knockVector2PerFrame);
            rb.velocity += knockVector2PerFrame;
            knockVector2PerFrame = Vector2.zero;
        }
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            //face right
            IsFacingRight = true;
        }
        else if (moveInput.x < 0)
        {
            //face left
            IsFacingRight = false;
        }
    }

    //重回地面时调用
    public void OnBackToGround()
    {
        Debug.Log("123231");
        JumpTimes = canJumpTimes;
        // 消除空中速度在地上的滑行
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!IsAlive && !LockVelocity)
        {
            IsMoving = false;
            return;
        }
        moveInput = context.ReadValue<Vector2>();
        IsMoving = moveInput != Vector2.zero;
        SetFacingDirection(moveInput);
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true;
            extraSpeed += 3f;
        }
        else if (context.canceled)
        {
            IsRunning = false;
            extraSpeed -= 3f;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && CanMove && JumpTimes > 0)
        {
            JumpTimes -= 1;
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            animator.ResetTrigger(AnimationStrings.attackTrigger);
            rb.velocity = new Vector2(rb.velocity.x, JumpImpulse);
        }
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsOnGround)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }

    public void OnHit(int damage, Vector2 knockback, GameObject attacker)
    {
        knockVector2PerFrame += knockback;
    }
}
