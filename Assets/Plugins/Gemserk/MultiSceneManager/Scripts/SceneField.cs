using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gemserk.MultiSceneManager
{
    [Serializable]
    public class SceneField
    {
        [SerializeField] private string sceneGUID;
        [SerializeField] private string sceneName = "";

        public string SceneName
        {
            get { return sceneName; }
        }

        public string SceneGUID
        {
            get { return sceneGUID; }
        }

        // makes it work with the existing Unity methods (LoadLevel/LoadScene)
        public static implicit operator string(SceneField sceneField)
        {
            return sceneField.SceneName;
        }

        public static SceneField Create(string sceneGUID, string sceneName)
        {
            SceneField sceneField = new SceneField
            {
                sceneGUID = sceneGUID,
                sceneName = sceneName
            };
            return sceneField;
        }

#if UNITY_EDITOR
        public bool Fix()
        {
            var scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
            var fixedPath = FixedPath(scenePath);
            var oldPath = sceneName;
            sceneName = fixedPath;
            return oldPath != fixedPath;
        }

        public static SceneField CreateFromGUID(string guid)
        {
            SceneField sceneField = Create(guid, null);
            sceneField.Fix();
            return sceneField;
        }
#endif

        public static string FixedPath(string fullPath)
        {
            var scenePath = fullPath;
            var assetsIndex = scenePath.IndexOf("Assets", StringComparison.Ordinal) + 7;
            var extensionIndex = scenePath.LastIndexOf(".unity", StringComparison.Ordinal);
            scenePath = scenePath.Substring(assetsIndex, extensionIndex - assetsIndex);
            return scenePath;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SceneField))]
    public class SceneFieldPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            var sceneGUID = property.FindPropertyRelative("sceneGUID");
            var sceneName = property.FindPropertyRelative("sceneName");
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            if (sceneGUID != null)
            {
                EditorGUI.BeginChangeCheck();
                var sceneObj =
                    AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(sceneGUID.stringValue));
                var value = EditorGUI.ObjectField(position, sceneObj, typeof(SceneAsset), false);
                if (EditorGUI.EndChangeCheck())
                {
                    if (value != null)
                    {
                        var scenePath = AssetDatabase.GetAssetPath(value);
                        var newGUID = AssetDatabase.AssetPathToGUID(scenePath);
                        var assetsIndex = scenePath.IndexOf("Assets", StringComparison.Ordinal) + 7;
                        var extensionIndex = scenePath.LastIndexOf(".unity", StringComparison.Ordinal);
                        scenePath = scenePath.Substring(assetsIndex, extensionIndex - assetsIndex);
                        sceneGUID.stringValue = newGUID;
                        sceneName.stringValue = scenePath;
                    }
                    else
                    {
                        sceneGUID.stringValue = null;
                        sceneName.stringValue = null;
                    }
                }
            }
            EditorGUI.EndProperty();
        }
    }
#endif
}