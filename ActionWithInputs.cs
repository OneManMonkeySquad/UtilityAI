using System.Collections.Generic;

namespace UtilityAI {
    public abstract class ActionWithInputsBase : Action {
        public List<InputScorerBase> scorers;
    }

    public abstract class ActionWithInputs<T> : ActionWithInputsBase {
        protected T GetBest(IContext context, IEnumerable<T> inputs) {
            var bestOption = default(T);
            var bestScore = 0f;
            foreach (var input in inputs) {
                var totalScore = 1f;

                foreach (var scorer in scorers) {
                    var sc = (InputScorer<T>)scorer;
                    var score = sc.Score(context, input);
                    totalScore *= score;
                }

                if (totalScore > bestScore) {
                    bestScore = totalScore;
                    bestOption = input;
                }
            }
            return bestOption;
        }
    }
}