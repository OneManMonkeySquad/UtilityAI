using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI {
    public abstract class ActionWithInputsBase : ActionBase {
        [HideInInspector]
        public List<InputScorerBase> scorers;
    }
}