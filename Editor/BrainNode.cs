namespace UtilityAI.Editor {
    public class BrainNode : Node {
        public readonly Brain brain;

        public Port rootIn;

        public BrainNode(Brain brain, NodeContext context)
            : base(400, 100, brain.GetType().FullName, context, brain) {
            this.brain = brain;

            rootIn = AddPort(PortType.In, "Selector");
            rootIn.AcceptConnect = OnAcceptConnect;
            rootIn.OnDisconnect = OnDisconnect;
        }

        protected override void DrawContent() {
            var inspector = UnityEditor.Editor.CreateEditor(brain);
            inspector.DrawDefaultInspector();
        }
        
        bool OnAcceptConnect(Port cp) {
            var sn = cp.node as SelectorNode;
            if (sn == null)
                return false;

            brain.root = sn.selector;
            return true;
        }

        void OnDisconnect(Port cp) {
            brain.root = null;
        }
    }
}