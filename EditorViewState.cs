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
        List<NodeState> states;

        public bool TryGet(ScriptableObject obj, out Vector2 pos) {
            if (states != null) {
                for (int i = 0; i < states.Count; ++i) {
                    var state = states[i];
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
            if (states == null) {
                states = new List<NodeState>();
            }
            for (int i = 0; i < states.Count; ++i) {
                var state = states[i];
                if (state.ownedScriptableObject == obj) {
                    state.position = pos;
                    states[i] = state;
                    return;
                }
            }
            states.Add(new NodeState() { ownedScriptableObject = obj, position = pos });
        }
    }
}