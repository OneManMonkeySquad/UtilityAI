using UnityEngine;
using UnityEngine.Assertions;

namespace UtilityAI {
    public interface IDebugger {
        void FrameReset();
        void BestQualifier(Qualifier qualifier, Selector parentSelector);
        void ContextualScorer(ContextualScorerBase scorer, float score);
    }

    public class AIDebuggingHook : MonoBehaviour {
        public AI ai;
        public IAIContextProvider contextProvider;

        public static IDebugger currentDebugger;
        public static IAIContextProvider currentDebuggedContextProvider;
        public static IDebugger debugger;
    }


    public class AI {
        Brain _brain;
        public Brain brain {
            get { return _brain; }
        }

        ActionBase _currentAction;
        float _nextUpdateTime;

        public AI(Brain brain) {
            Assert.IsNotNull(brain);
            Assert.IsNotNull(brain.root);

            _brain = brain;
        }

        public void Process(IAIContextProvider contextProvider) {
            var isUpdate = Time.time >= _nextUpdateTime;
            if (isUpdate) {
                contextProvider.UpdateContext();
            }

            var context = contextProvider.GetContext();

            if (isUpdate) {
                _nextUpdateTime = Time.time + brain.settings.updateInterval;

                if (AIDebuggingHook.currentDebuggedContextProvider == contextProvider) {
                    AIDebuggingHook.debugger = AIDebuggingHook.currentDebugger;
                    AIDebuggingHook.debugger.FrameReset();
                }

                var bestQualifier = _brain.root.Select(context);

                AIDebuggingHook.debugger = null;

                if (_currentAction != null && (bestQualifier == null || bestQualifier.action != _currentAction)) {
                    _currentAction.Stop(context);
                }

                if (bestQualifier != null) {
                    _currentAction = bestQualifier.action;
                }
            }

            if (_currentAction != null) {
                _currentAction.Execute(context);
            }
        }
    }
}
