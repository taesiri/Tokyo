using Assets.Scripts.Arena;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class MagicBall : MonoBehaviour
    {
        private readonly GUILocationHelper _locationHelper = new GUILocationHelper();
        public float DampingX = 2.0f;
        public float DampingY = 2.0f;
        public float JumpHeight = 10.0f;
        public Transform PlayerCamera;
        public float Speed = 10.0f;
        private float _currentX;
        private float _currentY;
        private Transform _localTransform;
        private float _wantedX;
        private float _wantedY;

        public void Start()
        {
            _localTransform = transform;
            _locationHelper.PointLocation = GUILocationHelper.Point.BottomRight;
            _locationHelper.UpdateLocation();
        }

        public void Update()
        {
            float horizontalMovement = Input.GetAxis("Horizontal");

            GetComponent<Rigidbody>().AddForce(Vector3.right*horizontalMovement*100*Speed*Time.deltaTime, ForceMode.Force);
            if (Input.GetButtonDown("Jump"))
            {
                GetComponent<Rigidbody>().AddForce(Vector3.up*JumpHeight, ForceMode.Impulse);
            }
        }

        public void LateUpdate()
        {
            _wantedY = _localTransform.position.y;
            _wantedX = _localTransform.position.x;

            _currentY = PlayerCamera.position.y;
            _currentX = PlayerCamera.position.x;

            _currentY = Mathf.Lerp(_currentY, _wantedY, DampingY*Time.deltaTime);
            _currentX = Mathf.Lerp(_currentX, _wantedX, DampingX*Time.deltaTime);

            Vector3 pos = PlayerCamera.position;
            pos.y = _currentY;
            pos.x = _currentX;

            PlayerCamera.position = pos;
        }

        public void OnGUI()
        {
            GUI.matrix = AdvanceBrain.GUIMatrix;

            if (GUI.RepeatButton(new Rect(_locationHelper.Offset.x - 200f, _locationHelper.Offset.y - 200f, 150, 150), ">"))
            {
                GetComponent<Rigidbody>().AddForce(Vector3.right*20*Speed*Time.deltaTime, ForceMode.Force);
            }

            if (GUI.RepeatButton(new Rect(_locationHelper.Offset.x - 400f, _locationHelper.Offset.y - 200f, 150, 150), "<"))
            {
                GetComponent<Rigidbody>().AddForce(Vector3.left*20*Speed*Time.deltaTime, ForceMode.Force);
            }

            if (GUI.Button(new Rect(10, _locationHelper.Offset.y - 200f, 150, 150), "Jump"))
            {
                GetComponent<Rigidbody>().AddForce(Vector3.up * JumpHeight, ForceMode.Impulse);
            }
            GUI.matrix = Matrix4x4.identity;
        }
    }
}