using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI {
    public abstract class Selector : ScriptableObject, ISelectable {
        [HideInInspector]
        public Qualifier defaultQualifier;
        [HideInInspector]
        public List<Qualifier> qualifiers;

        public abstract Qualifier Select(IAIContext context);
    }
}