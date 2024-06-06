using UnityEngine;

public class Bullet : MonoBehaviour
{
    CircleCollider2D bulletCollider;
    SpriteRenderer bulletSpriteRender;

    [SerializeField] ParticleSystem rockHitPE;
    [SerializeField] ParticleSystem humanHitPE;

    void Start()
    {
        bulletCollider = GetComponent<CircleCollider2D>();
        bulletSpriteRender = GetComponent<SpriteRenderer>();

    }

    void Update()
    {

    }

    void HideBullet()
    {
        bulletCollider.enabled = false;
        bulletSpriteRender.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // khi đạn chạm vào đá 
        if (collision.gameObject.CompareTag("Rock"))
        {
            HideBullet();

            ParticleSystem spawnedHitFX = Instantiate(rockHitPE, transform.position, Quaternion.identity);

            Destroy(spawnedHitFX, spawnedHitFX.main.duration + spawnedHitFX.main.startLifetime.constantMax);
            Destroy(gameObject, spawnedHitFX.main.duration + spawnedHitFX.main.startLifetime.constantMax);


            Debug.Log("đạn bật vào đá");

        };

        // khi đạn chạm vào kẻ địch
        if (collision.gameObject.CompareTag("Enemy"))
        {
            HideBullet();

            ParticleSystem spawnedHitFX = Instantiate(humanHitPE, transform.position, Quaternion.identity);

            Destroy(spawnedHitFX, spawnedHitFX.main.duration + spawnedHitFX.main.startLifetime.constantMax);
            Destroy(gameObject, spawnedHitFX.main.duration + spawnedHitFX.main.startLifetime.constantMax);

            Debug.Log("Enemy đã ăn đạn");



        }

        
    }

}
