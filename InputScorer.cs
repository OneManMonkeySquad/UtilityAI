using UnityEngine;

namespace Cube.UtilityAI {
    public class InputScorerBase : ScriptableObject {
    }

    public abstract class InputScorer<T> : InputScorerBase {
        public abstract float Score(IContext context, T input);
    }
}