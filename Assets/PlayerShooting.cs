using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireRate = 0.2f;
    private float nextFireTime = 0f;

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                Shoot(Vector3.up);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                Shoot(Vector3.down);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                Shoot(Vector3.left);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                Shoot(Vector3.right);
            }
        }
    }

    void Shoot(Vector3 direction)
    {
        nextFireTime = Time.time + fireRate;

        // Creamos un pequeño desfase (offset) para que la bala nazca fuera del colisionador del jugador
        Vector3 spawnPosition = transform.position + (direction * 0.6f);

        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null)
        {
            float bulletSpeed = 10f;
            rbBullet.linearVelocity = direction * bulletSpeed;
        }
    }
}