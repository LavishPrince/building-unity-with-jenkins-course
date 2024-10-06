using UnityEngine;

namespace RainbowJump.Scripts
{
    public class DestroyObstacle : MonoBehaviour
    {
        private Camera mainCamera;

        void Start()
        {
            // Get a reference to the MainCamera
            mainCamera = Camera.main;
        }

        void Update()
        {
            // Check if the gameobject's y position is less than the MainCamera's position -7
            if (transform.position.y < mainCamera.transform.position.y - 7.0f && gameObject.CompareTag("Obstacle"))
            {
                // Destroy the gameobject
                Destroy(gameObject);
            }
        }
    }
}