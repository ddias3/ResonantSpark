using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

namespace ResonantSpark {
    namespace Multiplayer {
        public class ControllerConnectHandler : MonoBehaviour {

            public void Awake() {
                ReInput.ControllerConnectedEvent += OnControllerConnected;
                ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
                ReInput.ControllerPreDisconnectEvent += OnControllerPreDisconnect;

                foreach (Joystick joystick in ReInput.controllers.Joysticks) {
                    if (!ReInput.controllers.IsJoystickAssigned(joystick)) {
                        AssignJoystickToNextOpenPlayer(joystick);
                    }
                }
            }

            public void OnControllerConnected(ControllerStatusChangedEventArgs args) {
                if (args.controllerType != ControllerType.Joystick) {
                    return;
                }

                AssignJoystickToNextOpenPlayer(ReInput.controllers.GetJoystick(args.controllerId));
            }

            public void OnControllerDisconnected(ControllerStatusChangedEventArgs args) {
                Debug.Log("A controller was disconnected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
            }

            public void OnControllerPreDisconnect(ControllerStatusChangedEventArgs args) {
                // TODO: Save the controller map.
                Debug.Log("A controller is being disconnected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
            }

            private void AssignJoystickToNextOpenPlayer(Joystick joystick) {
                foreach (Player p in ReInput.players.Players) {
                    if (p.controllers.joystickCount == 0) {
                        p.controllers.AddController(joystick, true);
                        return;
                    }
                }
            }

            public void OnDestroy() {
                ReInput.ControllerConnectedEvent -= OnControllerConnected;
                ReInput.ControllerDisconnectedEvent -= OnControllerDisconnected;
                ReInput.ControllerPreDisconnectEvent -= OnControllerPreDisconnect;
            }
        }
    }
}