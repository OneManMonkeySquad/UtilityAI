using UnityEngine;

namespace UtilityAI {
    public abstract class ActionBase : ScriptableObject {
        public abstract void Execute(IAIContext context);
        public abstract void Stop(IAIContext context);
    }
}