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
        public InputActionAsset asset { get; }
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
                    ""name"": ""Button A"",
                    ""type"": ""Button"",
                    ""id"": ""698c8bc6-bad4-4347-91be-d19beca706cc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Button B"",
                    ""type"": ""Button"",
                    ""id"": ""f16c23f9-9ac7-44d3-ba9e-feebc2ee32db"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Button C"",
                    ""type"": ""Button"",
                    ""id"": ""07a234c5-b325-442b-9e1a-e663dbf5979a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Button D"",
                    ""type"": ""Button"",
                    ""id"": ""654ebbee-5600-4e78-b57b-588a35a2880d"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Button S"",
                    ""type"": ""Button"",
                    ""id"": ""b8ec316a-815f-4511-817e-da94e33002c9"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
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
                    ""path"": ""2DVector"",
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
                    ""id"": ""53d0653f-9b0d-4549-9d99-26a8fb93badc"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
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
                    ""path"": ""<Keyboard>/o"",
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
        },
        {
            ""name"": ""Ui"",
            ""id"": ""42a215a5-9c30-4383-a0f0-1aa715a1e303"",
            ""actions"": [
                {
                    ""name"": ""Submit"",
                    ""type"": ""PassThrough"",
                    ""id"": ""a80c2d57-3806-4372-9038-b3fa922fe834"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""PassThrough"",
                    ""id"": ""c77a51cc-4711-437c-98f9-b66d23866275"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Navigate"",
                    ""type"": ""PassThrough"",
                    ""id"": ""5ba06146-a806-482c-9f92-a6ca753b23e8"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""63c3305a-c0e5-4eff-a754-6e00552cd9ce"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a1401bf1-316e-4082-b627-560b573466d6"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2e2f1c61-0777-4ea1-a0f7-e0366a2d354c"",
                    ""path"": ""<Keyboard>/k"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""28ba5a0b-f8e6-4739-b91f-fc72d8e514ba"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""45cdc980-b8e3-4137-acf2-85ea0480a65e"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Navigate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""2c12facf-9dab-4c3b-9921-804445ae1ce6"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""1681f44b-89ee-4dfd-be2d-246b89ad9097"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""c8d75b98-2698-4963-a914-de66741ddf8d"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""2f55109c-708f-4bec-9917-e3e9cabd028d"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""62b2de9a-f413-4e8f-890a-934537513f2d"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""27038f25-0192-4e47-836b-af52915c0c10"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Navigate"",
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
            m_GamePlay_ButtonA = m_GamePlay.FindAction("Button A", throwIfNotFound: true);
            m_GamePlay_ButtonB = m_GamePlay.FindAction("Button B", throwIfNotFound: true);
            m_GamePlay_ButtonC = m_GamePlay.FindAction("Button C", throwIfNotFound: true);
            m_GamePlay_ButtonD = m_GamePlay.FindAction("Button D", throwIfNotFound: true);
            m_GamePlay_ButtonS = m_GamePlay.FindAction("Button S", throwIfNotFound: true);
            // Ui
            m_Ui = asset.FindActionMap("Ui", throwIfNotFound: true);
            m_Ui_Submit = m_Ui.FindAction("Submit", throwIfNotFound: true);
            m_Ui_Cancel = m_Ui.FindAction("Cancel", throwIfNotFound: true);
            m_Ui_Navigate = m_Ui.FindAction("Navigate", throwIfNotFound: true);
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

        // Ui
        private readonly InputActionMap m_Ui;
        private IUiActions m_UiActionsCallbackInterface;
        private readonly InputAction m_Ui_Submit;
        private readonly InputAction m_Ui_Cancel;
        private readonly InputAction m_Ui_Navigate;
        public struct UiActions
        {
            private @BasicActions m_Wrapper;
            public UiActions(@BasicActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @Submit => m_Wrapper.m_Ui_Submit;
            public InputAction @Cancel => m_Wrapper.m_Ui_Cancel;
            public InputAction @Navigate => m_Wrapper.m_Ui_Navigate;
            public InputActionMap Get() { return m_Wrapper.m_Ui; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(UiActions set) { return set.Get(); }
            public void SetCallbacks(IUiActions instance)
            {
                if (m_Wrapper.m_UiActionsCallbackInterface != null)
                {
                    @Submit.started -= m_Wrapper.m_UiActionsCallbackInterface.OnSubmit;
                    @Submit.performed -= m_Wrapper.m_UiActionsCallbackInterface.OnSubmit;
                    @Submit.canceled -= m_Wrapper.m_UiActionsCallbackInterface.OnSubmit;
                    @Cancel.started -= m_Wrapper.m_UiActionsCallbackInterface.OnCancel;
                    @Cancel.performed -= m_Wrapper.m_UiActionsCallbackInterface.OnCancel;
                    @Cancel.canceled -= m_Wrapper.m_UiActionsCallbackInterface.OnCancel;
                    @Navigate.started -= m_Wrapper.m_UiActionsCallbackInterface.OnNavigate;
                    @Navigate.performed -= m_Wrapper.m_UiActionsCallbackInterface.OnNavigate;
                    @Navigate.canceled -= m_Wrapper.m_UiActionsCallbackInterface.OnNavigate;
                }
                m_Wrapper.m_UiActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Submit.started += instance.OnSubmit;
                    @Submit.performed += instance.OnSubmit;
                    @Submit.canceled += instance.OnSubmit;
                    @Cancel.started += instance.OnCancel;
                    @Cancel.performed += instance.OnCancel;
                    @Cancel.canceled += instance.OnCancel;
                    @Navigate.started += instance.OnNavigate;
                    @Navigate.performed += instance.OnNavigate;
                    @Navigate.canceled += instance.OnNavigate;
                }
            }
        }
        public UiActions @Ui => new UiActions(this);
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
            void OnButtonA(InputAction.CallbackContext context);
            void OnButtonB(InputAction.CallbackContext context);
            void OnButtonC(InputAction.CallbackContext context);
            void OnButtonD(InputAction.CallbackContext context);
            void OnButtonS(InputAction.CallbackContext context);
        }
        public interface IUiActions
        {
            void OnSubmit(InputAction.CallbackContext context);
            void OnCancel(InputAction.CallbackContext context);
            void OnNavigate(InputAction.CallbackContext context);
        }
    }
}
