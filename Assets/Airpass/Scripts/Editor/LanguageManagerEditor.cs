using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Data;
using System;
using System.IO;
using System.Text;
using System.Linq;

namespace Airpass.Language
{
    [CustomEditor(typeof(LanguageManager))]
    public class LanguageManagerEditor : Editor
    {
        LanguageManager self;

        void SetDataSet(string dataSet)
        {
            self.usingDataSet = dataSet;
        }

        void ReloadData()
        {
            // Reset array
            LanguageManager.ReloadResourceData();
        }

        void OnEnable()
        {
            self = (LanguageManager)target;
            ReloadData();
        }

        public override void OnInspectorGUI()
        {
            if (LanguageManager.dataSets.Count > 0)
            {
                // Init the dataset/
                if (!LanguageManager.dataSets.ContainsKey(self.usingDataSet))
                {
                    self.usingDataSet = LanguageManager.dataSets.Keys.ToArray()[0];
                }
                string[] array = LanguageManager.dataSets.Keys.ToArray();
                int index = EditorGUILayout.Popup(self.dataSetIndex, array);
                if (index != self.dataSetIndex)
                {
                    SetDataSet(array[self.dataSetIndex = index]);
                }
                if (GUI.Button(EditorGUILayout.GetControlRect(GUILayout.MinWidth(20)), "Reload Data"))
                {
                    ReloadData();
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("languagePreview"));
                if (GUI.Button(EditorGUILayout.GetControlRect(GUILayout.MinWidth(20)), "Apply"))
                {
                    self.ApplyLanguageModifacation(self.languagePreview);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("languageObjects"));
                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                EditorGUILayout.LabelField("Please create at least one LanguageData at 'Resources/Language' folder");
            }

        }
    }
}