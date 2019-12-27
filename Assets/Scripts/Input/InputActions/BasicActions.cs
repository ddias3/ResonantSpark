// GENERATED AUTOMATICALLY FROM 'Assets/InputActions/BasicActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace ResonantSpark
{
    public class @BasicActions : IInputActionCollection, IDisposable
    {
        private InputActionAsset asset;
        public @BasicActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""BasicActions"",
    ""maps"": [
        {
            ""name"": ""GamePlay"",
            ""id"": ""081761cf-b8b6-4282-8428-dec7e0e43be2"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""PassThrough"",
                    ""id"": ""908f578c-b4b3-4558-b244-e9c99b000415"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Fire"",
                    ""type"": ""PassThrough"",
                    ""id"": ""0af61e80-070d-4356-88bf-2e37ddf84a78"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Button A"",
                    ""type"": ""Button"",
                    ""id"": ""698c8bc6-bad4-4347-91be-d19beca706cc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Button B"",
                    ""type"": ""Button"",
                    ""id"": ""f16c23f9-9ac7-44d3-ba9e-feebc2ee32db"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Button C"",
                    ""type"": ""Button"",
                    ""id"": ""07a234c5-b325-442b-9e1a-e663dbf5979a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Button D"",
                    ""type"": ""Button"",
                    ""id"": ""654ebbee-5600-4e78-b57b-588a35a2880d"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Button S"",
                    ""type"": ""Button"",
                    ""id"": ""b8ec316a-815f-4511-817e-da94e33002c9"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""6457e8c5-3c7c-491f-8029-4726ce1547b4"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""cdc23752-97e4-4788-acaa-4b8caee1fb6d"",
                    ""path"": ""2DVector(normalize=false)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""0308b7c5-e860-44e6-835f-3fdaced812e5"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""97c55b7b-25b2-471e-84d0-945c0524e267"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""c06ff365-476f-4f69-a5aa-915ae1604c5b"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f7aefa0c-a14e-4510-a42a-d980ce2736a4"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""f3ed63da-2e86-47c0-a5e1-2d4adc0f9fa5"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""ArrowKeys"",
                    ""id"": ""d5db8765-98fe-4a96-bc8b-cd791231fe3c"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""7c59cf04-529e-4ba7-a76c-9523d0ec522f"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""5110a0a8-590e-4478-8e53-5a6bd18a9466"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""127a19e3-8400-4420-a1f7-14b61530b9ff"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""3ba1088b-2af1-405e-b220-01b3ad27a9c2"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""4a3a76ce-014c-4afc-bc37-68b02eae4922"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Button A"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aa1781ad-7e57-422c-831c-e51e45e50542"",
                    ""path"": ""<Keyboard>/u"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Button A"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""869974e3-95e8-4fbc-870a-3ef3af892418"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Button B"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e60bf4a4-1c07-4d6b-80a7-8a5a12c6cc53"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Button B"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""04c5fc1b-080e-4152-90d4-902ec487a6b6"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Button C"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""88b002d2-e190-4980-8dd8-cee3f52d7875"",
                    ""path"": ""<Keyboard>/k"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Button C"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1d7899df-35aa-4f2f-b146-20b9c6e337b1"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Button D"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7fdbc3b5-d050-4224-83cc-7723d7f8c918"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Button D"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cb1f2230-8648-4c84-a748-60097ab73f7e"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Button S"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""59f6c8fa-7a45-49e4-84a8-95ddb9085b36"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Button S"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // GamePlay
            m_GamePlay = asset.FindActionMap("GamePlay", throwIfNotFound: true);
            m_GamePlay_Move = m_GamePlay.FindAction("Move", throwIfNotFound: true);
            m_GamePlay_Fire = m_GamePlay.FindAction("Fire", throwIfNotFound: true);
            m_GamePlay_ButtonA = m_GamePlay.FindAction("Button A", throwIfNotFound: true);
            m_GamePlay_ButtonB = m_GamePlay.FindAction("Button B", throwIfNotFound: true);
            m_GamePlay_ButtonC = m_GamePlay.FindAction("Button C", throwIfNotFound: true);
            m_GamePlay_ButtonD = m_GamePlay.FindAction("Button D", throwIfNotFound: true);
            m_GamePlay_ButtonS = m_GamePlay.FindAction("Button S", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // GamePlay
        private readonly InputActionMap m_GamePlay;
        private IGamePlayActions m_GamePlayActionsCallbackInterface;
        private readonly InputAction m_GamePlay_Move;
        private readonly InputAction m_GamePlay_Fire;
        private readonly InputAction m_GamePlay_ButtonA;
        private readonly InputAction m_GamePlay_ButtonB;
        private readonly InputAction m_GamePlay_ButtonC;
        private readonly InputAction m_GamePlay_ButtonD;
        private readonly InputAction m_GamePlay_ButtonS;
        public struct GamePlayActions
        {
            private @BasicActions m_Wrapper;
            public GamePlayActions(@BasicActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_GamePlay_Move;
            public InputAction @Fire => m_Wrapper.m_GamePlay_Fire;
            public InputAction @ButtonA => m_Wrapper.m_GamePlay_ButtonA;
            public InputAction @ButtonB => m_Wrapper.m_GamePlay_ButtonB;
            public InputAction @ButtonC => m_Wrapper.m_GamePlay_ButtonC;
            public InputAction @ButtonD => m_Wrapper.m_GamePlay_ButtonD;
            public InputAction @ButtonS => m_Wrapper.m_GamePlay_ButtonS;
            public InputActionMap Get() { return m_Wrapper.m_GamePlay; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GamePlayActions set) { return set.Get(); }
            public void SetCallbacks(IGamePlayActions instance)
            {
                if (m_Wrapper.m_GamePlayActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnMove;
                    @Fire.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnFire;
                    @Fire.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnFire;
                    @Fire.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnFire;
                    @ButtonA.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnButtonA;
                    @ButtonA.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnButtonA;
                    @ButtonA.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnButtonA;
                    @ButtonB.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnButtonB;
                    @ButtonB.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnButtonB;
                    @ButtonB.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnButtonB;
                    @ButtonC.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnButtonC;
                    @ButtonC.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnButtonC;
                    @ButtonC.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnButtonC;
                    @ButtonD.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnButtonD;
                    @ButtonD.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnButtonD;
                    @ButtonD.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnButtonD;
                    @ButtonS.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnButtonS;
                    @ButtonS.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnButtonS;
                    @ButtonS.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnButtonS;
                }
                m_Wrapper.m_GamePlayActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @Fire.started += instance.OnFire;
                    @Fire.performed += instance.OnFire;
                    @Fire.canceled += instance.OnFire;
                    @ButtonA.started += instance.OnButtonA;
                    @ButtonA.performed += instance.OnButtonA;
                    @ButtonA.canceled += instance.OnButtonA;
                    @ButtonB.started += instance.OnButtonB;
                    @ButtonB.performed += instance.OnButtonB;
                    @ButtonB.canceled += instance.OnButtonB;
                    @ButtonC.started += instance.OnButtonC;
                    @ButtonC.performed += instance.OnButtonC;
                    @ButtonC.canceled += instance.OnButtonC;
                    @ButtonD.started += instance.OnButtonD;
                    @ButtonD.performed += instance.OnButtonD;
                    @ButtonD.canceled += instance.OnButtonD;
                    @ButtonS.started += instance.OnButtonS;
                    @ButtonS.performed += instance.OnButtonS;
                    @ButtonS.canceled += instance.OnButtonS;
                }
            }
        }
        public GamePlayActions @GamePlay => new GamePlayActions(this);
        private int m_KeyboardSchemeIndex = -1;
        public InputControlScheme KeyboardScheme
        {
            get
            {
                if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
                return asset.controlSchemes[m_KeyboardSchemeIndex];
            }
        }
        private int m_GamepadSchemeIndex = -1;
        public InputControlScheme GamepadScheme
        {
            get
            {
                if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
                return asset.controlSchemes[m_GamepadSchemeIndex];
            }
        }
        public interface IGamePlayActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnFire(InputAction.CallbackContext context);
            void OnButtonA(InputAction.CallbackContext context);
            void OnButtonB(InputAction.CallbackContext context);
            void OnButtonC(InputAction.CallbackContext context);
            void OnButtonD(InputAction.CallbackContext context);
            void OnButtonS(InputAction.CallbackContext context);
        }
    }
}
