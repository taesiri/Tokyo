using System;
using Assets.Scripts.Arena;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    [CustomEditor(typeof (Grid))]
    public class GridEditor : UnityEditor.Editor
    {
        private int _offX = -4, _offY = -4;
        private int _sizeX = 8, _sizeY = 8;
        private int _zIndex = 5;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Label("Grid Helper");
            
            _sizeX = EditorGUILayout.IntField("Size - X", _sizeX);
            _sizeY = EditorGUILayout.IntField("Size - Y", _sizeY);

            _offX = EditorGUILayout.IntField("Offset - X", _offX);
            _offY = EditorGUILayout.IntField("Offset - Y", _offY);

            _zIndex = EditorGUILayout.IntField("Z Index", _zIndex);


            if (GUILayout.Button("Generate Grid"))
            {
                GenerateGrid();
            }
            if (GUILayout.Button("Clear Childs"))
            {
                ClearGrid();
            }
            if (GUI.changed)
            {
            }
        }


        private void GenerateGrid()
        {
            var currentObject = (Grid) target;
            if (_sizeX != 0 && _sizeY != 0)
            {
                for (int i = 0; i < _sizeX; i++)
                {
                    for (int j = 0; j < _sizeY; j++)
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.name = String.Format("{0}-{1}", i, j);
                        cube.transform.position = new Vector3(i*2 + _offX, j*2 + _offY, _zIndex);
                        cube.AddComponent<GridCellObject>();
                        cube.transform.parent = currentObject.transform;
                    }
                }
            }
        }

        private void ClearGrid()
        {
            var currentObject = (Grid) target;
            var killList = new Transform[currentObject.transform.childCount];
            for (int i = 0; i < killList.Length; i++)
            {
                killList[i] = currentObject.transform.GetChild(i);
            }

            for (int i = 0; i < killList.Length; i++)
            {
                DestroyImmediate(killList[i].gameObject);
            }
        }
    }
}