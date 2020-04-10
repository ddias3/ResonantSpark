using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Rewired;

namespace ResonantSpark {
    namespace Menu {
        public class CharacterSelectUiController : MonoBehaviour {
            private MultipleControllers multiControllers;

            private int playerIndex;
            private int displayIndex;

            public RectTransform controllerIcon;

            private Player player;

            private Action<int, float> OnNavigateHorizontalCallback;
            private Action<int, bool> OnSubmitCallback;
            private Action<int, bool> OnCancelCallback;

            private int selectSidePos = 1;

            private int selectedPlayerSide = -1;

            public void Init(MultipleControllers multiControllers, RectTransform controllerIcon, Player player, int index, Action<int, float> navCb, Action<int, bool> subCb, Action<int, bool> canCb) {
                this.multiControllers = multiControllers;
                this.controllerIcon = controllerIcon;
                this.player = player;

                playerIndex = index;

                selectSidePos = 1;

                OnNavigateHorizontalCallback = navCb;
                OnSubmitCallback = subCb;
                OnCancelCallback = canCb;

                player.AddInputEventDelegate(OnNavigateHorizontal, UpdateLoopType.Update, "Navigate Horizontal");
                player.AddInputEventDelegate(OnSubmit, UpdateLoopType.Update, "Submit");
                player.AddInputEventDelegate(OnCancel, UpdateLoopType.Update, "Cancel");

                controllerIcon.anchoredPosition = multiControllers.SetDisplayIndex(controllerIcon.anchoredPosition, index);
            }

            public void SetDisplayIndex(int index) {
                controllerIcon.anchoredPosition = multiControllers.SetDisplayIndex(controllerIcon.anchoredPosition, index);
            }

            public void SetControllerSelected(int index) {
                controllerIcon.anchoredPosition = multiControllers.SelectController(controllerIcon.anchoredPosition, index);
                selectedPlayerSide = index;
            }

            public int GetControllerSelected() {
                return selectedPlayerSide;
            }

            public Player GetPlayer() {
                return player;
            }

            public void MoveController(int offset) {
                selectSidePos += offset;

                if (selectSidePos <= 0) {
                    SetControllerSelected(0);
                }
                else if (selectSidePos == 1) {
                    SetControllerSelected(-1);
                }
                else if (selectSidePos >= 2) {
                    SetControllerSelected(1);
                }
            }

            private void OnNavigateHorizontal(InputActionEventData data) {
                if (data.GetAxis() > 0.5f || data.GetAxis() < -0.5f) {
                    OnNavigateHorizontalCallback(playerIndex, data.GetAxis());
                }
            }

            private void OnSubmit(InputActionEventData data) {
                if (data.GetButtonDown()) OnSubmitCallback(selectedPlayerSide, data.GetButtonDown());
            }

            private void OnCancel(InputActionEventData data) {
                if (data.GetButtonDown()) OnSubmitCallback(selectedPlayerSide, data.GetButtonDown());
            }
        }
    }
}