using UnityEngine;
using UnityEngine.Assertions;

namespace Cube.UtilityAI {
    public interface IDebugger {
        void FrameReset();
        void BestQualifier(Qualifier qualifier, Selector parentSelector);
        void ContextualScorer(ContextualScorer scorer, float score);
    }

    public class AIDebuggingHook : MonoBehaviour {
        public AI ai;
        public IContextProvider contextProvider;

        public static IDebugger currentDebugger;
        public static IContextProvider currentDebuggedContextProvider;
        public static IDebugger debugger;
    }


    public class AI {
        Brain _brain;
        public Brain brain {
            get { return _brain; }
        }

        Action _currentAction;
        float _nextUpdateTime;

        public AI(Brain brain) {
            Assert.IsNotNull(brain);
            Assert.IsNotNull(brain.root);

            _brain = brain;
        }

        public void Process(IContextProvider contextProvider) {
            var context = contextProvider.GetContext();

            if (Time.time >= _nextUpdateTime) {
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
