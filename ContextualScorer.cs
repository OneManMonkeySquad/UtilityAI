using UnityEngine;

namespace UtilityAI {
    public abstract class ContextualScorer : ScriptableObject {
        public abstract float Score(IContext context);
    }
}