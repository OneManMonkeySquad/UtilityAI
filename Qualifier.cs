using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI {
    public abstract class Qualifier : ScriptableObject, ISelectable {
        [HideInInspector]
        public ActionBase action;
        [HideInInspector]
        public Selector selector;
        [HideInInspector]
        public List<ContextualScorerBase> scorers;

        public Qualifier Select(IAIContext context) {
            if (action != null)
                return this;
            else
                return selector.Select(context);
        }

        public abstract float Score(IAIContext context);
    }
}