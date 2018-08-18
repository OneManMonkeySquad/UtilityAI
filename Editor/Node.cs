using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UtilityAI {
    public class NodeContext {
        public ViewState viewState;
        public Action<Port> OnClickInPoint;
        public Action<Port> OnClickOutPoint;
    }

    public abstract class Node {
        public Rect rect;
        public string title;
        public bool isDragged;

        List<Port> inPorts = new List<Port>();
        List<Port> outPorts = new List<Port>();

        ScriptableObject ownedScriptableObject;

        NodeContext context;

        public Node(float width, float height, string title, NodeContext context, ScriptableObject ownedScriptableObject) {
            rect = new Rect(512, 256, width, height);
            this.context = context;
            this.title = title;
            this.ownedScriptableObject = ownedScriptableObject;

            LoadFromViewState();
        }

        public void Drag(Vector2 delta) {
            rect.position += delta;
            SaveToViewState();
        }

        public void Draw() {
            for (int i = 0; i < inPorts.Count; ++i) {
                var port = inPorts[i];
                port.Draw(i);
            }
            for (int i = 0; i < outPorts.Count; ++i) {
                var port = outPorts[i];
                port.Draw(i);
            }

            GUI.skin.box.fontSize = 20;
            GUI.Box(rect, title);
            GUILayout.BeginArea(new Rect(rect.x, rect.y + 34, rect.width, rect.height - 34));
            DrawContent();
            GUILayout.EndArea();
        }

        protected abstract void DrawContent();

        public bool ProcessEvents(Event e) {
            switch (e.type) {
                case EventType.MouseDown:
                    if (e.button == 0 && rect.Contains(e.mousePosition)) {
                        isDragged = true;
                    }
                    break;

                case EventType.MouseUp:
                    isDragged = false;
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && isDragged) {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
            }

            return false;
        }

        protected Port AddConnectionPoint(ConnectionPointType type, string text) {
            var point = new Port(this, type, text, type == ConnectionPointType.In ? context.OnClickInPoint : context.OnClickOutPoint);
            (type == ConnectionPointType.In ? inPorts : outPorts).Add(point);

            return point;
        }

        void LoadFromViewState() {
            if (context.viewState == null)
                return;

            Vector2 pos;
            if (context.viewState.TryGet(ownedScriptableObject, out pos)) {
                rect.position = pos;
            }
        }

        void SaveToViewState() {
            if (context.viewState == null) {
                context.viewState = new ViewState();
            }
            context.viewState.Set(ownedScriptableObject, rect.position);
        }
    }
}