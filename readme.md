# Project Title
Bare-bonesutility ai implementation for [Unity3d](https://unity3d.com).

## Getting Started
Unzip the repository to your Unity Assets oder any subfolder. Create a new Brain asset by right clicking into the project explorer, click Create/Cube.UnityAI/Brain. Double-click the Brain. The editor for the brain will open. Read the node descriptions below on what everything does.

    using UtilityAI;

    [Serializable]
    public class TestContext : IContext {
        public TestAgent agent;
        // Add things here the Brain needs to know, like a list of known enemies or potential cover positions
        List<Vector3> somePositions;
    }

    // Add this component to the AI character
    public class TestAgent : MonoBehaviour, IContextProvider {
        public Cube.UtilityAI.Brain brain; // Assign this in the editor

        Cube.UtilityAI.AI ai;

        void Start() {
            context = new TestContext() {
                agent = this
            };

            ai = new AI(brain);

    #if UNITY_EDITOR
            var debuggerHook = context.aiAgent.gameObject.AddComponent<Cube.UtilityAI.AIDebuggingHook>();
            debuggerHook.ai = _ai;
            debuggerHook.contextProvider = this;
    #endif
        }

        void Update() {
            UpdateContext();

            ai.Process(this);
        }

        void UpdateContext() {
            somePositions.Clear();
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

### Prerequisites
Recent Unity version (2018), not testen on older versions.

## Versioning
We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://bitbucket.org/unique-code/cube.utilityai/downloads/?tab=tags).

## Authors
* **Oliver Weitzel (SirPolly)** - *Everything*

See also the list of [contributors](https://bitbucket.org/unique-code/cube.utilityai/addon/bitbucket-graphs/graphs-repo-page#!graph=contributors) who participated in this project.

## License
This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Editor Screenshots
![Editor](Docs/Editor.png)
![Scoring](Docs/Scoring.png)