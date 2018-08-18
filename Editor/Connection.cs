using System;
using UnityEditor;
using UnityEngine;

namespace UtilityAI {
    public class Connection {
        public Port inPoint;
        public Port outPoint;
        public Action<Connection> OnClickRemoveConnection;

        public Connection(Port inPoint, Port outPoint, Action<Connection> OnClickRemoveConnection) {
            this.inPoint = inPoint;
            this.outPoint = outPoint;
            this.OnClickRemoveConnection = OnClickRemoveConnection;
        }

        public void Draw() {
            Handles.DrawBezier(
                inPoint.connectionPoint,
                outPoint.connectionPoint,
                inPoint.connectionPoint + Vector2.left * 50f,
                outPoint.connectionPoint - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap)) {
                if (OnClickRemoveConnection != null) {
                    OnClickRemoveConnection(this);
                }
            }
        }
    }
}