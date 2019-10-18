using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public Transform firePoint;
    private bool facingRight;

    [SerializeField] private Transform firePointLeft;
    [SerializeField] private Transform firePointRight;
    [SerializeField] private GameObject prefabBullet;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private LineRenderer lineRenderer;

    void Start() {
        facingRight = true;
    }

    public void setFacing(bool facing) {
        facingRight = facing;
    }

    // Update is called once per frame
    void Update() {
        if(facingRight) {
            firePointRight.gameObject.SetActive(true);
            firePointLeft.gameObject.SetActive(false);
            firePoint = firePointRight;
        } else {
            firePointRight.gameObject.SetActive(false);
            firePointLeft.gameObject.SetActive(true);
            firePoint = firePointLeft;
        }

        if(Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot() {
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right);

        if(hit) {
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if(enemy != null) {
                enemy.TakeDamage(1);
            }

            // Instantiate(impactEffect, hit.point, Quaternion.identity);

            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, hit.point);
        } else {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, firePoint.position + firePoint.right * 100);
        }

        lineRenderer.enabled = true;

        yield return new WaitForSeconds(0.02f);

        lineRenderer.enabled = false;
    }

    public Transform getFirePoint() {
        return firePoint;
    }
}
