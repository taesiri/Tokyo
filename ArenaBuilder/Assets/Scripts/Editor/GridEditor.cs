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
        private float _scaleFactor = 1.01f;
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
            _scaleFactor = EditorGUILayout.FloatField("Scale Factor", _scaleFactor);


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

                        if (currentObject.GridTileObject)
                        {

                            var pos = new Vector3(i*_scaleFactor + _offX, j*_scaleFactor + _offY, _zIndex);
                            GameObject tileElement = (GameObject) Instantiate(currentObject.GridTileObject, pos, Quaternion.identity);
                            tileElement.name = String.Format("{0}-{1}", i, j);
                            tileElement.transform.parent = currentObject.transform;
                            tileElement.layer = 8;
                        }
                       
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