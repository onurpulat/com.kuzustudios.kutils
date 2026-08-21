using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KuzuStudios.Kutils
{
    public class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
    {
        private static T _instance;
        private static readonly object _lock = new();
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            T[] assets = AssetDatabase.FindAssets($"t:{typeof(T).Name}")
                                .Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)))
                                .ToArray();

                            #if UNITY_EDITOR
                                if (assets == null || assets.Length == 0)
                                {
                                    // If no instance exists, create a new one and save it as an asset
                                    _instance = ScriptableObject.CreateInstance<T>();
                                    _instance.name = typeof(T).Name;

                                    string assetPath = $"Assets/{typeof(T).Name}.asset";

                                    AssetDatabase.CreateAsset(_instance, assetPath);
                                    
                                    ScriptableObjectUtils.SaveScriptableObject(_instance);
                                    _instance.OnCreate();
                                }
                            #endif
                            
                            if (assets != null)
                            {
                                _instance = assets[0];
                            }

                            if (_instance == null)
                            {
                                Debug.LogError("[Singleton Scriptable Object] Failed to create instance of " + typeof(T).Name);
                            }
                        }
                    }
                }

                return _instance;
            }
        }

        protected virtual void OnCreate() { }

        public void MakeSureCreated() { }
    }
}