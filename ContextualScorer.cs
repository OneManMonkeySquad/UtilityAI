using UnityEngine;

namespace Cube.UtilityAI {
    public abstract class ContextualScorer : ScriptableObject {
        public AnimationCurve mapping = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

        public float Score(IContext context) {
            var rawScore = RawScore(context);
            return mapping.Evaluate(rawScore);
        }

        protected abstract float RawScore(IContext context);
    }
}