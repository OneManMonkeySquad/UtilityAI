namespace UtilityAI.Editor {
#if UNITY_EDITOR
    public class ActionNode : Node {
        public readonly Action action;

        public Port actionOut;

        public ActionNode(Action action, NodeContext context)
            : base(400, 100, action.GetType().FullName, context, action) {
            this.action = action;

            actionOut = AddPort(PortType.Out, "Action");
        }

        protected override void DrawContent() {
            var inspector = UnityEditor.Editor.CreateEditor(action);
            inspector.DrawDefaultInspector();
        }
    }
#endif
}
