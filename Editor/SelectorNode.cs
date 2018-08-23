using System.Collections.Generic;

namespace UtilityAI.Editor {
    public class SelectorNode : Node {
        public readonly Selector selector;

        public Port qualifiersIn;
        public Port selectorOut;

        public SelectorNode(Selector selector, NodeContext context)
            : base(400, 100, selector.GetType().FullName, context, selector) {
            this.selector = selector;

            qualifiersIn = AddPort(PortType.In, "Qualifiers");
            qualifiersIn.AcceptConnect = OnAcceptConnect;
            qualifiersIn.OnDisconnect = OnDisconnect;

            selectorOut = AddPort(PortType.Out, "Selector");
        }

        protected override void DrawContent() {
            var inspector = UnityEditor.Editor.CreateEditor(selector);
            inspector.DrawDefaultInspector();
        }

        bool OnAcceptConnect(Port cp) {
            var qn = cp.node as QualifierNode;
            if (qn == null)
                return false;

            if (selector.qualifiers == null) {
                selector.qualifiers = new List<Qualifier>();
            }
            selector.qualifiers.Add(qn.qualifier);
            return true;
        }

        void OnDisconnect(Port cp) {
            var qn = (QualifierNode)cp.node;

            if (selector.qualifiers == null)
                return;

            selector.qualifiers.Remove(qn.qualifier);
        }
    }
}
