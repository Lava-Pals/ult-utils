#if UNITY_EDITOR
using System.IO;
using UltEvents;
using UltUtils.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;

namespace LavaPals.Editor
{ 
    public static class UltUtils
    {
        private static class UltSwapUtils
        {
            private static GameObject _ultSwapPrefab;
            public static GameObject UltSwapPrefab
            {
                get
                {
                    _ultSwapPrefab =
                        AssetDatabase.LoadAssetAtPath<GameObject>(
                            "Packages/com.lavapals.ultutils/Assets/UltSwap.prefab") ??
                        AssetDatabase.LoadAssetAtPath<GameObject>(
                            "Assets/Packages/com.lavapals.ultutils/Assets/UltSwap.prefab");

                    return _ultSwapPrefab;
                }
            }
            
            private static GameObject _ultSwapFromParentPrefab;
            public static GameObject UltSwapFromParentPrefab
            {
                get
                {
                    _ultSwapFromParentPrefab =
                        AssetDatabase.LoadAssetAtPath<GameObject>(
                            "Packages/com.lavapals.ultutils/Assets/UltSwapFromParent.prefab") ??
                        AssetDatabase.LoadAssetAtPath<GameObject>(
                            "Assets/Packages/com.lavapals.ultutils/Assets/UltSwapFromParent.prefab");

                    return _ultSwapFromParentPrefab;
                }
            }
            
            private static void RemoveAllComponents(GameObject gameObject)
            {
                foreach (var comp in gameObject.GetComponents<Component>())
                {
                    if (comp is Transform) continue;
                    Object.DestroyImmediate(comp);
                }
            }

            private static T SetupDummyComponent<T>(GameObject dummyGameObject, T sourceComponent) where T : Component
            {
                var dummyComponent = (T)dummyGameObject.AddComponent(sourceComponent.GetType());

                if (dummyComponent == null)
                {
                    // It'll be null if we try and add a Transform, or anything else that's invalid
                    dummyComponent = (T)dummyGameObject.GetComponent(sourceComponent.GetType());
                }
                else
                {
                    EditorUtility.CopySerialized(sourceComponent, dummyComponent);
                }

                return dummyComponent;
            }

            internal static void GenerateComponentUltSwapFromParent(Component component, bool destroySource=false)
            {
                var prefab = UltSwapFromParentPrefab;
                var instance = Object.Instantiate(prefab, component.transform.parent);
                instance.transform.SetSiblingIndex(component.transform.GetSiblingIndex() + 1);

                var rootHolder = instance.GetComponent<UltEventHolder>();
                rootHolder.Event.PersistentCallsList[3].PersistentArguments[0].String = GetTypeName(component);
                
                var dummyGameObject = instance.transform.Find("Swapped").GetChild(0).gameObject;
                RemoveAllComponents(dummyGameObject);
                
                var dummyComponent = SetupDummyComponent(dummyGameObject, component);
                
                var referenceHolder = instance.transform.Find("Holders/ReferenceHolder").GetComponent<XRInteractorAffordanceStateProvider>();
                referenceHolder.interactorSource = dummyComponent;
                
                var swappedHolder = instance.transform.Find("Swapped").GetComponent<UltEventHolder>();
                swappedHolder.Event.PersistentCallsList[0].SetMethod(null, dummyComponent);
                
                instance.name = component is Transform
                    ? $"{prefab.name} (Transform Swap - Keep Disabled!)"
                    : $"{prefab.name} ({component.GetType().Name})";
                
                if (component is Transform) instance.SetActive(false);

                if (destroySource)
                {
                    Undo.DestroyObjectImmediate(component.gameObject);
                }
                
                Undo.RegisterCreatedObjectUndo(instance, "Generate UltSwap (From Parent)");
                
                Selection.activeObject = swappedHolder;
                EditorGUIUtility.PingObject(swappedHolder);
            }

            internal static void GenerateAssetsUltSwapFromParent(Object dummyObject)
            {
                var prefab = UltSwapFromParentPrefab;
                var instance = Object.Instantiate(prefab);

                var rootHolder = instance.GetComponent<UltEventHolder>();
                rootHolder.Event.PersistentCallsList[3].PersistentArguments[0].String = GetTypeName(dummyObject);
                
                // The dummy GameObject has no use here
                Object.DestroyImmediate(instance.transform.Find("Swapped").GetChild(0).gameObject);
                
                var referenceHolder = instance.transform.Find("Holders/ReferenceHolder").GetComponent<XRInteractorAffordanceStateProvider>();
                referenceHolder.interactorSource = dummyObject;
                
                var swappedHolder = instance.transform.Find("Swapped").GetComponent<UltEventHolder>();
                swappedHolder.Event.PersistentCallsList[0].SetMethod(null, dummyObject);
                
                instance.name = $"{prefab.name} ({dummyObject.GetType().Name})";
                
                Undo.RegisterCreatedObjectUndo(instance, "Generate UltSwap (From Parent)");
                
                Selection.activeObject = swappedHolder;
                EditorGUIUtility.PingObject(swappedHolder);
            }

            internal static void GenerateAssetsUltSwap(Object dummyObject)
            {
                var prefab = UltSwapPrefab;
                var instance = Object.Instantiate(prefab);
                
                // The dummy GameObject has no use here
                Object.DestroyImmediate(instance.transform.Find("Swapped").GetChild(0).gameObject);
                
                var referenceHolder = instance.transform.Find("Holders/ReferenceHolder").GetComponent<XRInteractorAffordanceStateProvider>();
                referenceHolder.interactorSource = dummyObject;
                
                var swappedHolder = instance.transform.Find("Swapped").GetComponent<UltEventHolder>();
                swappedHolder.Event.PersistentCallsList[0].SetMethod(null, dummyObject);
                
                instance.name = $"{prefab.name} ({dummyObject.GetType().Name})";
                
                Undo.RegisterCreatedObjectUndo(instance, "Generate UltSwap (From Parent)");
                
                Selection.activeObject = swappedHolder;
                EditorGUIUtility.PingObject(swappedHolder);
            }

            internal static void GenerateComponentUltSwap(Component component)
            {
                var prefab = UltSwapPrefab;
                var instance = Object.Instantiate(prefab, component.transform.parent);
                instance.transform.SetSiblingIndex(component.transform.GetSiblingIndex() + 1);
            
                var dummyGameObject = instance.transform.Find("Swapped").GetChild(0).gameObject;
                RemoveAllComponents(dummyGameObject);
                var dummyComponent = SetupDummyComponent(dummyGameObject, component);
            
                instance.name = $"{prefab.name} ({component.GetType().Name})";
            
                var swappedHolder = instance.transform.Find("Swapped").GetComponent<UltEventHolder>();
                swappedHolder.Event.PersistentCallsList[0].SetMethod(null, dummyComponent);
            
                var referenceHolder = instance.transform.Find("Holders/ReferenceHolder").GetComponent<XRInteractorAffordanceStateProvider>();
                referenceHolder.interactorSource = dummyComponent;
            
                Undo.DestroyObjectImmediate(component.gameObject);
                Undo.RegisterCreatedObjectUndo(instance, "Generate UltSwap (Manual)");
            
                Selection.activeObject = swappedHolder;
                EditorGUIUtility.PingObject(swappedHolder);
            }

            internal static void GenerateGameObjectUltSwap(GameObject gameObject)
            {
                var prefab = UltSwapPrefab;
                var instance = Object.Instantiate(prefab, gameObject.transform.parent);
                instance.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex() + 1);
            
                var dummyGameObject = instance.transform.Find("Swapped").GetChild(0).gameObject;
                RemoveAllComponents(dummyGameObject);
            
                instance.name = $"{prefab.name} ({gameObject.GetType().Name})";
            
                var swappedHolder = instance.transform.Find("Swapped").GetComponent<UltEventHolder>();
                swappedHolder.Event.PersistentCallsList[0].SetMethod(null, dummyGameObject);
            
                var referenceHolder = instance.transform.Find("Holders/ReferenceHolder").GetComponent<XRInteractorAffordanceStateProvider>();
                referenceHolder.interactorSource = dummyGameObject;
            
                Undo.DestroyObjectImmediate(gameObject);
                Undo.RegisterCreatedObjectUndo(instance, "Generate UltSwap on GameObject (Manual)");
            
                Selection.activeObject = swappedHolder;
                EditorGUIUtility.PingObject(swappedHolder);
            }
        }
        
        [MenuItem("CONTEXT/Component/Ult Utils/JSON Serialize/Copy Pretty")]
        private static void LogJsonPretty(MenuCommand menuCommand)
        {
            var component = menuCommand.context as Component;
            var json = JsonUtility.ToJson(component, prettyPrint: true);
            Debug.Log(json);
            EditorGUIUtility.systemCopyBuffer = json;
        }
        
        [MenuItem("CONTEXT/Component/Ult Utils/JSON Serialize/Copy Ugly")]
        private static void LogJsonUgly(MenuCommand menuCommand)
        {
            var component = menuCommand.context as Component;
            var json = JsonUtility.ToJson(component, prettyPrint: false);
            Debug.Log(json);
            EditorGUIUtility.systemCopyBuffer = json;
        }
        
        [MenuItem("CONTEXT/Component/Ult Utils/Copy Type Name")]
        private static void CopyTypeName(MenuCommand menuCommand)
        {
            var component = (Component)menuCommand.context;
            var output = GetTypeName(component);
            EditorGUIUtility.systemCopyBuffer = output;
            Debug.Log($"Copied {output} to clipboard");
        }

        [MenuItem("CONTEXT/Component/Ult Utils/Convert to Ult Swap/Manual")]
        private static void GenerateComponentUltSwap(MenuCommand menuCommand)
        {
            var component = (Component)menuCommand.context;
            UltSwapUtils.GenerateComponentUltSwap(component);
        }
        
        [MenuItem("GameObject/Ult Utils/Convert to Ult Swap (Manual)")]
        private static void GenerateGameObjectUltSwap(MenuCommand menuCommand)
        {
            var gameObject = (GameObject)menuCommand.context;
            UltSwapUtils.GenerateGameObjectUltSwap(gameObject);
        }

        private static void CopyAndInvoke(GameObject gameObject)
        {
            var copy = Object.Instantiate(gameObject, gameObject.transform.parent);
            UltSwapLinter.SilenceLintersRecursively(copy);
            copy.name = $"{gameObject.name} (Copy)";
            copy.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex() + 1);
            
            Undo.RegisterCreatedObjectUndo(copy, "Invoke on Copy");
            
            var copyHolder = copy.GetComponent<UltEventHolder>();
            copyHolder.Invoke();
            
            Selection.activeObject = copyHolder;
            EditorGUIUtility.PingObject(copyHolder);
        }
        
        [MenuItem("GameObject/Ult Utils/Invoke on Copy")]
        private static void GameObjectCopyAndInvoke(MenuCommand menuCommand)
        {
            var gameObject = (GameObject)menuCommand.context;
            
            if (!gameObject.TryGetComponent<UltEventHolder>(out _))
            {
                Debug.LogError("No UltEventHolder found on GameObject");
                return;
            }
            
            CopyAndInvoke(gameObject);
        }
        
        [MenuItem("CONTEXT/UltEventHolder/(Ult Utils) Invoke on Copy")]
        private static void ContextCopyAndInvoke(MenuCommand menuCommand)
        {
            var holder = (UltEventHolder)menuCommand.context;
            CopyAndInvoke(holder.gameObject);
        }
        
        [MenuItem("CONTEXT/Component/Ult Utils/Convert to Ult Swap/From Parent")]
        private static void GenerateComponentUltSwapFromParent(MenuCommand menuCommand)
        {
            UltSwapUtils.GenerateComponentUltSwapFromParent((Component)menuCommand.context, destroySource: true);
        }
        
        [MenuItem("Assets/Ult Utils/Ult Swap/Manual")]
        private static void GenerateAssetsUltSwap()
        {
            var obj = Selection.GetFiltered<Object>(SelectionMode.Assets)[0];

            if (obj is GameObject)
            {
                Debug.LogError(
                    "GameObject Ult Swaps cannot be generated through the project window - please use the Hierarchy instead.");
                return;
            }
            
            UltSwapUtils.GenerateAssetsUltSwap(obj);
        }
        
        [MenuItem("Assets/Ult Utils/Ult Swap/From Parent")]
        private static void GenerateAssetsUltSwapFromParent()
        {
            var obj = Selection.GetFiltered<Object>(SelectionMode.Assets)[0];

            if (obj is GameObject)
            {
                Debug.LogError(
                    "GameObject Ult Swaps cannot be generated through the project window - please use the Hierarchy instead.");
                return;
            }
            
            UltSwapUtils.GenerateAssetsUltSwapFromParent(obj);
        }
        
        private static string GetTypeName(Object component)
        {
            var name = $"{component.GetType().FullName}, {Path.GetFileName(component.GetType().Assembly.Location)}";
            return name.EndsWith(".dll") ? name[..^4] : name;
        }
    }
}
#endif