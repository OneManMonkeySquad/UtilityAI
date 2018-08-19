using System.Collections.Generic;
using UnityEditor;

namespace UtilityAI {
    public class QualifierNode : Node {
        public readonly Qualifier qualifier;

        public Port actionIn;
        public Port scorersIn;
        public Port qualifierOut;

        public QualifierNode(Qualifier qualifier, NodeContext context)
            : base(400, 200, qualifier.GetType().FullName, context, qualifier) {
            this.qualifier = qualifier;

            actionIn = AddPort(PortType.In, "Action");
            actionIn.AcceptConnect = OnActionAcceptConnect;
            actionIn.OnDisconnect = OnActionDisconnect;

            scorersIn = AddPort(PortType.In, "Contextual Scorers");
            scorersIn.AcceptConnect = OnScorersAcceptConnect;
            scorersIn.OnDisconnect = OnScorersDisconnect;

            qualifierOut = AddPort(PortType.Out, "Qualifier");
        }

        protected override void DrawContent() {
            var inspector = Editor.CreateEditor(qualifier);
            inspector.DrawDefaultInspector();
        }

        bool OnActionAcceptConnect(Port cp) {
            var an = cp.node as ActionNode;
            if (an == null)
                return false;

            qualifier.action = an.action;
            return true;
        }

        void OnActionDisconnect(Port cp) {
            qualifier.action = null;
        }

        bool OnScorersAcceptConnect(Port cp) {
            var sn = cp.node as ContextualScorerNode;
            if (sn == null)
                return false;

            if (qualifier.scorers == null) {
                qualifier.scorers = new List<ContextualScorer>();
            }
            qualifier.scorers.Add(sn.scorer);
            return true;
        }

        void OnScorersDisconnect(Port cp) {
            var sn = (ContextualScorerNode)cp.node;

            if (qualifier.scorers == null)
                return;

            qualifier.scorers.Remove(sn.scorer);
        }
    }
}
