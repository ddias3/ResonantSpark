using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ResonantSpark.Service;
using ResonantSpark.Utility;
using ResonantSpark.Gameplay;

namespace ResonantSpark {
    namespace Gamemode {
        public class TrainingMode : OneOnOneRoundBased {
            public GameObject menuPrefab;

            private FightingGameCharacter player;
            private TrainingDummy dummy;

            public override void SetUp(PlayerService playerService, FightingGameService fgService, UiService uiService) {
                base.SetUp(playerService, fgService, uiService);

                Persistence persistence = Persistence.Get();

                Menu.PauseMenu trainingMenus = GameObject.Instantiate(menuPrefab).GetComponent<Menu.PauseMenu>();

                trainingMenus.Init(persistence.GetHumanPlayers(), persistence.CreatePlayerDeviceMap());
                HookUpMenu(trainingMenus.GetHooks());
            }

            private void HookUpMenu(Dictionary<string, UnityEventBase> hooks) {
                HookReceive hookReceive = new HookReceive(hooks);

                hookReceive.HookIn<bool>("pause", new UnityAction<bool>(PauseGame));
                //hookReceive.HookIn<string>("dummyBlock", new UnityAction<string>(dummy.SetBlockMode));
            }
        }
    }
}