using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] float bulletDamage = 10f;
    [SerializeField] float meleeDamage = 20f;
    

    void Start()
    {

    }

    void Update()
    {

    }

    public float GetBulletDamage()
    {
        return bulletDamage;
    }

    public float GetMeleeDamage()
    {
        return meleeDamage;
    }

    public void Hit()
    {
        Destroy(gameObject);
    }
}
