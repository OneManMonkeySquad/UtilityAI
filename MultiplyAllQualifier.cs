namespace UtilityAI {
    public class MultiplyAllQualifier : Qualifier {
        public override float Score(IAIContext context) {
            var totalScore = 1f;
            foreach (var scorer in scorers) {
                var score = scorer.Score(context);
                totalScore *= score;

                if (AIDebuggingHook.debugger != null) {
                    AIDebuggingHook.debugger.ContextualScorer(scorer, score);
                }
            }

            return totalScore;
        }
    }
}