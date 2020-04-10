using System;
using System.Collections.Generic;
using UnityEngine;

using Rewired;
using ResonantSpark.Gameplay;
using UnityEngine.SceneManagement;

namespace ResonantSpark {
    namespace Menu {
        public class MultipleControllers : MonoBehaviour {

            private enum CharacterSelectMode : int {
                ControllerSelect,
                CharacterSelect,
                Confirm,
            }

            public CharacterSelectUiController prefab;
            public RectTransform controllerIconPrefab;

            public FightingGameCharacter lawrencePrefab;

            public RectTransform controllerSelectTransform;

            public RectTransform topController;
            public RectTransform secondController;

            public RectTransform leftPosition;
            public RectTransform rightPosition;

            public GameObject charSelectMenu;
            public GameObject controllerSelectMenu;

            private Dictionary<Player, CharacterSelectUiController> controllers;
            private Dictionary<int, Player> sideToPlayer;
            private List<CharacterSelectUiController> controllerOrder;

            private CharacterSelectMode selectMode = CharacterSelectMode.ControllerSelect;

            public Transform p0CharTransform;
            public Transform p1CharTransform;

            private FightingGameCharacter p0FgCharSelect;
            private FightingGameCharacter p1FgCharSelect;

            public void Awake() {
                controllers = new Dictionary<Player, CharacterSelectUiController>();
                sideToPlayer = new Dictionary<int, Player>();
                controllerOrder = new List<CharacterSelectUiController>();

                //foreach (Controller controller in ReInput.controllers.Controllers) {
                //    CreateControllerSelect(controller);
                //}

                foreach (Player player in ReInput.players.GetPlayers()) {
                    CreateControllerSelect(player);
                }

                //ReInput.ControllerConnectedEvent += OnControllerConnected;
                //ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
                //ReInput.ControllerPreDisconnectEvent += OnControllerPreDisconnect;
            }

            //private void OnControllerConnected(ControllerStatusChangedEventArgs args) {
            //    Debug.Log("A controller was connected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
            //    CreateControllerSelect(args.controller);
            //}

            private void CreateControllerSelect(Player player) {
                RectTransform newIcon = Instantiate<RectTransform>(controllerIconPrefab, controllerSelectTransform);

                CharacterSelectUiController newController = Instantiate<CharacterSelectUiController>(prefab, transform);
                newController.Init(this, newIcon, player, player.id, new Action<int, float>(OnNavigateHorizontal), new Action<int, bool>(OnSubmit), new Action<int, bool>(OnCancel));
                newController.SetControllerSelected(-1);

                controllers.Add(player, newController);
                controllerOrder.Add(newController);
            }

            // This function will be called when a controller is fully disconnected
            // You can get information about the controller that was disconnected via the args parameter
            //private void OnControllerDisconnected(ControllerStatusChangedEventArgs args) {
            //    Debug.Log("A controller was disconnected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);

            //    controllerOrder.Remove(controllers[args.controller]);
            //    controllers.Remove(args.controller);

            //    for (int n = 0; n < controllerOrder.Count; ++n) {
            //        controllerOrder[n].SetDisplayIndex(n);
            //    }
            //}

            // This function will be called when a controller is about to be disconnected
            // You can get information about the controller that is being disconnected via the args parameter
            // You can use this event to save the controller's maps before it's disconnected
            //private void OnControllerPreDisconnect(ControllerStatusChangedEventArgs args) {
            //    Debug.Log("A controller is being disconnected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
            //}

            //public void OnDestroy() {
            //    ReInput.ControllerConnectedEvent -= OnControllerConnected;
            //    ReInput.ControllerDisconnectedEvent -= OnControllerDisconnected;
            //    ReInput.ControllerPreDisconnectEvent -= OnControllerPreDisconnect;
            //}

            public Vector2 SetDisplayIndex(Vector2 currPosition, int index) {
                float offset = secondController.anchoredPosition.y - topController.anchoredPosition.y;
                return new Vector2(currPosition.x, topController.anchoredPosition.y + offset * index);
            }

            public Vector2 SelectController(Vector2 currPosition, int playerId) {
                if (playerId == 0) {
                    return new Vector2(leftPosition.anchoredPosition.x, currPosition.y);
                }
                else if (playerId == 1) {
                    return new Vector2(rightPosition.anchoredPosition.x, currPosition.y);
                }
                else {
                    return new Vector2(topController.anchoredPosition.x, currPosition.y);
                }
            }

            public void EnableCharacterSelect() {
                controllerSelectMenu.SetActive(false);
                charSelectMenu.SetActive(true);
            }

            public void EnableControllerSelect() {
                charSelectMenu.SetActive(false);
                controllerSelectMenu.SetActive(true);
            }

            public void LoadCharacter(int playerIndex) {
                Debug.Log(playerIndex);
                if (playerIndex == 0) {
                    p0FgCharSelect = Instantiate(lawrencePrefab, p0CharTransform.position, p0CharTransform.rotation);
                }
                else if (playerIndex == 1) {
                    p1FgCharSelect = Instantiate(lawrencePrefab, p1CharTransform.position, p1CharTransform.rotation);

                }
            }

            private void OnNavigateHorizontal(int playerIndex, float value) {
                Debug.Log(playerIndex + " On Navigate Horizontal = " + value);

                switch (selectMode) {
                    case CharacterSelectMode.ControllerSelect:
                        controllerOrder[playerIndex].MoveController(value > 0 ? 1 : -1);
                        break;
                    case CharacterSelectMode.CharacterSelect:
                        break;
                }
            }

            private void OnSubmit(int selectedPlayerSide, bool value) {
                Debug.Log(selectedPlayerSide + " On Submit = " + value);

                switch (selectMode) {
                    case CharacterSelectMode.ControllerSelect:
                        for (int n = 0; n < controllerOrder.Count; ++n) {
                            if (controllerOrder[n].GetControllerSelected() != -1) {
                                selectMode = CharacterSelectMode.CharacterSelect;
                                EnableCharacterSelect();
                                for (int m = 0; m < controllerOrder.Count; ++m) {
                                    if (controllerOrder[m].GetControllerSelected() != -1) {
                                        sideToPlayer.Add(m, controllerOrder[m].GetPlayer());
                                    }
                                }
                            }
                        }
                        break;
                    case CharacterSelectMode.CharacterSelect:
                        LoadCharacter(selectedPlayerSide);
                        selectMode = CharacterSelectMode.Confirm;
                        break;
                    case CharacterSelectMode.Confirm:
                        SceneManager.LoadScene("Scenes/Levels/Practice");
                        break;
                }
            }

            private void OnCancel(int selectedPlayerSide, bool value) {
                Debug.Log(selectedPlayerSide + " On Cancel = " + value);
            }
        }
    }
}