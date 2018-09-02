using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UtilityAI.Editor {
#if UNITY_EDITOR
    public class AIDebugger : EditorWindow, IDebugger {
        Vector2 _scroll;

        [MenuItem("Window/Analysis/UtilityAI Debugger")]
        public static void ShowWindow() {
            var window = GetWindow(typeof(AIDebugger));
            window.titleContent = new GUIContent("UtilityAI Debugger");
        }

        void OnGUI() {
            if (Selection.gameObjects.Length != 1) {
                EditorGUILayout.LabelField(new GUIContent("Multi selection not supported"));
                return;
            }

            var hook = Selection.gameObjects[0].GetComponentInParent<AIDebuggingHook>();
            if (hook == null) {
                EditorGUILayout.LabelField(new GUIContent("AIDebuggingHook Component missing"));
                return;
            }

            var ai = hook.ai;

            AIDebuggingHook.currentDebuggedContextProvider = hook.contextProvider;
            AIDebuggingHook.currentDebugger = this;

            _scroll = EditorGUILayout.BeginScrollView(_scroll, false, true);
            DrawSelector(ai.brain.root);
            EditorGUILayout.EndScrollView();
        }
        
        Dictionary<ContextualScorer, float> _foo2 = new Dictionary<ContextualScorer, float>();
        Dictionary<Selector, Qualifier> _bestQualifier = new Dictionary<Selector, Qualifier>();

        void DrawSelector(Selector selector) {
            EditorGUILayout.LabelField(selector.GetType().Name);

            ++EditorGUI.indentLevel;
            foreach (var qualifier in selector.qualifiers) {
                DrawQualifier(qualifier, selector);
            }
            --EditorGUI.indentLevel;
        }

        void DrawQualifier(Qualifier qualifier, Selector parentSelector) {
            var name = qualifier.GetType().Name;

            if (qualifier.action != null) {
                name = qualifier.action.GetType().Name + " : " + name;
            }

            var style = EditorStyles.label;

            Qualifier bestQualifier;
            if (_bestQualifier.TryGetValue(parentSelector, out bestQualifier)) {
                if (qualifier == bestQualifier) {
                    style = EditorStyles.boldLabel;
                }
            }

            EditorGUILayout.LabelField(name, style);

            ++EditorGUI.indentLevel;
            
            foreach (var scorer in qualifier.scorers) {
                DrawContextualScorer(scorer);
            }
            
            if (qualifier.selector != null) {
                DrawSelector(qualifier.selector);
            }

            --EditorGUI.indentLevel;
        }

        void DrawContextualScorer(ContextualScorer scorer) {
            var name = scorer.GetType().Name;

            float score;
            if (_foo2.TryGetValue(scorer, out score)) {
                name += string.Format(" {0:F2}", score);
            }

            EditorGUILayout.LabelField(name);
        }

        void Update() {
            Repaint();
        }

        public void FrameReset() {
            _foo2.Clear();
            _bestQualifier.Clear();
        }
        
        public void ContextualScorer(ContextualScorer scorer, float score) {
            _foo2[scorer] = score;
        }

        public void BestQualifier(Qualifier qualifier, Selector parentSelector) {
            _bestQualifier[parentSelector] = qualifier;
        }
    }
#endif
}