using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("移动参数")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float airControl = 0.8f;
    
    [Header("地面检测")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;
    
    [Header("视觉效果")]
    public bool facingRight = true;
    public Animator animator;
    
    // 组件引用
    private Rigidbody2D rb;
    
    // 状态变量
    private bool isGrounded;
    private bool isJumping;
    private float moveInput;
    
    // 输入变量
    private bool jumpInput;

    void Start()
    {
        // 获取组件
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        // 创建地面检测点（如果没有）
        if (groundCheck == null)
        {
            CreateGroundCheck();
        }
    }

    void Update()
    {
        GetInput();
        CheckGround();
        HandleAnimation();
        HandleJumpInput();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleJumpPhysics();
    }

    void GetInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetButtonDown("Jump");
    }

    void CheckGround()
    {
        // 使用圆形检测地面
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        // 如果在地面上，重置跳跃状态
        if (isGrounded && rb.velocity.y <= 0)
        {
            isJumping = false;
        }
    }

    void HandleMovement()
    {
        // 计算移动速度
        float targetVelocityX = moveInput * moveSpeed;
        
        // 空中控制调整
        if (!isGrounded)
        {
            targetVelocityX *= airControl;
        }
        
        // 应用水平移动
        rb.velocity = new Vector2(targetVelocityX, rb.velocity.y);
        
        // 处理角色朝向
        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && facingRight)
        {
            Flip();
        }
    }

    void HandleJumpInput()
    {
        // 跳跃输入检测
        if (jumpInput && isGrounded)
        {
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void HandleJumpPhysics()
    {
        // 可变高度跳跃（松开跳跃键时减少上升速度）
        if (isJumping && !Input.GetButton("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    void HandleAnimation()
    {
        if (animator != null)
        {
            // 设置移动动画
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
            
            // 设置跳跃/下落动画
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("VerticalVelocity", rb.velocity.y);
            
            // 设置跳跃动画
            if (jumpInput && isGrounded)
            {
                animator.SetTrigger("Jump");
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    void CreateGroundCheck()
    {
        GameObject groundCheckObj = new GameObject("GroundCheck");
        groundCheckObj.transform.SetParent(transform);
        groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
        groundCheck = groundCheckObj.transform;
    }

    // 可视化调试
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
    
    // 公共方法
    public bool IsGrounded()
    {
        return isGrounded;
    }
    
    public bool IsMoving()
    {
        return Mathf.Abs(moveInput) > 0.1f;
    }
}