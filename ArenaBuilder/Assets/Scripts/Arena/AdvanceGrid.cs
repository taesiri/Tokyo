using UnityEngine;

namespace Assets.Scripts.Arena
{
    public class AdvanceGrid : MonoBehaviour
    {
        [HideInInspector] public int Columns;
        [HideInInspector] public bool DrawDebugLines = false;
        public Transform PlaneTransform;
        [HideInInspector] public int Rows;


        //public Vector3 GetLowerLeftOf(Transform objTransform)
        //{
        //    return transform.position - new Vector3(objTransform.renderer.bounds.size.x/2f, objTransform.renderer.bounds.size.y/2f, objTransform.renderer.bounds.size.z/2f);
        //}
    }
}