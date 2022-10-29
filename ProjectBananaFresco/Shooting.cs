/*****************************************************************************
// File Name :         Shooting.cs
// Author :            Lucas Johnson
// Creation Date :     January 24, 2022
//
// Brief Description : A C# script that handles the firing of paintballs from
                       the paintball gun.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    private Camera mainCam;

    public GameObject paintBall;
    public Transform firePoint;

    public float shootForce;

    public float timeBetweenShots = 0.5f;
    private float nextShotTime;
    public float destroyDelay = 1;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Rotation();
        Shoot();
    }


    void Rotation()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = mainCam.ScreenToWorldPoint(mousePos);

        Vector2 delta = transform.position - worldPos;

        // Get the angle between the two objects
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle + 90);
    }

    void Shoot()
    {
        bool canShoot = Time.time > nextShotTime;

        if (Input.GetButton("Fire1") && (canShoot))
        {
            GameObject projectile = Instantiate(paintBall, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb.AddForce(firePoint.up * shootForce, ForceMode2D.Impulse);

            Destroy(projectile, destroyDelay);

            nextShotTime = Time.time + timeBetweenShots;
        }
    }
}
