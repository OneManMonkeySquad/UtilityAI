using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace UtilityAI.Editor {
    public class BrainEditor : EditorWindow {
        [SerializeField]
        Brain brain;

        List<Node> nodes = new List<Node>();
        List<Connection> connections = new List<Connection>();

        BrainNode brainNode;

        NodeContext context;

        Port selectedInPoint;
        Port selectedOutPoint;

        const float kZoomMin = 0.2f;
        const float kZoomMax = 1;

        Rect _zoomArea = new Rect(0, 0, 1, 1);
        float _zoom = 1.0f;

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line) {
            if (Selection.activeObject as Brain != null) {
                OpenWindow();
                return true;
            }
            return false;
        }

        [MenuItem("Window/AI/UtilityAI Editor")]
        static void OpenWindow() {
            var window = GetWindow<BrainEditor>();
            window.titleContent = new GUIContent("UtilityAI Editor");
        }

        void OnEnable() {
            context = new NodeContext {
                OnClickInPoint = OnClickInPoint,
                OnClickOutPoint = OnClickOutPoint
            };

            if (brain == null) {
                brain = Selection.activeObject as Brain;
            }

            if (brain != null) {
                context.viewState = brain.viewState;
                Rebuild(brain);
            }
        }

        void OnGUI() {
            var newBrain = Selection.activeObject as Brain;
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
                DrawConnections();
                DrawNodes();
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

        void Rebuild(Brain brain) {
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
                        if (qualifier.selector == sn.selector) {
                            var c = new Connection(sn.selectorOut, qn.actionOrSelectorIn, OnClickRemoveConnection);
                            connections.Add(c);
                        }

                        if (sn.selector.qualifiers != null && sn.selector.qualifiers.Contains(qualifier)) {
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
                            var c = new Connection(qn.actionOrSelectorIn, an.actionOut, OnClickRemoveConnection);
                            connections.Add(c);
                        }
                    }

                    continue;
                }

                var action = asset as ActionBase;
                if (action != null) {
                    var an = new ActionNode(action, context);
                    nodes.Add(an);

                    foreach (var qn in qualifierNodes) {
                        if (qn.qualifier.action == action) {
                            var c = new Connection(qn.actionOrSelectorIn, an.actionOut, OnClickRemoveConnection);
                            connections.Add(c);
                        }
                    }
                }
            }

            foreach (var asset in assets) {
                var scorer = asset as ContextualScorerBase;
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
                        var action = an.actionWithInputs as ActionWithInputsBase;
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
                    selectedInPoint.connectionPoint,
                    e.mousePosition,
                    selectedInPoint.connectionPoint + Vector2.left * 50f,
                    e.mousePosition - Vector2.left * 50f,
                    Color.black,
                    null,
                    3
                );

                GUI.changed = true;
            }

            if (selectedOutPoint != null && selectedInPoint == null) {
                Handles.DrawBezier(
                    selectedOutPoint.connectionPoint,
                    e.mousePosition,
                    selectedOutPoint.connectionPoint - Vector2.left * 50f,
                    e.mousePosition + Vector2.left * 50f,
                    Color.black,
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
            switch (e.type) {
                case EventType.MouseDown:
                    if (e.button == 1) {
                        if (selectedInPoint != null || selectedOutPoint != null) {
                            ClearConnectionSelection();
                        }
                        else {
                            ProcessContextMenu(e.mousePosition);
                        }
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0) {
                        OnDrag(e.delta);
                    }
                    break;

                case EventType.ScrollWheel:
                    Vector2 zoomCoordsMousePos = ConvertScreenCoordsToZoomCoords(Event.current.mousePosition);
                    var delta = Event.current.delta;
                    float zoomDelta = -delta.y / 60.0f;
                    float oldZoom = _zoom;
                    _zoom += zoomDelta;
                    _zoom = Mathf.Clamp(_zoom, kZoomMin, kZoomMax);

                    var offset = zoomCoordsMousePos - (oldZoom / _zoom) * zoomCoordsMousePos;
                    OnDrag(-offset);

                    Event.current.Use();
                    break;
            }
        }

        void OnDrag(Vector2 delta) {
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
               where type.IsSubclassOf(typeof(Selector)) && !type.IsAbstract
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
               where type.IsSubclassOf(typeof(Qualifier)) && !type.IsAbstract
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
               where type.IsSubclassOf(typeof(ActionBase)) && !type.IsAbstract
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
               where type.IsSubclassOf(typeof(ContextualScorerBase)) && !type.IsAbstract
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
            var action = (ActionBase)Activator.CreateInstance(type);
            action.name = type.FullName;

            AssetDatabase.AddObjectToAsset(action, brain);

            var actionWithInputs = action as ActionWithInputsBase;
            if (actionWithInputs == null) {
                var node = new ActionNode(action, context);
                nodes.Add(node);
            }
            else {
                var node = new ActionWithInputsNode(actionWithInputs, context);
                nodes.Add(node);
            }

            GUI.changed = true;
        }

        void OnClickAddContextualScorer(Vector2 mousePosition, Type type) {
            var scorer = (ContextualScorerBase)Activator.CreateInstance(type);
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

        void OnClickInPoint(Port inPoint) {
            selectedInPoint = inPoint;

            if (selectedOutPoint != null) {
                if (selectedOutPoint.node != selectedInPoint.node) {
                    CreateConnection();
                    ClearConnectionSelection();
                }
                else {
                    ClearConnectionSelection();
                }
            }
        }

        void OnClickOutPoint(Port outPoint) {
            selectedOutPoint = outPoint;

            if (selectedInPoint != null) {
                if (selectedOutPoint.node != selectedInPoint.node) {
                    CreateConnection();
                    ClearConnectionSelection();
                }
                else {
                    ClearConnectionSelection();
                }
            }
        }

        void OnClickRemoveConnection(Connection connection) {
            connections.Remove(connection);

            if (connection.inPoint.OnDisconnect != null) {
                connection.inPoint.OnDisconnect.Invoke(connection.outPoint);
            }
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

        Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords) {
            return screenCoords / _zoom;
        }
    }
}