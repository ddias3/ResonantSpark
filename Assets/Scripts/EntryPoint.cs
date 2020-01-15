﻿using System.Collections;
using UnityEngine;

using ResonantSpark.Service;
using ResonantSpark.Gamemode;
using ResonantSpark.Input;

namespace ResonantSpark {
    public class EntryPoint : MonoBehaviour {
        // TODO: Come up with a better system than this.
        public OneOnOneRoundBased gamemode;
        public HumanInputController controller;

        public GameObject char0Builder;
        public GameObject char1Builder;

        public PlayerService playerService;
        public FightingGameService fgService;

        public void Start() {
            StartCoroutine(TriggerEndOfFrame());
        }

        private IEnumerator TriggerEndOfFrame() {
            yield return new WaitForEndOfFrame();

            fgService.RegisterGamemode(gamemode);
            playerService.SetNumberHumanPlayers(1);
            playerService.AssociateHumanInput(0, controller);

            playerService.StartCharacterBuild();

            GameObject.FindGameObjectWithTag("rspTime").GetComponent<FrameEnforcer>().StartFrameEnforcer();
        }
    }
}