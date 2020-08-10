namespace ResonantSpark {
    namespace Gameplay {
        public interface IPredeterminedActions {
            void PredeterminedActions(string actionName);
            void PredeterminedActions(string actionName, params object[] objs);
        }
    }
}