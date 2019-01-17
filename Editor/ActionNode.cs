namespace UtilityAI.Editor {
    public class ActionNode : Node {
        public readonly ActionBase action;

        public Port actionOut;

        public ActionNode(ActionBase action, NodeContext context)
            : base(action.GetType().FullName, context, action) {
            this.action = action;

            actionOut = AddPort(PortType.Out, "Action");
        }

        protected override void DrawContent() {
            var inspector = UnityEditor.Editor.CreateEditor(action);
            inspector.DrawDefaultInspector();
        }
    }
}
