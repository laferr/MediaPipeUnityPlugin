using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Airpass.AudioManager
{
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Reload AudioClips"))
            {
                ((AudioManager)target).ReloadAudioClips();
                EditorUtility.SetDirty(target);
            }
            
            GUILayout.EndHorizontal();
        }
    }
}