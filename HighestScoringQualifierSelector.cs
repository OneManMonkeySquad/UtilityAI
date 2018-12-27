using UnityEngine;

namespace UtilityAI {
    public class HighestScoringQualifierSelector : Selector {
        [Range(0, 1)]
        public float minScoreThreshold = 0;

        public override Qualifier Select(IAIContext context) {
            Qualifier bestQualifier = defaultQualifier;
            float bestScore = defaultQualifier != null ? defaultQualifier.Score(context) : float.MinValue;
            for (int i = 0; i < qualifiers.Count; ++i) {
                var qualifier = qualifiers[i];
                var score = qualifier.Score(context);

                if (score > bestScore) {
                    bestScore = score;
                    bestQualifier = qualifier;
                }
            }

            if (AIDebuggingHook.debugger != null) {
                AIDebuggingHook.debugger.BestQualifier(bestQualifier, this);
            }

            if (bestQualifier == null || bestScore < minScoreThreshold)
                return null;
            
            return bestQualifier.Select(context);
        }
    }
}