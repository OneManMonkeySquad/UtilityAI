using UnityEngine;

namespace UtilityAI {
    public abstract class Action<C> : ActionBase where C:IAIContext {
        public abstract void Execute(C context);
        public abstract void Stop(C context);

        public override void Execute(IAIContext context) {
            Execute((C)context);
        }
        public override void Stop(IAIContext context) {
            Stop((C)context);
        }
    }
}