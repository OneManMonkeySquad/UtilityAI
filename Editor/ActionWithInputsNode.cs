using System.Collections.Generic;
using UnityEditor;

namespace UtilityAI {
    public class ActionWithInputsNode : Node {
        public readonly ActionWithInputsBase action;

        public ConnectionPoint inputScorersIn;
        public ConnectionPoint actionOut;

        public ActionWithInputsNode(ActionWithInputsBase action, NodeContext context)
            : base(400, 100, action.GetType().FullName, context, action) {
            this.action = action;

            var actionType = action.GetType().BaseType;
            while (actionType != null) {
                if (actionType.GetGenericTypeDefinition() == typeof(ActionWithInputs<>)) {
                    title += string.Format(" <{0}>", actionType.GenericTypeArguments[0].Name);
                    break;
                }
                actionType = actionType.BaseType;
            }

            inputScorersIn = AddConnectionPoint(ConnectionPointType.In, "InputScorers");
            inputScorersIn.AcceptConnect = OnAcceptConnect;

            actionOut = AddConnectionPoint(ConnectionPointType.Out, "Action");
        }

        protected override void DrawContent() {
            var inspector = Editor.CreateEditor(action);
            inspector.DrawDefaultInspector();
        }

        bool OnAcceptConnect(ConnectionPoint cp) {
            var sn = cp.node as InputScorerNode;
            if (sn == null)
                return false;

            if (action.scorers == null) {
                action.scorers = new List<InputScorerBase>();
            }
            action.scorers.Add(sn.scorer);
            return true;
        }
    }
}
