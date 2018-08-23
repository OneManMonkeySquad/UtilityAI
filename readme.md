# Cube.UtilityAI
Bare-bones utility ai implementation for [Unity3d](https://unity3d.com).

## Screenshots
![Editor](Docs/Editor.png)
![Scoring](Docs/Scoring.png)

## Getting Started
Unzip the repository to your Unity /Assets oder any subfolder. Create a new Brain asset by right clicking in the project explorer, click Create/UnityAI/Brain. Double-click the Brain. The editor will open.

After you have your brain, you need to write some code to connect the AI character with it.

    using UtilityAI;

    // Per agent instance; Shared piece of memory between the AI character and the Brain
    [Serializable]
    public class TestContext : IContext {
        public TestAgent agent;
        // Add things here the Brain needs to know, like a list of known enemies or potential cover positions
        List<Vector3> somePositions;
    }

    // Add this component to the AI character
    public class TestAgent : MonoBehaviour, IContextProvider {
        public Brain brain; // Assign this in the editor; One Brain is a "type" of agent, so shared by multiple agents

        AI ai; // Instance connecting the AI character with its Brain

        void Start() {
            context = new TestContext() {
                agent = this
            };

            ai = new AI(brain);

    #if UNITY_EDITOR
            var debuggerHook = context.agent.gameObject.AddComponent<AIDebuggingHook>();
            debuggerHook.ai = ai;
            debuggerHook.contextProvider = this;
    #endif
        }

        void Update() {
            UpdateContext();

            ai.Process(this);
        }

        void UpdateContext() {
            somePositions.Clear();
            if (Random.Range(0f, 1f) > 0.5f)
                return;

            somePositions.Add(transform.position + Vector3.left);
            somePositions.Add(transform.position + Vector3.right);
        }

        public IContext GetContext() {
            return context;
        }

        public void Jump() {
            transform.position += Vector3.up;
        }

        public void SetPosition(Vector3 pos) {
            transform.position = pos;
        }
    }

    public class JumpAction : Action {
        public override void Execute(IContext context) {
            var c = (TestContext)context;
            c.agent.Jump();
        }

        public override void Stop(IContext context) {
        }
    }

    public class SetPositionAction : ActionWithInputs<Vector3> {
        public override void Execute(IContext context) {
            var c = (TestContext)context;

            var pos = GetBest(c, c.somePositions); // Evaluate the best input using the InputScorers attached to the Action
            c.agent.SetPosition(pos);
        }

        public override void Stop(IContext context) {
        }
    }

    public class HasSomePositions : ContextualScorer {
        protected override float RawScore(IContext context) {
            var c = (TestContext)context;

            return c.somePositions.Count > 0 ? 1 : 0;
        }
    }

    public class RandomInput : InputScorer<Vector3> {
        public override float Score(IContext context, Vector3 position) {
            return Random.Range(0f, 1f);
        }
    }

### Prerequisites
Recent Unity version (2018), not testen on older versions.

## Versioning
We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [releases](https://github.com/SirPolly/UtilityAI/releases).

## License
This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
