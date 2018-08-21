using System;
using UnityEngine;

namespace Cube.UtilityAI {
    public class HighestScoringQualifier : Selector {
        public override Qualifier Select(IContext context) {
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

            if (bestQualifier == null)
                return null;

            return bestQualifier.Select(context);
        }
    }
}