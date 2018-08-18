﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UtilityAI {
    public class AIBrainEditor : EditorWindow {
        AIBrain brain;

        List<Node> nodes = new List<Node>();
        List<Connection> connections = new List<Connection>();

        BrainNode brainNode;

        NodeContext context;

        ConnectionPoint selectedInPoint;
        ConnectionPoint selectedOutPoint;

        Vector2 offset;
        Vector2 drag;

        const float kZoomMin = 0.1f;
        const float kZoomMax = 8.0f;

        Rect _zoomArea = new Rect(0, 0, 1, 1);
        float _zoom = 1.0f;

        [MenuItem("Window/Cube/UtilityAI Editor")]
        [MenuItem("Cube/Window/UtilityAI Editor")]
        static void OpenWindow() {
            var window = GetWindow<AIBrainEditor>();
            window.titleContent = new GUIContent("UtilityAI Editor");
        }

        Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords) {
            return screenCoords / _zoom;
        }

        void OnEnable() {
            context = new NodeContext {
                OnClickInPoint = OnClickInPoint,
                OnClickOutPoint = OnClickOutPoint
            };

            brain = Selection.activeObject as AIBrain;
            if (brain != null) {
                context.viewState = brain.viewState;
                Rebuild(brain);
            }
        }

        void OnGUI() {
            var newBrain = Selection.activeObject as AIBrain;
            if (newBrain != null && newBrain != brain) {
                brain = newBrain;
                context.viewState = brain.viewState;
                Rebuild(newBrain);
            }

            if (brain == null) {
                EditorGUILayout.LabelField(new GUIContent("Select AIBrain asset"));
                return;
            }

            _zoomArea.width = position.width;
            _zoomArea.height = position.height;
            EditorZoomArea.Begin(_zoom, _zoomArea);
            {
                DrawNodes();
                DrawConnections();
                DrawConnectionLine(Event.current);
            }
            EditorZoomArea.End();

            DrawToolbar();

            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);

            if (GUI.changed) {
                Repaint();
            }
        }

        void Rebuild(AIBrain brain) {
            Clear();

            brainNode = new BrainNode(brain, context);
            nodes.Add(brainNode);

            var selectorNodes = new List<SelectorNode>();
            var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(brain));
            foreach (var asset in assets) {
                var selector = asset as Selector;
                if (selector != null) {
                    var sn = new SelectorNode(selector, context);
                    nodes.Add(sn);
                    selectorNodes.Add(sn);

                    if (brain.root == selector) {
                        var c = new Connection(brainNode.rootIn, sn.selectorOut, OnClickRemoveConnection);
                        connections.Add(c);
                    }
                }
            }

            var qualifierNodes = new List<QualifierNode>();
            foreach (var asset in assets) {
                var qualifier = asset as Qualifier;
                if (qualifier != null) {
                    var qn = new QualifierNode(qualifier, context);
                    nodes.Add(qn);
                    qualifierNodes.Add(qn);

                    foreach (var sn in selectorNodes) {
                        if (sn.selector.qualifiers.Contains(qualifier)) {
                            var c = new Connection(sn.qualifiersIn, qn.qualifierOut, OnClickRemoveConnection);
                            connections.Add(c);
                        }
                    }
                }
            }

            var actionNodes = new List<ActionWithInputsNode>();
            foreach (var asset in assets) {
                var actionWithInputs = asset as ActionWithInputsBase;
                if (actionWithInputs != null) {
                    var an = new ActionWithInputsNode(actionWithInputs, context);
                    nodes.Add(an);
                    actionNodes.Add(an);

                    foreach (var qn in qualifierNodes) {
                        if (qn.qualifier.action == actionWithInputs) {
                            var c = new Connection(qn.actionIn, an.actionOut, OnClickRemoveConnection);
                            connections.Add(c);
                        }
                    }

                    continue;
                }

                var action = asset as Action;
                if (action != null) {
                    var an = new ActionNode(action, context);
                    nodes.Add(an);

                    foreach (var qn in qualifierNodes) {
                        if (qn.qualifier.action == action) {
                            var c = new Connection(qn.actionIn, an.actionOut, OnClickRemoveConnection);
                            connections.Add(c);
                        }
                    }
                }
            }

            foreach (var asset in assets) {
                var scorer = asset as ContextualScorer;
                if (scorer != null) {
                    var an = new ContextualScorerNode(scorer, context);
                    nodes.Add(an);

                    foreach (var qn in qualifierNodes) {
                        if (qn.qualifier.scorers.Contains(scorer)) {
                            var c = new Connection(qn.scorersIn, an.scorerOut, OnClickRemoveConnection);
                            connections.Add(c);
                        }
                    }
                }
            }

            foreach (var asset in assets) {
                var scorer = asset as InputScorerBase;
                if (scorer != null) {
                    var sn = new InputScorerNode(scorer, context);
                    nodes.Add(sn);

                    foreach (var an in actionNodes) {
                        var action = an.action as ActionWithInputsBase;
                        if (action != null && action.scorers.Contains(scorer)) {
                            var c = new Connection(an.inputScorersIn, sn.scorerOut, OnClickRemoveConnection);
                            connections.Add(c);
                        }
                    }
                }
            }
        }

        void Clear() {
            brainNode = null;
            nodes.Clear();
            connections.Clear();
        }

        void DrawNodes() {
            for (int i = 0; i < nodes.Count; i++) {
                nodes[i].Draw();
            }
        }

        void DrawConnections() {
            for (int i = 0; i < connections.Count; i++) {
                connections[i].Draw();
            }
        }

        void DrawConnectionLine(Event e) {
            if (selectedInPoint != null && selectedOutPoint == null) {
                Handles.DrawBezier(
                    selectedInPoint.rect.center,
                    e.mousePosition,
                    selectedInPoint.rect.center + Vector2.left * 50f,
                    e.mousePosition - Vector2.left * 50f,
                    Color.white,
                    null,
                    2f
                );

                GUI.changed = true;
            }

            if (selectedOutPoint != null && selectedInPoint == null) {
                Handles.DrawBezier(
                    selectedOutPoint.rect.center,
                    e.mousePosition,
                    selectedOutPoint.rect.center - Vector2.left * 50f,
                    e.mousePosition + Vector2.left * 50f,
                    Color.white,
                    null,
                    2f
                );

                GUI.changed = true;
            }
        }

        void DrawToolbar() {
            EditorGUILayout.BeginHorizontal("Toolbar", GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Save", "ToolbarButton", GUILayout.Width(50))) {
                AssetDatabase.SaveAssets();
            }
            if (GUILayout.Button("Reload", "ToolbarButton", GUILayout.Width(50))) {
                Rebuild(brain);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        void ProcessEvents(Event e) {
            drag = Vector2.zero;

            switch (e.type) {
                case EventType.MouseDown:
                    if (e.button == 1) {
                        ProcessContextMenu(e.mousePosition);
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0) {
                        OnDrag(e.delta);
                    }
                    break;

                case EventType.ScrollWheel:
                    var delta = Event.current.delta;
                    float zoomDelta = -delta.y / 100.0f;
                    _zoom += zoomDelta;
                    _zoom = Mathf.Clamp(_zoom, kZoomMin, kZoomMax);

                    Event.current.Use();
                    break;
            }
        }

        void OnDrag(Vector2 delta) {
            drag = delta;

            for (int i = 0; i < nodes.Count; i++) {
                nodes[i].Drag(delta);
            }

            GUI.changed = true;
        }

        void ProcessNodeEvents(Event e) {
            var prevMousePos = Event.current.mousePosition;
            Event.current.mousePosition = ConvertScreenCoordsToZoomCoords(Event.current.mousePosition);

            if (nodes != null) {
                for (int i = nodes.Count - 1; i >= 0; i--) {
                    bool guiChanged = nodes[i].ProcessEvents(e);

                    if (guiChanged) {
                        GUI.changed = true;
                    }
                }
            }

            Event.current.mousePosition = prevMousePos;
        }

        void ProcessContextMenu(Vector2 mousePosition) {
            var menu = new GenericMenu();
            AddSelectorContextMenu(menu, mousePosition);
            AddQualifierContextMenu(menu, mousePosition);
            AddActionContextMenu(menu, mousePosition);
            AddContextualScorerContextMenu(menu, mousePosition);
            AddInputScorerContextMenu(menu, mousePosition);
            menu.ShowAsContext();
        }

        void AddSelectorContextMenu(GenericMenu menu, Vector2 mousePosition) {
            var classes = (
               from assembly in AppDomain.CurrentDomain.GetAssemblies()
               from type in assembly.GetTypes()
               where type.IsSubclassOf(typeof(Selector))
               select type
           ).ToList();

            foreach (var cls in classes) {
                menu.AddItem(new GUIContent("Selector/" + cls.FullName), false, () => OnClickAddSelector(mousePosition, cls));
            }
        }

        void AddQualifierContextMenu(GenericMenu menu, Vector2 mousePosition) {
            var classes = (
               from assembly in AppDomain.CurrentDomain.GetAssemblies()
               from type in assembly.GetTypes()
               where type.IsSubclassOf(typeof(Qualifier))
               select type
           ).ToList();

            foreach (var cls in classes) {
                menu.AddItem(new GUIContent("Qualifiers/" + cls.FullName), false, () => OnClickAddQualifier(mousePosition, cls));
            }
        }

        void AddActionContextMenu(GenericMenu menu, Vector2 mousePosition) {
            var classes = (
               from assembly in AppDomain.CurrentDomain.GetAssemblies()
               from type in assembly.GetTypes()
               where type.IsSubclassOf(typeof(Action)) && !type.IsAbstract
               select type
           ).ToList();

            foreach (var cls in classes) {
                menu.AddItem(new GUIContent("Actions/" + cls.FullName), false, () => OnClickAddAction(mousePosition, cls));
            }
        }

        void AddContextualScorerContextMenu(GenericMenu menu, Vector2 mousePosition) {
            var classes = (
               from assembly in AppDomain.CurrentDomain.GetAssemblies()
               from type in assembly.GetTypes()
               where type.IsSubclassOf(typeof(ContextualScorer))
               select type
           ).ToList();

            foreach (var cls in classes) {
                menu.AddItem(new GUIContent("ContextualScorers/" + cls.FullName), false, () => OnClickAddContextualScorer(mousePosition, cls));
            }
        }

        void AddInputScorerContextMenu(GenericMenu menu, Vector2 mousePosition) {
            var classes = (
               from assembly in AppDomain.CurrentDomain.GetAssemblies()
               from type in assembly.GetTypes()
               where type.IsSubclassOf(typeof(InputScorerBase)) && !type.IsAbstract
               select type
           ).ToList();

            foreach (var cls in classes) {
                menu.AddItem(new GUIContent("InputScorers/" + cls.FullName), false, () => OnClickAddInputScorer(mousePosition, cls));
            }
        }

        void OnClickAddSelector(Vector2 mousePosition, Type type) {
            var selector = (Selector)Activator.CreateInstance(type);
            selector.name = type.FullName;

            AssetDatabase.AddObjectToAsset(selector, brain);

            var node = new SelectorNode(selector, context);
            nodes.Add(node);

            GUI.changed = true;
        }

        void OnClickAddQualifier(Vector2 mousePosition, Type type) {
            var qualifier = (Qualifier)Activator.CreateInstance(type);
            qualifier.name = type.FullName;

            AssetDatabase.AddObjectToAsset(qualifier, brain);

            var node = new QualifierNode(qualifier, context);
            nodes.Add(node);

            GUI.changed = true;
        }

        void OnClickAddAction(Vector2 mousePosition, Type type) {
            var action = (Action)Activator.CreateInstance(type);
            action.name = type.FullName;

            AssetDatabase.AddObjectToAsset(action, brain);

            var actionWithInputs = action as ActionWithInputsBase;
            if (actionWithInputs == null) {
                var node = new ActionNode(action, context);
                nodes.Add(node);
            } else {
                var node = new ActionWithInputsNode(actionWithInputs, context);
                nodes.Add(node);
            }

            GUI.changed = true;
        }

        void OnClickAddContextualScorer(Vector2 mousePosition, Type type) {
            var scorer = (ContextualScorer)Activator.CreateInstance(type);
            scorer.name = type.FullName;

            AssetDatabase.AddObjectToAsset(scorer, brain);

            var node = new ContextualScorerNode(scorer, context);
            nodes.Add(node);

            GUI.changed = true;
        }

        void OnClickAddInputScorer(Vector2 mousePosition, Type type) {
            var scorer = (InputScorerBase)Activator.CreateInstance(type);
            scorer.name = type.FullName;

            AssetDatabase.AddObjectToAsset(scorer, brain);

            var node = new InputScorerNode(scorer, context);
            nodes.Add(node);

            GUI.changed = true;
        }

        void OnClickInPoint(ConnectionPoint inPoint) {
            selectedInPoint = inPoint;

            if (selectedOutPoint != null) {
                if (selectedOutPoint.node != selectedInPoint.node) {
                    CreateConnection();
                    ClearConnectionSelection();
                } else {
                    ClearConnectionSelection();
                }
            }
        }

        void OnClickOutPoint(ConnectionPoint outPoint) {
            selectedOutPoint = outPoint;

            if (selectedInPoint != null) {
                if (selectedOutPoint.node != selectedInPoint.node) {
                    CreateConnection();
                    ClearConnectionSelection();
                } else {
                    ClearConnectionSelection();
                }
            }
        }

        void OnClickRemoveConnection(Connection connection) {
            connections.Remove(connection);
        }

        void CreateConnection() {
            if (selectedInPoint.AcceptConnect != null) {
                if (!selectedInPoint.AcceptConnect.Invoke(selectedOutPoint))
                    return;
            }

            connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
        }

        void ClearConnectionSelection() {
            selectedInPoint = null;
            selectedOutPoint = null;
        }
    }
}