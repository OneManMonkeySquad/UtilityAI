using System.Collections.Generic;
using UnityEditor;

namespace UtilityAI {
    public class QualifierNode : Node {
        public readonly Qualifier qualifier;

        public ConnectionPoint actionIn;
        public ConnectionPoint scorersIn;
        public ConnectionPoint qualifierOut;

        public QualifierNode(Qualifier qualifier, NodeContext context)
            : base(400, 200, qualifier.GetType().FullName, context, qualifier) {
            this.qualifier = qualifier;

            actionIn = AddConnectionPoint(ConnectionPointType.In, "Action");
            actionIn.AcceptConnect = OnActionAcceptConnect;

            scorersIn = AddConnectionPoint(ConnectionPointType.In, "Scorers");
            scorersIn.AcceptConnect = OnScorersAcceptConnect;

            qualifierOut = AddConnectionPoint(ConnectionPointType.Out, "Qualifier");
        }

        protected override void DrawContent() {
            var inspector = Editor.CreateEditor(qualifier);
            inspector.DrawDefaultInspector();
        }

        bool OnActionAcceptConnect(ConnectionPoint cp) {
            var an = cp.node as ActionNode;
            if (an == null)
                return false;

            qualifier.action = an.action;
            return true;
        }

        bool OnScorersAcceptConnect(ConnectionPoint cp) {
            var sn = cp.node as ContextualScorerNode;
            if (sn == null)
                return false;

            if (qualifier.scorers == null) {
                qualifier.scorers = new List<ContextualScorer>();
            }
            qualifier.scorers.Add(sn.scorer);
            return true;
        }
    }
}
