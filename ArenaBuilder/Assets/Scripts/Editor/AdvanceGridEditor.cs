using Assets.Scripts.Arena;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    [CustomEditor(typeof (AdvanceGrid))]
    public class AdvanceGridEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var grid = (AdvanceGrid) target;
            //base.OnInspectorGUI();
            if (!grid.PlaneTransform)
            {
                if (GUILayout.Button("Create Grid Plane"))
                {
                    CreateGridPlane(grid);
                }
            }
            else
            {
                grid.DrawDebugLines = EditorGUILayout.ToggleLeft("Draw Debug Lines - Under revision, Use with caution!", grid.DrawDebugLines);

                grid.Columns = EditorGUILayout.IntField("Columns", grid.Columns);
                grid.Rows = EditorGUILayout.IntField("Rows", grid.Rows);


                if (GUILayout.Button("Update Grid Size"))
                {
                    UpdateGridSize(grid);
                }
            }
        }

        private void UpdateGridSize(AdvanceGrid grid)
        {
            if (grid.PlaneTransform)
            {
                grid.PlaneTransform.localScale = new Vector3(grid.Columns/10f, 1, grid.Rows/10f);
            }
        }

        private void CreateGridPlane(AdvanceGrid grid)
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "Grid Plane";
            plane.transform.Rotate(Vector3.right, 270);
            plane.transform.parent = grid.transform;
            grid.PlaneTransform = plane.transform;

            grid.Rows = 8;
            grid.Columns = 8;

            UpdateGridSize(grid);
        }

        public void OnSceneGUI()
        {
            var grid = (AdvanceGrid) target;

            if (grid.DrawDebugLines)
            {
                Vector3 center = grid.transform.position;
                Vector3 lel = center - new Vector3(grid.Columns/2f, grid.Rows/2f, 0);
                Handles.color = Color.red;

                for (int i = 0; i <= grid.Rows; i++)
                {
                    Handles.DrawLine(lel + new Vector3(0, i, 0), lel + new Vector3(grid.Columns, i, 0));
                }

                for (int i = 0; i <= grid.Columns; i++)
                {
                    Handles.DrawLine(lel + new Vector3(i, 0, 0), lel + new Vector3(i, grid.Rows, 0));
                }

                SceneView.RepaintAll();
            }
        }
    }
}