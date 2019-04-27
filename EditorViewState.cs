using System;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI {
    [Serializable]
    public class EditorViewState {
        [Serializable]
        struct NodeState {
            public ScriptableObject ownedScriptableObject;
            public Vector2 position;
        }

        [SerializeField]
        List<NodeState> nodeStates;

        public bool TryGet(ScriptableObject obj, out Vector2 pos) {
            if (nodeStates != null) {
                for (int i = 0; i < nodeStates.Count; ++i) {
                    var state = nodeStates[i];
                    if (state.ownedScriptableObject == obj) {
                        pos = state.position;
                        return true;
                    }
                }
            }
            pos = Vector2.zero;
            return false;
        }

        public void Set(ScriptableObject obj, Vector2 pos) {
            if (nodeStates == null) {
                nodeStates = new List<NodeState>();
            }
            for (int i = 0; i < nodeStates.Count; ++i) {
                var state = nodeStates[i];
                if (state.ownedScriptableObject == obj) {
                    state.position = pos;
                    nodeStates[i] = state;
                    return;
                }
            }
            nodeStates.Add(new NodeState() {
                ownedScriptableObject = obj,
                position = pos
            });
        }
    }
}