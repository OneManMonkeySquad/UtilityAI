using UnityEngine;

namespace UtilityAI.Editor {
    public class ContextualScorerNode : Node {
        public readonly ContextualScorerBase scorer;

        public Port scorerOut;

        public ContextualScorerNode(ContextualScorerBase scorer, NodeContext context)
          : base(scorer.GetType().FullName, context, scorer) {
            this.scorer = scorer;

            scorerOut = AddPort(PortType.Out, "Context. Scorer");
        }

        protected override void DrawContent() {
            var inspector = UnityEditor.Editor.CreateEditor(scorer);
            inspector.DrawDefaultInspector();
        }
    }
}