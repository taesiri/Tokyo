using UnityEngine;

namespace Assets.Scripts.Arena
{
    public static class CameraMovementHandler
    {
        #region CameraFields

        public static float MaximumOrthographicSize = 40.0f;
        public static float MinPinchSpeed = 2.0F;
        public static float MovementSpeed = .025f;
        public static float VarianceInDistances = 5.0F;
        public static float ZoomSpeedMouse = 5.0f;
        public static float ZoomSpeedTocuh = 0.5f;

        #endregion

        public static void Handler()
        {
            if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved &&
                Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                Vector2 curDist = Input.GetTouch(0).position - Input.GetTouch(1).position;
                //current distance between finger touches
                Vector2 prevDist = ((Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition) -
                                    (Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition));

                float touchDelta = curDist.magnitude - prevDist.magnitude;

                float speedTouch0 = Input.GetTouch(0).deltaPosition.magnitude/Input.GetTouch(0).deltaTime;
                float speedTouch1 = Input.GetTouch(1).deltaPosition.magnitude/Input.GetTouch(1).deltaTime;


                if ((speedTouch0 > MinPinchSpeed) && (speedTouch1 > MinPinchSpeed))
                {
                    if (touchDelta + VarianceInDistances <= 1)
                    {
                        Camera.main.orthographicSize =
                            Mathf.Clamp(Camera.main.orthographicSize + (1*ZoomSpeedTocuh),
                                MaximumOrthographicSize/10,
                                MaximumOrthographicSize);
                    }
                    else if (touchDelta + VarianceInDistances > 1)
                    {
                        Camera.main.orthographicSize =
                            Mathf.Clamp(Camera.main.orthographicSize - (1*ZoomSpeedTocuh),
                                MaximumOrthographicSize/10, MaximumOrthographicSize);
                    }
                    MoveCamera(Vector3.zero);
                }
            }
            else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                MoveCamera(Input.GetTouch(0).deltaPosition);
            }
        }


        public static void MoveCamera(Vector3 movement)
        {
            //float boundaryX = (GameGrid.Columns * 6) - (Camera.main.aspect * Camera.main.orthographicSize);
            //float boundaryY = (GameGrid.Rows * 6) - Camera.main.orthographicSize;

            movement.z = 0;
            movement *= -MovementSpeed;
            movement *= Camera.main.orthographicSize/10;

            Vector3 pos = Camera.main.transform.position;
            pos += movement;

            //pos.x = Mathf.Clamp(pos.x, -boundaryX, boundaryX);
            //pos.y = Mathf.Clamp(pos.y, -boundaryY, boundaryY);

            Camera.main.transform.position = pos;
        }
    }
}