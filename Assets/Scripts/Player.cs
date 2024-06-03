using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Rigidbody2D playerRigidbody;
    SpriteRenderer playerSpriteRender;

    Vector2 moveInput;
    bool IsMovingHorizontal;
    bool IsMovingVertical;

    Animator playerAnimator;

    [SerializeField] float moveSpeed = 600f;

    [Header("Roll Status")]
    [SerializeField] float rollAnimTime = 0.4f;
    [SerializeField] float rollSpeedBoost = 400f;
    float rollCountdown;
    [SerializeField] float delayBetweenRolls = 1;
    [SerializeField] Vector2 rollDistance = new Vector2(20, 0);

    [Header("Player Weapon")]
    [SerializeField] GameObject playerGun;
    [SerializeField] GameObject playerBullet;
    [SerializeField] Transform gunTip;
    float fireCountdown;
    [SerializeField] float fireDelay;
    [SerializeField] float bulletForce;
    [SerializeField] float destroyBulletDelay = 5f;
    [SerializeField] GameObject gunFX;
    [SerializeField] float destroyMuzzleDelay;

    private void Awake()
    {
        playerSpriteRender = GetComponent<SpriteRenderer>();
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
        RotateGun();

    }

    private void Update()
    {
        

        

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
        // xử lý cooldown của Roll
        rollCountdown -= Time.deltaTime;

        IsMovingHorizontal = Mathf.Abs(playerRigidbody.velocity.x) > Mathf.Epsilon;
        IsMovingVertical = Mathf.Abs(playerRigidbody.velocity.y) > Mathf.Epsilon;

        if ((value.isPressed && IsMovingHorizontal && rollCountdown <= 0) || (value.isPressed && IsMovingVertical && rollCountdown <= 0))
        {
            playerAnimator.SetTrigger("isRolling");

            moveSpeed += rollSpeedBoost;

            rollCountdown = delayBetweenRolls;

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

        //IsMovingHorizontal = Mathf.Abs(playerRigidbody.velocity.x) > Mathf.Epsilon;

        //if (IsMovingHorizontal)
        //{
        //    transform.localScale = new Vector2(Mathf.Sign(playerRigidbody.velocity.x), 1f);
        //}


        if (moveInput.x != 0)
        {
            if (moveInput.x > Mathf.Epsilon)
            {
                playerSpriteRender.flipX = false;

            }
            else if (moveInput.x < -Mathf.Epsilon)
            {
                playerSpriteRender.flipX = true;

            }
        }

    }

    #endregion

    #region playerAttack

    void RotateGun()
    {
        // lấy ra toạ độ hiện tại của con chuột trên màn hình và chuyển nó thành vị trí Vector2 (transform)
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Get ra hướng vector từ con chuột tới vũ khí của người chơi 
        Vector2 lookDirection = mousePosition - transform.position;

        // Tính góc alpha của playerGun thông qua 
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        playerGun.transform.rotation = rotation;

        if (playerGun.transform.eulerAngles.z > 90 && playerGun.transform.eulerAngles.z < 270)
        {
            playerGun.transform.localScale 
                = new Vector2(7,-7); 
        }
        else 
        {
            playerGun.transform.localScale 
                = new Vector2(7, 7); 
            

        }
    }

    void OnFire(InputValue value)
    {
        // xử lý cooldown của Fire
        fireCountdown -= Time.deltaTime;

        if (value.isPressed && fireCountdown <= 0)
        {
            GameObject spawnedBullet = Instantiate(playerBullet, gunTip.position, Quaternion.identity);

            GameObject spawnedMuzzle = Instantiate(gunFX, gunTip.position, gunTip.transform.rotation);

            Rigidbody2D bulletRigidbody = spawnedBullet.GetComponent<Rigidbody2D>();

            bulletRigidbody.AddForce(playerGun.transform.right * bulletForce, ForceMode2D.Impulse);

            StartCoroutine(DestroySpawnedBullet(spawnedBullet, destroyBulletDelay, spawnedMuzzle, destroyMuzzleDelay));


        }


    }

    IEnumerator DestroySpawnedBullet(GameObject objectToDestroy, float destroyBulletDelay, 
                                     GameObject spawnedMuzzle, float destroyMuzzleDelay)
    {
        yield return new WaitForSeconds(destroyMuzzleDelay);
        Destroy(spawnedMuzzle);
        

        yield return new WaitForSeconds(destroyBulletDelay - destroyMuzzleDelay);
        Destroy(objectToDestroy);
    }

    #endregion
}
