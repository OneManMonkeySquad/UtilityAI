using System;
using UnityEngine;

namespace UtilityAI {
    public enum ConnectionPointType { In, Out }

    public class Port {
        public Rect rect;

        public ConnectionPointType type;

        public Node node;
        
        public Action<Port> OnClickConnectionPoint;
        public Func<Port, bool> AcceptConnect;

        public Vector2 connectionPoint {
            get { return rect.center + new Vector2(type == ConnectionPointType.In ? rect.width * -0.5f : rect.width * 0.5f, 0); }
        }

        public string text;

        public Port(Node node, ConnectionPointType type, string text, Action<Port> OnClickConnectionPoint) {
            this.node = node;
            this.type = type;
            this.text = text;
            this.OnClickConnectionPoint = OnClickConnectionPoint;
            rect = new Rect(0, 0, 50, 20);
        }
        
        public void Draw(int idx) {
            rect.y = node.rect.y + idx * rect.height;

            switch (type) {
                case ConnectionPointType.In:
                    rect.x = node.rect.x - rect.width;
                    break;

                case ConnectionPointType.Out:
                    rect.x = node.rect.x + node.rect.width;
                    break;
            }

            if (GUI.Button(rect, text)) {
                if (OnClickConnectionPoint != null) {
                    OnClickConnectionPoint(this);
                }
            }
        }
    }
}