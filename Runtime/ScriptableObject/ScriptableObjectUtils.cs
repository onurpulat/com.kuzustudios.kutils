using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KuzuStudios.Kutils
{
    public static class ScriptableObjectUtils
    {
        public static void SaveScriptableObject(params ScriptableObject[] scriptableObjects)
        {
            MarkDirty(scriptableObjects);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void MarkDirty(params ScriptableObject[] scriptableObjects)
        {
            foreach (var scriptableObject in scriptableObjects)
            {
                if (scriptableObject == null) continue;
                EditorUtility.SetDirty(scriptableObject);
            }
        }

        public static void DeleteScriptableObject(ScriptableObject scriptableObject)
        {
            if (scriptableObject == null) return;

            string path = AssetDatabase.GetAssetPath(scriptableObject);
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.DeleteAsset(path);

                SaveScriptableObject();
            }
        }

        public static void DeleteFromScriptableObject(ScriptableObject scriptableObject)
        {
            if (scriptableObject == null) return;

            AssetDatabase.RemoveObjectFromAsset(scriptableObject);
            
            DeleteScriptableObject(scriptableObject);
        }

        public static List<T> FindScriptableObjects<T>() where T : ScriptableObject
        {
            List<T> results = new List<T>();

            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);

                if (asset != null)
                {
                    results.Add(asset);
                }
            }

            return results;
        }  

        public static string GetUniqueScriptableObjectName<T>(string baseName) where T : ScriptableObject
        {
            var soNameList = FindScriptableObjects<T>().Select(s=> s.name).ToList();            
            
            return StringUtils.GetUniqueName(baseName, soNameList);
        }

        public static void ChangeScriptableObjectName<T>(T scriptableObject, string tmpName) where T : ScriptableObject
        {
            if (scriptableObject == null || string.IsNullOrEmpty(tmpName)) return;

            string assetPath = AssetDatabase.GetAssetPath(scriptableObject);

            if (string.IsNullOrEmpty(assetPath)) return;
            
            tmpName = GetUniqueScriptableObjectName<T>(tmpName);

            AssetDatabase.RenameAsset(assetPath, tmpName);
            SaveScriptableObject(scriptableObject);
        }

        public static void ChangeChildScriptableObjectName<T>(T scriptableObject, string tmpName, List<string> list) where T : ScriptableObject
        {
            tmpName = StringUtils.GetUniqueName(tmpName, list);
            scriptableObject.name = tmpName;

            MarkDirty(scriptableObject);
        }

        public static bool TryGetMainObjectAs<T>(ScriptableObject subSo, out T mainSo)
            where T : ScriptableObject
        {
            mainSo = null;

            if (subSo == null) return false;

            string assetPath = AssetDatabase.GetAssetPath(subSo);
            if (string.IsNullOrEmpty(assetPath)) return false;

            var asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
            if (asset is T typedAsset)
            {
                mainSo = typedAsset;
                return true;
            }

            return false;
        }
    }
}