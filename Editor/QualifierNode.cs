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

        bool OnActionAcceptConnect(Port cp) {
            var an = cp.node as ActionNode;
            if (an == null)
                return false;

            qualifier.action = an.action;
            return true;
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
    }
}
