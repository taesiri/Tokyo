using Assets.Scripts.Arena;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    public class AutoLevelBuilder : EditorWindow
    {
        private float _camDistance;
        private bool _isometricSetup;
        private string _levelName = "new-level";

        [MenuItem("DeadMage/Create New Level", false, 100)]
        public static void ShowWindow()
        {
            EditorWindow window = GetWindow(typeof (AutoLevelBuilder));
            window.title = "New Arena";
        }

        private void OnGUI()
        {
            GUILayout.Label("General Settings", EditorStyles.boldLabel);
            _levelName = EditorGUILayout.TextField("Level Name", _levelName);


            _isometricSetup = EditorGUILayout.BeginToggleGroup("Isometric Setup", _isometricSetup);
            _camDistance = EditorGUILayout.FloatField("Camera Distance", _camDistance);
            EditorGUILayout.EndToggleGroup();

            if (GUILayout.Button("Create !"))
            {
                CreateLevel(_isometricSetup);
            }
        }


        private void CreateLevel(bool isIsometric)
        {
            var sceneRoot = new GameObject {name = "SceneObjects"};


            //Background
            GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Plane);
            bg.transform.localScale = new Vector3(10, 1, 10);
            bg.transform.position = new Vector3(0, 0, 10);
            bg.transform.Rotate(Vector3.right, 270);
            bg.name = "Background";
            bg.GetComponent<Renderer>().material = Resources.Load("Materials/DefaultBackgroundMaterial", typeof (Material)) as Material;


            //Camera
            Camera.main.orthographic = true;
            Camera.main.farClipPlane = 100;
            Camera.main.backgroundColor = Color.black;

            if (!isIsometric)
            {
                Camera.main.orthographicSize = 8.6f;
                Camera.main.transform.parent = sceneRoot.transform;
            }
            else
            {
                var camContainer = new GameObject("CameraContainer");

                Camera.main.orthographic = true;
                Camera.main.orthographicSize = 5.6f;
                Camera.main.transform.Rotate(45, 35, 0);
                Camera.main.transform.parent = camContainer.transform;
                Camera.main.transform.position = new Vector3(6, 15, -9);

                camContainer.transform.position = new Vector3(-16, 0.4f, 2.1f);
                camContainer.transform.Rotate(Vector3.right, 270);
            }

            // Light
            var dLight = new GameObject();
            var lightComponent = dLight.AddComponent<Light>();
            lightComponent.type = LightType.Directional;
            lightComponent.intensity = 0.5f;
            lightComponent.cookieSize = 10f;
            dLight.transform.position = new Vector3(0, 0, -20);
            dLight.name = "Scene Light";
            dLight.transform.parent = sceneRoot.transform;


            var gridObject = new GameObject("Grid");
            var grid = gridObject.AddComponent<AdvanceGrid>();

            var brainObject = new GameObject("Brain");
            var brain = brainObject.AddComponent<AdvanceBrain>();

            brain.GameGrid = grid;
        }
    }
}