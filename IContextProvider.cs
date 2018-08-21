
namespace Cube.UtilityAI {
    public interface IContext {
    }

    public interface IContextProvider {
        IContext GetContext();
    }
}