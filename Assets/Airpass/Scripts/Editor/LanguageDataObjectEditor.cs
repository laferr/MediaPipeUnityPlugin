using UnityEngine;
using UnityEditor;

namespace Airpass.Language
{
    [CustomEditor(typeof(LanguageDataObject))]
    public class LanguageDataObjectEditor : Editor
    {
        string searching;
        int index = 0;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            searching = EditorGUILayout.TextField(searching);
            if (GUI.Button(EditorGUILayout.GetControlRect(GUILayout.MinWidth(60)), "Search"))
            {
                EditorGUI.FocusTextInControl(searching);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("languageDatas"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}