using UnityEngine;

namespace Cube.UtilityAI {
    public abstract class Action : ScriptableObject {
        public abstract void Execute(IContext context);
        public abstract void Stop(IContext context);
    }
}