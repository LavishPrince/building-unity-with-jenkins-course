using UnityEngine;
using System.Collections.Generic;

namespace RainbowJump.Scripts
{

    public class Spawner : MonoBehaviour
    {
        public List<GameObject> obstacles;
        public Transform spawnPoint;
        public float yOffset = 4.0f;
        public float spawnInterval = 0.9f;
        private float timer = 0.0f;

        private static List<GameObject> spawnedObstacles = new List<GameObject>();

        void Start()
        {
            InitializeObstacles();
        }

        public void InitializeObstacles()
        {
            // Initialize 2 obstacles at start
            for (int i = 0; i < 2; i++)
            {
                SpawnObstacle();
            }
        }

        void Update()
        {
            timer += Time.deltaTime;



            if (timer >= spawnInterval)
            {
                // Reset the timer and execute the code
                timer = 0.0f;
                SpawnObstacle();
            }


            // Spawn a new obstacle every spawnInterval seconds
            //InvokeRepeating("SpawnObstacle", 0.1f, spawnInterval);
        }

        void SpawnObstacle()
        {
            // Choose a random obstacle from the list
            GameObject obstaclePrefab = obstacles[Random.Range(0, obstacles.Count)];

            // Calculate the spawn position of the obstacle
            Vector3 spawnPosition = spawnPoint.position + Vector3.up * yOffset * spawnedObstacles.Count;

            // Instantiate the obstacle at the spawn position
            GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);

            // Add the obstacle to the list of spawned obstacles
            spawnedObstacles.Add(obstacle);

            // Set the parent of the obstacle to the "Obstacles" GameObject
            obstacle.transform.SetParent(transform);
        }

        public void DestroyAllObstacles()
        {
            foreach (GameObject obstacle in spawnedObstacles)
            {
                Destroy(obstacle);
            }

            spawnedObstacles.Clear();
        }

    }
}
