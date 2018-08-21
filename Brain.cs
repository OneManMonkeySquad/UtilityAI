using UnityEngine;

namespace Cube.UtilityAI {
    [CreateAssetMenu(menuName = "Cube.UtilityAI/Brain")]
    public class Brain : ScriptableObject {
        public AISettings settings;
        public Selector root;
#if UNITY_EDITOR
        public EditorViewState viewState;
#endif
    }
}