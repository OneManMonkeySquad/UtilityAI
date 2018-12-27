using UnityEngine;

namespace UtilityAI {
    [CreateAssetMenu(menuName = "UtilityAI/Brain")]
    public class Brain : ScriptableObject {
        public AISettings settings;
        [HideInInspector]
        public Selector root;
#if UNITY_EDITOR
        [HideInInspector]
        public EditorViewState viewState;
#endif
    }
}