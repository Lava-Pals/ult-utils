#if UNITY_EDITOR
using System.Collections.Generic;
using UltUtils.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;

namespace LavaPals.Editor
{
    [CustomEditor(typeof(UltSwapLinter))]
    public class UltSwapLinterEditor : UnityEditor.Editor
    {
        private bool _showDummyScriptFoldout = true;
        
        private static bool ObjectField<T>(string text, out T go) where T : Object
        {
            go = (T)EditorGUILayout.ObjectField(text, null, typeof(T), true);
            return go;
        }
        
        private static bool Foldout(string text, ref bool boolean)
        {
            boolean = EditorGUILayout.Foldout(boolean, text);
            return boolean;
        }
        
        private static bool Toggle(string text, ref bool boolean)
        {
            boolean = EditorGUILayout.Toggle(text, boolean);
            return boolean;
        }
        
        public override void OnInspectorGUI()
        {
            var linter = (UltSwapLinter)target;

            if (linter.silenced)
            {
                if (!Toggle("Silenced", ref linter.silenced))
                {
                    EditorUtility.SetDirty(linter);
                }
                
                return;
            }

            if (!linter.referenceHolderGameObject)
            {
                EditorGUILayout.HelpBox("Drag the 'ReferenceHolder' Game Object of the UltSwap into the field below.", MessageType.Warning);

                if (!ObjectField<GameObject>("Reference Holder", out var gameObject)) return;
                linter.referenceHolderGameObject = gameObject;
                EditorUtility.SetDirty(linter);

                return;
            }
            
            if (!linter.ReferenceHolder)
            {
                linter.ReferenceHolder =
                    linter.referenceHolderGameObject.GetComponent<XRInteractorAffordanceStateProvider>()
                    ?? linter.referenceHolderGameObject.gameObject.AddComponent<XRInteractorAffordanceStateProvider>();
            }

            if (!linter.dummyGameObject)
            {
                EditorGUILayout.HelpBox("Drag the 'Dummy' Game Object of the UltSwap into the field below.", MessageType.Warning);

                if (!ObjectField<GameObject>("Dummy Game Object", out var gameObject)) return;
                linter.dummyGameObject = gameObject;
                EditorUtility.SetDirty(linter);

                return;
            }

            if (linter.ReferenceHolder.interactorSource as Component)
            {
                var component = (Component) linter.ReferenceHolder.interactorSource;

                if (component.gameObject != linter.dummyGameObject)
                {
                    EditorGUILayout.HelpBox("This UltSwap's 'ReferenceHolder' is holding the incorrect script or GameObject - this often happens when you run the UltSwap in-editor without doing it on a backup.\n\nTo fix, please select the correct dummy below, or add a new script to the dummy GameObject:", MessageType.Error);
                    DisplayDummyComponents();
                    return;
                }
            }
            
            if (linter.ReferenceHolder.interactorSource == null)
            {
                EditorGUILayout.HelpBox("This UltSwap is missing its dummy script! Add the script you would like to swap with to the 'Dummy' GameObject", MessageType.Error);
                DisplayDummyComponents();
                return;
            }
            
            EditorGUILayout.HelpBox("No obvious issues found with this UltSwap! :>", MessageType.None);

            return;

            void DisplayDummyComponents()
            {
                if (GUILayout.Button("Goto Dummy GameObject"))
                {
                    Selection.activeGameObject = linter.dummyGameObject;
                    EditorGUIUtility.PingObject(linter.dummyGameObject);
                }
                
                EditorGUILayout.BeginVertical("box");
                EditorGUI.indentLevel++;
                
                if (!Foldout("Assign an existing dummy", ref _showDummyScriptFoldout))
                {
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();
                    return;
                }
                
                if (DrawGameObjectButton())
                {
                    linter.ReferenceHolder.interactorSource = linter.dummyGameObject;
                    EditorUtility.SetDirty(linter.ReferenceHolder);
                    EditorGUIUtility.PingObject(linter.dummyGameObject);
                }

                foreach (var component in linter.dummyGameObject.GetComponents<Component>())
                {
                    if (!DrawComponentButton(component)) continue;
                    linter.ReferenceHolder.interactorSource = component;
                    EditorUtility.SetDirty(linter.ReferenceHolder);
                    EditorGUIUtility.PingObject(component);
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();

                return;

                bool DrawGameObjectButton()
                {
                    var content = new GUIContent
                    {
                        image = EditorGUIUtility.ObjectContent(null, linter.dummyGameObject.GetType()).image,
                        text = linter.dummyGameObject.GetType().Name,
                        tooltip = linter.dummyGameObject.name,
                    };

                    return GUILayout.Button(content,
                        new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft }, GUILayout.Height(20));
                }

                bool DrawComponentButton(Component component)
                {
                    var content = new GUIContent
                    {
                        image = EditorGUIUtility.ObjectContent(null, component.GetType()).image,
                        text = component.GetType().Name,
                        tooltip = component.GetType().FullName,
                    };

                    return GUILayout.Button(content,
                        new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft }, GUILayout.Height(20));
                }
            }
        }
    }
}
#endif