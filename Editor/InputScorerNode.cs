namespace UtilityAI.Editor {
    public class InputScorerNode : Node {
        public readonly InputScorerBase scorer;

        public Port scorerOut;

        public InputScorerNode(InputScorerBase scorer, NodeContext context)
          : base(scorer.GetType().FullName, context, scorer) {
            this.scorer = scorer;

            var scorerType = scorer.GetType().BaseType;
            while (scorerType != null) {
                if (scorerType.GetGenericTypeDefinition() == typeof(InputScorer<>)) {
                    title += string.Format(" <{0}>", scorerType.GenericTypeArguments[0].Name);
                    break;
                }
                scorerType = scorerType.BaseType;
            }

            scorerOut = AddPort(PortType.Out, "Input Scorer");
        }

        protected override void DrawContent() {
            var inspector = UnityEditor.Editor.CreateEditor(scorer);
            inspector.DrawDefaultInspector();
        }
    }
}