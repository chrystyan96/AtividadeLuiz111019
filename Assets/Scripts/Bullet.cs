using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private int damage;
    [SerializeField] private GameObject impactEffect;

    private Rigidbody2D rigidbody2D;
    // Start is called before the first frame update
    void Start()
    {
        bulletSpeed = 15f;
        rigidbody2D = GetComponent<Rigidbody2D>();

        rigidbody2D.velocity = transform.right * bulletSpeed;

        StartCoroutine(DestroyAfterTime(10f));
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Damage damageScript = collider.GetComponent<Damage>();
        if(damageScript != null) {
            damageScript.changeHeath(damage * (-1));
        }
        Destroy(gameObject);
    }

    private IEnumerator DestroyAfterTime(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            Destroy(gameObject);
        }
    }
}
