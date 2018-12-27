
namespace UtilityAI {
    public interface IAIContext {
    }

    public interface IAIContextProvider {
        void UpdateContext();
        IAIContext GetContext();
    }
}