using System;
using UnityEngine;

namespace UtilityAI.Editor {
    public enum PortType {
        In,
        Out
    }

    public class Port {
        public Rect rect;

        public PortType type;

        public Node node;
        
        public Action<Port> OnClickConnectionPoint;
        public Func<Port, bool> AcceptConnect;
        public Action<Port> OnDisconnect;

        public Vector2 connectionPoint {
            get { return rect.center + new Vector2(type == PortType.In ? rect.width * -0.5f : rect.width * 0.5f, 0); }
        }

        public string text;

        public Port(Node node, PortType type, string text, Action<Port> OnClickConnectionPoint) {
            this.node = node;
            this.type = type;
            this.text = text;
            this.OnClickConnectionPoint = OnClickConnectionPoint;
            rect = new Rect(0, 0, 120, 20);
        }
        
        public void Draw(ref float height) {
            rect.y = node.rect.y + height;
            height += rect.height - 1;

            switch (type) {
                case PortType.In:
                    rect.x = node.rect.x;
                    break;

                case PortType.Out:
                    rect.x = node.rect.x + node.rect.width - rect.width;
                    break;
            }

            if (GUI.Button(rect, text, GUI.skin.box)) {
                if (OnClickConnectionPoint != null) {
                    OnClickConnectionPoint(this);
                }
            }
        }
    }
}