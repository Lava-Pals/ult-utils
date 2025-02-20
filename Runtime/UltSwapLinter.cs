#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using Object = UnityEngine.Object;

namespace UltUtils.Runtime
{
    public class UltSwapLinter : MonoBehaviour
    {
        public GameObject dummyGameObject;
        public GameObject referenceHolderGameObject;
        [FormerlySerializedAs("silence")] public bool silenced;
        [NonSerialized] public XRInteractorAffordanceStateProvider ReferenceHolder;
        
        public static void SilenceLintersRecursively(GameObject go)
        {
            var linters = go.GetComponentsInChildren<UltSwapLinter>();
            foreach (var linter in linters)
            {
                linter.silenced = true;
            }
        }
        
        [ContextMenu("Silence")]
        private void Silence() => silenced = true;
    }
}
#endif