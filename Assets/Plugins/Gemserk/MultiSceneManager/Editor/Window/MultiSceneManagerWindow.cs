using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Gemserk.MultiSceneManager.Window
{
    public class MultiSceneManagerWindow : EditorWindow
    {
        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/Gemserk/MultiSceneManager")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            MultiSceneManagerWindow window =
                (MultiSceneManagerWindow) EditorWindow.GetWindow(typeof(MultiSceneManagerWindow));
            window.Show();
        }

        private List<SceneFavoritesAsset> favoritesAssets;

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                if (GUILayout.Button("Refresh"))
                {
                    var favoriteGUIDs = AssetDatabase.FindAssets("t:SceneFavoritesAsset");
                    favoritesAssets = favoriteGUIDs.Select(s =>
                        AssetDatabase.LoadAssetAtPath<SceneFavoritesAsset>(AssetDatabase.GUIDToAssetPath(s))).ToList();
                    return;
                }
                
                EditorGUILayout.Separator();

                if (favoritesAssets == null)
                    return;
                
                foreach (var favoritesAsset in favoritesAssets)
                {
                    EditorGUILayout.LabelField(favoritesAsset.name);


                    EditorGUI.indentLevel++;
                    foreach (var favorite in favoritesAsset.favorites)
                    {
                        EditorGUILayout.LabelField(favorite.name);

                        EditorGUILayout.LabelField(string.Join("|", favorite.scenes.Select(field => { field.Fix();
                            return field.SceneName;
                        }).ToArray()));
                        
                        EditorGUILayout.BeginHorizontal();
                        {
                            bool load = false;
                            bool aditive = false;
                            if (GUILayout.Button("Load"))
                            {
                                load = true;
                            } 
                            
                            if (GUILayout.Button("Load Additive"))
                            {
                                load = true;
                                aditive = true;
                            }

                            if (load)
                            {
                                var scenes = SolveAllScenes(favorite);
                                Load(scenes, aditive);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                       
                    }
                    EditorGUI.indentLevel--;
                    
                    EditorGUILayout.Separator();

                    if (GUILayout.Button("AddActiveAsFavorite"))
                    {
                        var activeScenePath = EditorSceneManager.GetActiveScene().path;
                        var activeSceneGUID = AssetDatabase.AssetPathToGUID(activeScenePath);
                        var sceneField = SceneField.CreateFromGUID(activeSceneGUID);
                        sceneField.Fix();
                        favoritesAsset.favorites.Add(new SceneFavoritesAsset.SceneFavorite
                        {
                            name = sceneField.SceneName,
                            scenes = new List<SceneField> {sceneField}
                        });
                        this.Repaint();
                        return;
                    }
                    
                    
                    EditorGUILayout.Separator();
                }
            }
            EditorGUILayout.EndVertical();


        }

        public List<SceneField> SolveAllScenes(SceneFavoritesAsset.SceneFavorite favorite)
        {
            List<SceneField> scenes = new List<SceneField>();
            var currentFavorite = favorite;
            while (currentFavorite != null)
            {
                scenes.AddRange(currentFavorite.scenes);
                currentFavorite = this.favoritesAssets.SelectMany(asset => asset.favorites).FirstOrDefault(sceneFavorite => sceneFavorite.name == currentFavorite.inheritsFrom);
            }

            return scenes;
        }
        
        public void Load(List<SceneField> scenes, bool additive)
        {
            for (var index = 0; index < scenes.Count; index++)
            {
                var sceneField = scenes[index];
                var mode = (index > 0 || additive) ? OpenSceneMode.Additive : OpenSceneMode.Single;
                EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(sceneField.SceneGUID), mode);
            }
        }
    }
}