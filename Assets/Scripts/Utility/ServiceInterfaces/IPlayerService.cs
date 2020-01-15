using UnityEngine;

using ResonantSpark.Gameplay;
using ResonantSpark.Input;
using ResonantSpark.Builder;

namespace ResonantSpark {
    namespace Service {
        public interface IPlayerService {
            void SetMaxPlayers(int maxTotalPlayers);
            void SetNumberHumanPlayers(int numHumanPlayers);
            void AssociateHumanInput(int playerIndex, HumanInputController humanController);
            void StartCharacterBuild();
            void SetCharacterSelected(int playerId, ICharacterBuilder charSelected);
        }
    }
}