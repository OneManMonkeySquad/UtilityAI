using System.Collections.Generic;

namespace UtilityAI {
    public abstract class ActionWithInputs<C, I> : ActionWithInputsBase where C : IAIContext {
        public abstract void Execute(C context);
        public abstract void Stop(C context);

        public override void Execute(IAIContext context) {
            Execute((C)context);
        }
        public override void Stop(IAIContext context) {
            Stop((C)context);
        }

        /// <summary>
        /// Get the input with the best score (using the InputScorers). default(I) if empty, first input if there are no scorers.
        /// </summary>
        /// <param name="context">The context passed to the scorers</param>
        /// <param name="inputs">The possible inputs the scorers can select from. Usually from the context</param>
        /// <returns></returns>
        protected I GetBest(C context, IEnumerable<I> inputs) {
            var bestOption = default(I);
            var bestScore = 0f;
            foreach (var input in inputs) {
                var totalScore = 1f;

                foreach (var scorer in scorers) {
                    var sc = (InputScorer<I>)scorer;
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