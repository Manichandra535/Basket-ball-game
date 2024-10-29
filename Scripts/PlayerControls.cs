using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
    public class PlayerControls : MonoBehaviour
    {
        public float moveSpeed = 5;
        public GameObject basketball;
        public Transform holdPos;
        public Transform overheadPos;
        private bool isBallInHands = true;
        private bool isBallFlying = false;
        public Transform targetPos;
        private float T = 0;
    
        // // Start is called before the first frame update
        void Start()
        {
            basketball.transform.position = holdPos.position;
        }

        void Update()
        {
            //Creating a direction vector based on the input axes (X being Horizontal direction, Y being 0 as we do not want the player to move beyond the plane and Z being Vertical direction).
            Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            //Change the position transform by the direction vector times the preset movement speed and the time.
            transform.position += direction * moveSpeed * Time.deltaTime;
            //Makes the character look towards the vector's direction.
            transform.LookAt(transform.position + direction);

            if (isBallInHands)
            {
                //If it is, we will define the space key as the key to hold the ball over the character's head.
                if (Input.GetKey(KeyCode.Space))
                {
                    basketball.transform.position = overheadPos.position;
                    transform.LookAt(targetPos.parent.position);
                }
                else
                {
                    basketball.transform.position = holdPos.position;
                }

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    //For the ball to be shot, it must leave the character hands, so we make the boolean variable false.
                    isBallInHands = false;
                    //To begin shooting, we change the boolean flying to true (as the ball starts flying from the hands)
                    isBallFlying = true;
                    //And restart the time (so that the new series of events can be easily written)
                    T = 0;
                }
            }

            if (isBallFlying)
            {
                //Time variable is assigned a new value based on the time the event starts
                T += Time.deltaTime;
                //duration of travel
                float duration = 0.5f;
                //This variable allows for the interpolation to be accurate, as we want a set duration for the shooting.
                float t01 = T / duration;  
            
                Vector3 point_A = overheadPos.position;
                Vector3 point_B = targetPos.position;
                //Lerp is the linear interpolation function, will result in a new vector that will represent the trajectory from A to B.
                Vector3 pos = Vector3.Lerp(point_A, point_B, t01);
                //Arc created using sine function
                Vector3 arc = Vector3.up * 5 * Mathf.Sin(t01 * 3.14f);
                //Moves the ball in an arc function 
                basketball.transform.position = pos + arc;

                if (t01 >= 1)
                {
                    isBallFlying = false;
                    basketball.GetComponent<Rigidbody>().isKinematic = false;
                }
            }
        }
            //when the collision trigger happens, this method is executed.
        private void OnTriggerEnter(Collider other)
        {
                //Conditional: If the ball isn't flying or has not been picked up yet, pick up the ball and activate its Rigidbody component's kinematics.
            if (!isBallInHands && !isBallFlying)
            {
                isBallInHands = true;
                basketball.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }
