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
    [SerializeField] float rollSpeedBoostTime = 0.4f;
    [SerializeField] float rollSpeedBoost = 400f;
    float rollCountdown;
    [SerializeField] float delayBetweenRolls = 1;
    [SerializeField] Vector2 rollDistance = new Vector2(20, 0);

    [Header("Player Weapon")]
    [SerializeField] GameObject playerGun;
    [SerializeField] GameObject playerBullet;
    [SerializeField] Transform gunTipPos;
    [SerializeField] GameObject gunMuzzle;

    public bool isFiring;
    Coroutine firingCoroutine;
    [SerializeField] float delayBetweenFire = 0.3f;
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
        

    }

    private void Update()
    {
        if (playerRigidbody.IsTouchingLayers(LayerMask.GetMask("Obstacles")))
        {
            Debug.Log("Is Touching Obstacles");
        }

        // xử lý cooldown của Roll
        rollCountdown -= Time.deltaTime;


        FlipSprite();

        RotateGun();
        Fire();

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

        if ((value.isPressed && IsMovingHorizontal && rollCountdown <= 0) || (value.isPressed && IsMovingVertical && rollCountdown <= 0))
        {
            playerAnimator.SetTrigger("isRolling");

            moveSpeed += rollSpeedBoost;

            rollCountdown = delayBetweenRolls;

            StartCoroutine(ResetMoveSpeed(rollSpeedBoostTime));

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
                = new Vector2(1, -1);
        }
        else
        {
            playerGun.transform.localScale
                = new Vector2(1, 1);


        }
    }

    void Fire()
    {
        // nếu đang giữ chuột thì isFiring = true
        isFiring = Input.GetMouseButton(0) ? true : false;


        if (isFiring == true && firingCoroutine == null)
        {
            firingCoroutine = StartCoroutine(FireContinuously());

        }
        else if (!isFiring && firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);

            firingCoroutine = null;
        }
    }

    IEnumerator FireContinuously()
    {
        // làm hàm fire loop liên tục miễn là ng chơi còn giữ chuột
        while (true)
        {
            GameObject spawnedBullet = Instantiate(playerBullet, gunTipPos.position, Quaternion.identity);

            gunMuzzle.SetActive(true);

            Rigidbody2D bulletRigidbody = spawnedBullet.GetComponent<Rigidbody2D>();

            bulletRigidbody.AddForce(playerGun.transform.right * bulletForce, ForceMode2D.Impulse);

            StartCoroutine(DestroySpawnedBullet(spawnedBullet, destroyBulletDelay, destroyMuzzleDelay));

            yield return new WaitForSeconds(delayBetweenFire);
        }

    }

    IEnumerator DestroySpawnedBullet(GameObject objectToDestroy, float destroyBulletDelay,
                                      float destroyMuzzleDelay)
    {
        yield return new WaitForSeconds(destroyMuzzleDelay);
        gunMuzzle.SetActive(false);


        yield return new WaitForSeconds(destroyBulletDelay - destroyMuzzleDelay);
        Destroy(objectToDestroy);
    }

    #endregion
}
