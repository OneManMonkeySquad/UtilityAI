﻿using UnityEditor;

namespace UtilityAI {
    public class ActionNode : Node {
        public readonly Action action;

        public Port actionOut;

        public ActionNode(Action action, NodeContext context)
            : base(400, 100, action.GetType().FullName, context, action) {
            this.action = action;

            actionOut = AddConnectionPoint(ConnectionPointType.Out, "Action");
        }

        protected override void DrawContent() {
            var inspector = Editor.CreateEditor(action);
            inspector.DrawDefaultInspector();
        }
    }
}
