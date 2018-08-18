using System;
using UnityEditor;
using UnityEngine;

namespace UtilityAI {
    public class BrainNode : Node {
        public readonly AIBrain brain;

        public ConnectionPoint rootIn;

        public BrainNode(AIBrain brain, NodeContext context)
            : base(400, 100, "Brain", context, brain) {
            this.brain = brain;

            rootIn = AddConnectionPoint(ConnectionPointType.In, "Root");
            rootIn.AcceptConnect = OnAcceptConnect;
        }

        protected override void DrawContent() {
            var inspector = Editor.CreateEditor(brain);
            inspector.DrawDefaultInspector();
        }
        
        bool OnAcceptConnect(ConnectionPoint cp) {
            var sn = cp.node as SelectorNode;
            if (sn == null)
                return false;

            brain.root = sn.selector;
            return true;
        }
    }
}