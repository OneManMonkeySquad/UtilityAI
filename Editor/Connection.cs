using System;
using UnityEditor;
using UnityEngine;

namespace UtilityAI.Editor {
#if UNITY_EDITOR
    public class Connection {
        public Port inPoint;
        public Port outPoint;
        public System.Action<Connection> OnClickRemoveConnection;

        public Connection(Port inPoint, Port outPoint, System.Action<Connection> OnClickRemoveConnection) {
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
                Color.black,
                null,
                3
            );

            Handles.color = Color.black;
            if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap)) {
                if (OnClickRemoveConnection != null) {
                    OnClickRemoveConnection(this);
                }
            }
        }
    }
#endif
}