﻿using UnityEditor;
using UnityEngine;

namespace UtilityAI {
    public class InputScorerNode : Node {
        public readonly InputScorerBase scorer;

        public ConnectionPoint scorerOut;

        public InputScorerNode(InputScorerBase scorer, NodeContext context)
          : base(400, 100, scorer.GetType().FullName, context, scorer) {
            this.scorer = scorer;

            var scorerType = scorer.GetType().BaseType;
            while (scorerType != null) {
                if (scorerType.GetGenericTypeDefinition() == typeof(InputScorer<>)) {
                    title += string.Format(" <{0}>", scorerType.GenericTypeArguments[0].Name);
                    break;
                }
                scorerType = scorerType.BaseType;
            }

            scorerOut = AddConnectionPoint(ConnectionPointType.Out, "Input Scorer");
        }

        protected override void DrawContent() {
            var inspector = Editor.CreateEditor(scorer);
            inspector.DrawDefaultInspector();
        }
    }
}