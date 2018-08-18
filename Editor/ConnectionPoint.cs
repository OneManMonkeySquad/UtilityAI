using System;
using UnityEngine;

namespace UtilityAI {
    public enum ConnectionPointType { In, Out }

    public class ConnectionPoint {
        public Rect rect;

        public ConnectionPointType type;

        public Node node;
        
        public Action<ConnectionPoint> OnClickConnectionPoint;
        public Func<ConnectionPoint, bool> AcceptConnect;

        public string text;

        public ConnectionPoint(Node node, ConnectionPointType type, string text, Action<ConnectionPoint> OnClickConnectionPoint) {
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