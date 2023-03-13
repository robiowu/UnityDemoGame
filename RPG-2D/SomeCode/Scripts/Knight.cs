using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Animator))]
public class Knight : MonoBehaviour, IAttacker
{
    #region 移动相关
    public bool CanMove => animator.GetBool(AnimationStrings.canMove);
    private float baseSpeed = 3f;
    [SerializeField]
    private float extraSpeed = 0f;
    [SerializeField]
    private float speedMulti = 0f;
    public float maxSpeed = 10f;
    public float MovingSpeed
    {
        get
        {
            if (!CanMove)
            {
                return 0;
            }
            float temp = Math.Min(baseSpeed * (1 + speedMulti) + extraSpeed, maxSpeed);
            return temp > 0 ? temp : 0f;
        }
    }

    public enum WalkableDirection { Right, Left }

    private Vector2 walkDirectionVector;

    private WalkableDirection _walkDirection;

    public WalkableDirection WalkDirection
    {
        get => _walkDirection;
        set
        {
            if (_walkDirection != value)
            {
                transform.localScale *= new Vector2(-1, 1);
                if (value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                }
                else if (value == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left;
                }
            }
            _walkDirection = value;
        }
    }

    #endregion

    #region 战斗相关

    private bool _hasTarget;
    public bool HasTarget
    {
        get => _hasTarget;
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    private BaseDamageProperty damageProperty = new BaseDamageProperty(10, 0, 0, 0, 1);

    public BaseDamageProperty DamageProperty() => damageProperty;

    #endregion


    public DetectionZone attackZone;
    private Rigidbody2D rb;
    private TouchingDirections touchingDirections;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        walkDirectionVector = (transform.localScale.x > 0) ? Vector2.right : Vector2.left;
    }

    void Update()
    {
        //transform.position = transform.position + new Vector3(Time.deltaTime * MovingSpeed * Vector2.left.x, 0, 0);
        HasTarget = attackZone.detectedColliders.Count > 0;
    }

    private void FixedUpdate()
    {
        if (CanMove)
        {
            if (touchingDirections.IsOnGround && touchingDirections.IsOnWall)
            {
                FlipDirection();
            }
            if (touchingDirections.IsOnGround)
            {
                rb.velocity = new Vector2(MovingSpeed * walkDirectionVector.x, rb.velocity.y);
            }
        }
        else
            rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private void FlipDirection()
    {
        WalkDirection = (WalkDirection == WalkableDirection.Right) ? WalkableDirection.Left : WalkableDirection.Right;
    }
}
