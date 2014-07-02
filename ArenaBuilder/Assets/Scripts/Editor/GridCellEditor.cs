using Assets.Scripts.Arena;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    [CustomEditor(typeof (GridCell))]
    public class GridCellEditor : UnityEditor.Editor
    {
        public void OnSceneGUI()
        {
            var currentObject = (GridCell) target;
            Handles.color = currentObject.IsEmpty ? Color.green : Color.red;
            Handles.CubeCap(0,
                currentObject.transform.position,
                currentObject.transform.rotation,
                0.9f);
        }
    }
}