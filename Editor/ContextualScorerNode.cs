using UnityEditor;

namespace Cube.UtilityAI {
    public class ContextualScorerNode : Node {
        public readonly ContextualScorer scorer;

        public Port scorerOut;

        public ContextualScorerNode(ContextualScorer scorer, NodeContext context)
          : base(400, 100, scorer.GetType().FullName, context, scorer) {
            this.scorer = scorer;

            scorerOut = AddPort(PortType.Out, "Contextual Scorer");
        }

        protected override void DrawContent() {
            var inspector = Editor.CreateEditor(scorer);
            inspector.DrawDefaultInspector();
        }
    }
}