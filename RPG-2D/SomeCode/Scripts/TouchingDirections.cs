using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����ײ�����жϵ�ǰ��ײ�����ķ���
[RequireComponent(typeof(CapsuleCollider2D), typeof(Animator))]
public class TouchingDirections : MonoBehaviour
{
    public ContactFilter2D castFilter;  //��ʾ��ײ���ã���һ��struct������������ײ��layer���Ƿ����trigger��
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
        //����ײ������ͶӰ�����ײ���ų�����ģ�ͣ��������������ײ����>0��������������ڵ����ϵġ�
        IsOnGround = touchingCollider.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
        IsOnCeiling = touchingCollider.Cast(Vector2.up, castFilter, ceilingHits, ceilingDistance) > 0;
        IsOnWall = touchingCollider.Cast(wallCheckDirection, castFilter, wallHits, wallDistance) > 0;
    }
}
