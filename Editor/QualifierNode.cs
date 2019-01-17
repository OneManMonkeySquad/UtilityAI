using System.Collections.Generic;

namespace UtilityAI.Editor {
    public class QualifierNode : Node {
        public readonly Qualifier qualifier;

        public Port actionOrSelectorIn;
        public Port scorersIn;
        public Port qualifierOut;

        public QualifierNode(Qualifier qualifier, NodeContext context)
            : base(qualifier.GetType().FullName, context, qualifier) {
            this.qualifier = qualifier;

            actionOrSelectorIn = AddPort(PortType.In, "Action / Selector");
            actionOrSelectorIn.AcceptConnect = OnActionOrSelectorAcceptConnect;
            actionOrSelectorIn.OnDisconnect = OnActionOrSelectorDisconnect;

            scorersIn = AddPort(PortType.In, "Context. Scorers");
            scorersIn.AcceptConnect = OnScorersAcceptConnect;
            scorersIn.OnDisconnect = OnScorersDisconnect;

            qualifierOut = AddPort(PortType.Out, "Qualifier");
        }

        protected override void DrawContent() {
            var inspector = UnityEditor.Editor.CreateEditor(qualifier);
            inspector.DrawDefaultInspector();
        }

        bool OnActionOrSelectorAcceptConnect(Port cp) {
            var an = cp.node as ActionNode;
            if (an != null) {
                qualifier.action = an.action;
                qualifier.selector = null;
            }
            var sn = cp.node as SelectorNode;
            if (sn != null) {
                qualifier.selector = sn.selector;
                qualifier.action = null;
            }
            return true;
        }

        void OnActionOrSelectorDisconnect(Port cp) {
            qualifier.selector = null;
            qualifier.action = null;
        }

        bool OnScorersAcceptConnect(Port cp) {
            var sn = cp.node as ContextualScorerNode;
            if (sn == null)
                return false;

            if (qualifier.scorers == null) {
                qualifier.scorers = new List<ContextualScorerBase>();
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
