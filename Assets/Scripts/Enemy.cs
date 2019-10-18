using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour { 

    [SerializeField] private int heath;
    [SerializeField] private int points;
    [SerializeField] private float speed;
    [SerializeField] private float rayDistance;
    [SerializeField] private Transform groundDetection;
    [SerializeField] private GameManager gameManager;

    private bool movingRight;

    void Start() {
        movingRight = true;
    }

    void Update() {
        if(heath <= 0) {
            gameManager.UpdateScore(points);
            Destroy(gameObject);
        }

        transform.Translate(Vector2.right * speed * Time.deltaTime);

        RaycastHit2D groundInfo = Raycast(groundDetection.position, Vector2.down, rayDistance);
        if(!groundInfo.collider) {
            if(movingRight) {
                transform.eulerAngles = new Vector3(0, -180, 0);
                movingRight = false;
            } else {
                transform.eulerAngles = new Vector3(0, 0, 0);
                movingRight = true;
            }
        }
    }

    public void TakeDamage(int damage) {
        if(heath > 0) {
            heath -= damage;
        }
    }

    private RaycastHit2D Raycast(Vector2 start, Vector2 dir, float len) {
        Debug.DrawLine(start, start + dir * len, Color.red);
        return Physics2D.Raycast(start, dir, len);
    }
}
