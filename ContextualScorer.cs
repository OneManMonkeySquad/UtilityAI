namespace UtilityAI {
    public abstract class ContextualScorer<C> : ContextualScorerBase where C : IAIContext {
        protected abstract float RawScore(C context);

        protected override float RawScore(IAIContext context) {
            return RawScore((C)context);
        }
    }
}