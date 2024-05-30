using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Rigidbody2D playerRigidbody;

    Vector2 moveInput;
    bool IsMovingHorizontal;
    bool IsMovingVertical;

    Animator playerAnimator;

    [SerializeField] float moveSpeed = 600f;
    [SerializeField] float rollAnimTime = 0.4f;
    [SerializeField] float rollSpeedBoost = 400f;
    [SerializeField] Vector2 rollDistance = new Vector2(20, 0);

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
    }

    void Start()
    {

    }

    // Fixed Update r for playerMovement
    void FixedUpdate()
    {

        CharacterMovement();
        FlipSprite();

    }

    #region playerMovement

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();


    }

    public void CharacterMovement()
    {
        Vector2 playerInput = new(moveInput.x * moveSpeed * Time.fixedDeltaTime,
                                          moveInput.y * moveSpeed * Time.fixedDeltaTime);

        playerRigidbody.velocity = playerInput;


        // Walking Animations
        //if (IsMovingHorizontal || IsMovingVertical)
        //{
        //    playerAnimator.SetTrigger("isWalking");

        //}
        //else if (!IsMovingHorizontal && !IsMovingVertical)
        //{
        //    playerAnimator.SetTrigger("isIdling");
        //}

        // một cách hay hơn để nhận biết khi nào nên bật Walking anim
        playerAnimator.SetFloat("Speed", moveInput.sqrMagnitude);

    }

    void OnRoll(InputValue value)
    {
        IsMovingHorizontal = Mathf.Abs(playerRigidbody.velocity.x) > Mathf.Epsilon;
        IsMovingVertical = Mathf.Abs(playerRigidbody.velocity.y) > Mathf.Epsilon;

        if ((value.isPressed && IsMovingHorizontal) || (value.isPressed && IsMovingVertical))
        {
            playerAnimator.SetTrigger("isRolling");

            moveSpeed += rollSpeedBoost;
            StartCoroutine(ResetMoveSpeed(rollAnimTime));

            playerRigidbody.AddForce(rollDistance * Mathf.Sign(transform.localScale.x), ForceMode2D.Impulse);
        }
    }

    IEnumerator ResetMoveSpeed(float value)
    {
        yield return new WaitForSeconds(value);

        moveSpeed -= rollSpeedBoost;
    }

    void FlipSprite()
    {
        //if (IsMovingHorizontal)
        //{
        //    transform.localScale = new Vector2(Mathf.Sign(playerRigidbody.velocity.x), transform.localScale.y);
        //    Debug.Log("velocity is:" + playerRigidbody.velocity);
        //}

        IsMovingHorizontal = Mathf.Abs(playerRigidbody.velocity.x) > Mathf.Epsilon;

        if (IsMovingHorizontal)
        {
            transform.localScale = new Vector2(Mathf.Sign(playerRigidbody.velocity.x), 1f);
        }


        //if (moveInput.x != 0)
        //{
        //    if (moveInput.x > Mathf.Epsilon)
        //    {
        //        transform.localScale = new Vector2(1, 1);

        //    }
        //    else if (moveInput.x < -Mathf.Epsilon)
        //    {
        //        transform.localScale = new Vector2(-1, 1);
        //    }
        //}

    }

    #endregion
}
