using System.Collections.Generic;

namespace UtilityAI.Editor {
    public class SelectorNode : Node {
        public readonly Selector selector;

        public Port qualifiersIn;
        public Port defaultQualifier;
        public Port selectorOut;

        public SelectorNode(Selector selector, NodeContext context)
            : base(selector.GetType().FullName, context, selector) {
            this.selector = selector;

            qualifiersIn = AddPort(PortType.In, "Qualifiers");
            qualifiersIn.AcceptConnect = OnQualifiersAcceptConnect;
            qualifiersIn.OnDisconnect = OnQualifiersDisconnect;

            defaultQualifier = AddPort(PortType.In, "Default Qualifier");
            defaultQualifier.AcceptConnect = OnDefaultQualifierAcceptConnect;
            defaultQualifier.OnDisconnect = OnDefaultQualifierDisconnect;

            selectorOut = AddPort(PortType.Out, "Selector");
        }

        protected override void DrawContent() {
            var inspector = UnityEditor.Editor.CreateEditor(selector);
            inspector.DrawDefaultInspector();
        }

        bool OnQualifiersAcceptConnect(Port cp) {
            var qn = cp.node as QualifierNode;
            if (qn == null)
                return false;

            if (selector.qualifiers == null) {
                selector.qualifiers = new List<Qualifier>();
            }
            selector.qualifiers.Add(qn.qualifier);
            return true;
        }

        void OnQualifiersDisconnect(Port cp) {
            var qn = (QualifierNode)cp.node;

            if (selector.qualifiers == null)
                return;

            selector.qualifiers.Remove(qn.qualifier);
        }

        bool OnDefaultQualifierAcceptConnect(Port cp) {
            var qn = cp.node as QualifierNode;
            if (qn == null)
                return false;

            selector.defaultQualifier = qn.qualifier;
            return true;
        }

        void OnDefaultQualifierDisconnect(Port cp) {
            selector.defaultQualifier = null;
        }
    }
}
