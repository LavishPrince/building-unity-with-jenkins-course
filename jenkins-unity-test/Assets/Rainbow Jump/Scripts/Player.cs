using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RainbowJump.Scripts
{
    public class Player : MonoBehaviour
    {
        public float jumpForce = 10f;
        public Manager manager;

        public Rigidbody2D rb;

        // Update is called once per frame
        void Update()
        {
            if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) && !EventSystem.current.IsPointerOverGameObject())
            {
                rb.velocity = Vector2.up * jumpForce;
                manager.PlayTapSound();
            }


            if (transform.position.y < -5f)
            {
                manager.gameOver = true;
                manager.PlayDeathSound();
                transform.position = new Vector3(transform.position.x, -4.9f, transform.position.z);
            }


        }

        void OnTriggerEnter2D(Collider2D col)
        {
            manager.gameOver = true;
            manager.PlayDeathSound();
        }

        void OnBecameInvisible()
        {
            manager.gameOver = true;
            manager.PlayDeathSound();
        }
    }
}
