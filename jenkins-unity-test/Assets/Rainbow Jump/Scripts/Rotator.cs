using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RainbowJump.Scripts
{
    public class Rotator : MonoBehaviour
    {

        public float rotatingSpeed = 100f;
        // Start is called before the first frame update

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(0f, 0f, rotatingSpeed * Time.deltaTime);
        }
    }
}
