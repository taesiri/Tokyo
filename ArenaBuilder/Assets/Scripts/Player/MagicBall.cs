using UnityEngine;

namespace Assets.Scripts.Player
{
    public class MagicBall : MonoBehaviour
    {
        public float JumpHeight = 10.0f;
        public float Speed = 10.0f;


        public void Update()
        {
            float horizontalMovement = Input.GetAxis("Horizontal");

            rigidbody.AddForce(Vector3.right*horizontalMovement*100*Speed*Time.deltaTime, ForceMode.Force);


            if (Input.GetButtonDown("Jump"))
            {
                rigidbody.AddForce(Vector3.up*JumpHeight, ForceMode.Impulse);
            }
        }
    }
}