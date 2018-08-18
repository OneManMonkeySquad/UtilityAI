using UnityEditor;

namespace UtilityAI {
    public class ContextualScorerNode : Node {
        public readonly ContextualScorer scorer;

        public ConnectionPoint scorerOut;

        public ContextualScorerNode(ContextualScorer scorer, NodeContext context)
          : base(400, 100, scorer.GetType().FullName, context, scorer) {
            this.scorer = scorer;

            scorerOut = AddConnectionPoint(ConnectionPointType.Out, "Conextual Scorer");
        }

        protected override void DrawContent() {
            var inspector = Editor.CreateEditor(scorer);
            inspector.DrawDefaultInspector();
        }
    }
}