using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Gemserk.MultiSceneManager
{
    [CreateAssetMenu(menuName = "Gemserk/MultiSceneManager/SceneFavorites")]
    public class SceneFavoritesAsset : ScriptableObject
    {
        [Serializable]
        public class SceneFavorite
        {
            public string name;
            public string inheritsFrom;
            public List<SceneField> scenes;


          
        }

        public List<SceneFavorite> favorites;
    }
}