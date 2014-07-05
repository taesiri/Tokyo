using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    public class AutoLevelBuilder : EditorWindow
    {
        private bool myBool = true;
        private float myFloat = 1.23f;
        private string myString = "Hello World";
        private bool groupEnabled;

        [MenuItem("DeadMage/Create New Level", false, 100)]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(AutoLevelBuilder));
            window.title = "New Arena";
        }


        private void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            myString = EditorGUILayout.TextField("Text Field", myString);

            groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
            myBool = EditorGUILayout.Toggle("Toggle", myBool);
            myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
            EditorGUILayout.EndToggleGroup();
        }
    }
}