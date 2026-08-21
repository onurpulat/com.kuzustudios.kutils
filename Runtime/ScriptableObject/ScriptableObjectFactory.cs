using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace KuzuStudios.Kutils.SO
{
    public class ScriptableObjectFactory<T> where T : ScriptableObject
    {
        protected const string _default_scriptable_object_path = "Assets/KuzuGenerated";

        private string _soName = string.Empty;
        private string _soPath = string.Empty;
        private ScriptableObject _parentSO = null;

        public ScriptableObjectFactory<T> Reset()
        {
            
            _soName = string.Empty;
            _soPath = string.Empty;
            _parentSO = null;
            return this;
        }

        public ScriptableObjectFactory<T> SetName(string name)
        {
            _soName = name;
            return this;
        }

        public ScriptableObjectFactory<T> SetPath(string path)
        {
            if (_parentSO != null)
            {
                Debug.LogWarning($"[ScriptableObjectFactory] The path is being set. Not necessary when a parent ScriptableObject is already assigned.");
            }

            _soPath = path;
            return this;
        }

        public ScriptableObjectFactory<T> SetParentSO(ScriptableObject parentSO)
        {
            if (!string.IsNullOrEmpty(_soPath))
            {
                Debug.LogWarning($"[ScriptableObjectFactory] The parent ScriptableObject is being set. Not necessary when a path is already assigned.");
            }

            _parentSO = parentSO;
            return this;
        }

        public T Create()
        {
            T newSO = ScriptableObject.CreateInstance<T>();

            if (string.IsNullOrEmpty(_soName))
            {
                _soName = $"New {typeof(T).Name}";
            }

            _soName = ScriptableObjectUtils.GetUniqueScriptableObjectName<T>(_soName);

            if (_parentSO == null)
            {
                if (string.IsNullOrEmpty(_soPath))
                {
                    _soPath = $"{_default_scriptable_object_path}/{typeof(T).Name}";

                    if (!Directory.Exists(_soPath))
                    {
                        Directory.CreateDirectory(_soPath);
                    }
                }

                _soPath = Path.Combine(_soPath, $"{_soName}.asset");
                AssetDatabase.CreateAsset(newSO, _soPath);

                ScriptableObjectUtils.SaveScriptableObject(newSO);
            }
            else
            {
                newSO.name = _soName;
                AssetDatabase.AddObjectToAsset(newSO, _parentSO);
                ScriptableObjectUtils.SaveScriptableObject(newSO, _parentSO);
            }

            return newSO;
        }
    }
}