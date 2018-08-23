using System.Collections.Generic;

namespace UtilityAI.Editor {
    public class ActionWithInputsNode : ActionNode {
        public readonly ActionWithInputsBase actionWithInputs;

        public Port inputScorersIn;

        public ActionWithInputsNode(ActionWithInputsBase action, NodeContext context)
            : base(action, context) {
            this.actionWithInputs = action;

            var actionType = action.GetType().BaseType;
            while (actionType != null) {
                if (actionType.GetGenericTypeDefinition() == typeof(ActionWithInputs<>)) {
                    title += string.Format(" <{0}>", actionType.GenericTypeArguments[0].Name);
                    break;
                }
                actionType = actionType.BaseType;
            }

            inputScorersIn = AddPort(PortType.In, "Input Scorers");
            inputScorersIn.AcceptConnect = OnAcceptConnect;
            inputScorersIn.OnDisconnect = OnDisconnect;
        }
        
        bool OnAcceptConnect(Port cp) {
            var sn = cp.node as InputScorerNode;
            if (sn == null)
                return false;

            if (actionWithInputs.scorers == null) {
                actionWithInputs.scorers = new List<InputScorerBase>();
            }
            actionWithInputs.scorers.Add(sn.scorer);
            return true;
        }

        void OnDisconnect(Port cp) {
            var sn = (InputScorerNode)cp.node;

            if (actionWithInputs.scorers == null)
                return;

            actionWithInputs.scorers.Remove(sn.scorer);
        }
    }
}
