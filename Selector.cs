using System.Collections.Generic;
using UnityEngine;

namespace Cube.UtilityAI {
    public abstract class Selector : ScriptableObject, ISelectable {
        public Qualifier defaultQualifier;
        public List<Qualifier> qualifiers;

        public abstract Qualifier Select(IContext context);
    }
}