using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用碰撞箱来判断当前碰撞发生的方向
[RequireComponent(typeof(CapsuleCollider2D), typeof(Animator))]
public class TouchingDirections : MonoBehaviour
{
    public ContactFilter2D castFilter;  //表示碰撞设置，是一个struct，可以设置碰撞的layer，是否忽略trigger等
    public float groundDistance = 0.05f;
    public float ceilingDistance = 0.05f;
    public float wallDistance = 0.2f;

    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    private CapsuleCollider2D touchingCollider;
    private RaycastHit2D[] groundHits = new RaycastHit2D[5];
    private RaycastHit2D[] ceilingHits = new RaycastHit2D[5];
    private RaycastHit2D[] wallHits = new RaycastHit2D[5];
    private Animator animator;

    private bool _isOnGround;
    public bool IsOnGround
    {
        get => _isOnGround;
        private set
        {
            _isOnGround = value;
            animator.SetBool(AnimationStrings.isOnGround, _isOnGround);
        }
    }

    private bool _IsOnCeiling;
    public bool IsOnCeiling
    {
        get => _IsOnCeiling;
        private set
        {
            _IsOnCeiling = value;
            animator.SetBool(AnimationStrings.isOnCeiling, _IsOnCeiling);
        }
    }

    private bool _IsOnWall;
    public bool IsOnWall
    {
        get => _IsOnWall;
        private set
        {
            _IsOnWall = value;
            animator.SetBool(AnimationStrings.isOnWall, _IsOnWall);
            //if (value)
            //{
            //    Debug.Log(wallCheckDirection);
            //    Debug.Log(wallHits);
            //}
        }
    }

    void Awake()
    {
        touchingCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        //从碰撞箱向下投影检测碰撞（排除自身模型），如果检测出的碰撞箱数>0，则代表我们是在地面上的。
        IsOnGround = touchingCollider.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
        IsOnCeiling = touchingCollider.Cast(Vector2.up, castFilter, ceilingHits, ceilingDistance) > 0;
        IsOnWall = touchingCollider.Cast(wallCheckDirection, castFilter, wallHits, wallDistance) > 0;
    }
}
