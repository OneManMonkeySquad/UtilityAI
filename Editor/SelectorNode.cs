using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UtilityAI {
    public class SelectorNode : Node {
        public readonly Selector selector;

        public ConnectionPoint qualifiersIn;
        public ConnectionPoint selectorOut;

        public SelectorNode(Selector selector, NodeContext context)
            : base(400, 100, "Selector", context, selector) {
            this.selector = selector;

            qualifiersIn = AddConnectionPoint(ConnectionPointType.In, "Qualifiers");
            qualifiersIn.AcceptConnect = OnAcceptConnect;

            selectorOut = AddConnectionPoint(ConnectionPointType.Out, "Selector");
        }

        protected override void DrawContent() {
            var inspector = Editor.CreateEditor(selector);
            inspector.DrawDefaultInspector();
        }

        bool OnAcceptConnect(ConnectionPoint cp) {
            var qn = cp.node as QualifierNode;
            if (qn == null)
                return false;

            if (selector.qualifiers == null) {
                selector.qualifiers = new List<Qualifier>();
            }
            selector.qualifiers.Add(qn.qualifier);
            return true;
        }
    }
}
