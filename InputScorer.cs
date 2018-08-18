using UnityEngine;

namespace UtilityAI {
    public class InputScorerBase : ScriptableObject {
    }

    public abstract class InputScorer<T> : InputScorerBase {
        public abstract float Score(IContext context, T input);
    }
}