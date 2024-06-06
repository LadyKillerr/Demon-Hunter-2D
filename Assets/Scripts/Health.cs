using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] bool isPlayer;
    [SerializeField] float health = 50f;
    Rigidbody2D rb2d;

    [SerializeField] ParticleSystem humanHitFX;
    [SerializeField] GameObject bleedPos;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {

    }

    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // khi ăn đạn và kp player
        if (collision.gameObject.CompareTag("PlayerBullet") && !isPlayer)

        {
            DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();
            health -= damageDealer.GetBulletDamage();



        };

        // khi va chạm với enemy
        if (collision.gameObject.CompareTag("Enemy") && isPlayer)
        {
            DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();
            health -= damageDealer.GetMeleeDamage();

            ParticleSystem spawnedFX = Instantiate(humanHitFX, bleedPos.transform.position, Quaternion.identity);

            Destroy(spawnedFX, spawnedFX.main.duration + spawnedFX.main.startLifetime.constantMax);

            Debug.Log("Người chơi đã ăn đạn");

        }

        
    
    }

    
    }


