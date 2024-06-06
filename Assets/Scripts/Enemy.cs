using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D enemyRigidbody;
    SpriteRenderer enemySpriteRenderer;
    Animator enemyAnimator;

    private void Awake()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemySpriteRenderer = GetComponent<SpriteRenderer>();
        enemyAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        CheckMovement();    
    }

    void Update()
    {
        FlipSprite();
    }

    void CheckMovement()
    {
        if (Mathf.Abs(enemyRigidbody.velocity.x) > Mathf.Epsilon)
        {
            enemyAnimator.SetBool("isMoving", true);
            enemyAnimator.SetBool("isIdling", false);
        }
        else
        {
            enemyAnimator.SetBool("isMoving", false);
            enemyAnimator.SetBool("isIdling", true);
        }
        //enemyAnimator.SetBool("isMoving", Mathf.Abs(enemyRigidbody.velocity.x) > Mathf.Epsilon);


    }

    void FlipSprite()
    {
        if (Mathf.Abs(enemyRigidbody.velocity.x) > Mathf.Epsilon)
        {
            if (enemyRigidbody.velocity.x > Mathf.Epsilon)
            {
                enemySpriteRenderer.flipX = false;
            }
            else
            {
                enemySpriteRenderer.flipX = true;
            }
        }
    }
}
