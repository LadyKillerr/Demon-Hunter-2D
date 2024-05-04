using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D playerRigidbody;

    public Vector3 moveInput;
    float playerScaleX;


    [SerializeField] float moveSpeed = 5f;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        playerScaleX = transform.localScale.x;
    }

    void Update()
    {
        CharacterMovement();
        FlipSprite();
    }

    public void CharacterMovement()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        transform.position += moveSpeed * moveInput * Time.deltaTime;
    }

    public void FlipSprite()
    {

        if (moveInput.x > Mathf.Epsilon)
        {
            transform.localScale = new Vector2(playerScaleX, transform.localScale.y);
        }
        else if (moveInput.x == 0)
        {
            transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
        }
        else
        {
            transform.localScale = new Vector2(-1 * playerScaleX, transform.localScale.y);
        }

    }
}
