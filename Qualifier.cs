using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI {
    public abstract class Qualifier : ScriptableObject, ISelectable {
        public Action action;
        public Selector selector;
        public List<ContextualScorer> scorers;

        public Qualifier Select(IContext context) {
            if (action != null)
                return this;
            else
                return selector.Select(context);
        }

        public abstract float Score(IContext context);
    }
}