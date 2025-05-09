//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/InputActions/InputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace HexFlow.Input
{
    public partial class @HFInput: IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @HFInput()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions"",
    ""maps"": [
        {
            ""name"": ""gameplay"",
            ""id"": ""c21a6ab8-a43e-4f39-8947-17be25ccc138"",
            ""actions"": [
                {
                    ""name"": ""move"",
                    ""type"": ""Value"",
                    ""id"": ""c1ae0cee-be71-4ee2-abbd-5ae9a9792102"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""zoom"",
                    ""type"": ""Value"",
                    ""id"": ""26f71952-f7da-437a-895a-ab9ec17e09f0"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": ""Clamp(min=-1,max=1)"",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""buttonZoom"",
                    ""type"": ""Value"",
                    ""id"": ""5289d379-d03e-4aa8-98f5-69dfd363a407"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""cursor"",
                    ""type"": ""Value"",
                    ""id"": ""694e7ac7-e7d0-456e-b6ab-d45038462092"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""place"",
                    ""type"": ""Button"",
                    ""id"": ""11a4727c-4981-40ce-b9c2-956d2a462261"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""destroy"",
                    ""type"": ""Button"",
                    ""id"": ""8acd081e-9ea5-4315-a401-f5df8903492c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""be78e55a-bdb1-4e44-b8f0-586c949e2066"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": ""NormalizeVector2,ScaleVector2(x=-10,y=-10)"",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""fbffec70-bc0b-4fd2-9c90-950eb4253339"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""afbc1678-89b2-43bb-a668-efa7bc2c910a"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""ed22d13d-4d10-4dd3-8b87-53e54b3b0111"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""5b8471c1-d99b-4744-9838-02c476e6b51e"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""mouseWithMiddle"",
                    ""id"": ""95fe55c4-d178-45fb-95a8-76297b335bb4"",
                    ""path"": ""OneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""4c31b8f5-23fc-4682-8788-5b89a7df73a5"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""binding"",
                    ""id"": ""a07bf4a1-f7e1-4231-b4c9-89918821485d"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""mouseScroll"",
                    ""id"": ""87b24c63-a0d4-4cbc-9798-3ba1d5630c86"",
                    ""path"": ""1DAxis(whichSideWins=1)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""zoom"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""90a6fc35-d968-4905-8146-a5b5c3724028"",
                    ""path"": ""<Mouse>/scroll/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""71a723cc-faaf-4b59-895d-1e74f1a23e72"",
                    ""path"": ""<Mouse>/scroll/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""4f140f87-4d97-4809-9add-8060d945a6a5"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""cursor"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""numpad"",
                    ""id"": ""118738ef-6132-43ca-bb34-efa5375ce995"",
                    ""path"": ""1DAxis(whichSideWins=1)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""buttonZoom"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""a74e734a-4cb5-4706-b640-e566629563e5"",
                    ""path"": ""<Keyboard>/numpadMinus"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""buttonZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""e233c2bc-7b3f-49ca-bc1f-16818f59a6fc"",
                    ""path"": ""<Keyboard>/numpadPlus"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""buttonZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""3f22b97b-4a55-4333-b85d-0ecd406cf578"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""place"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7ee185c0-25bb-4975-a3b9-7c3184bbc7b8"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""destroy"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""ui"",
            ""id"": ""d2395c9e-f515-4221-8719-0c9f1d3bc5b9"",
            ""actions"": [
                {
                    ""name"": ""quit"",
                    ""type"": ""Button"",
                    ""id"": ""bd4ea2a1-170e-4d0e-8f55-c9e7228f2159"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""14fe83ad-5105-4370-9061-eaa5980bf1dd"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""quit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4741459b-8487-4388-a65b-57fb2cb2108a"",
                    ""path"": ""<Mouse>/backButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""quit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // gameplay
            m_gameplay = asset.FindActionMap("gameplay", throwIfNotFound: true);
            m_gameplay_move = m_gameplay.FindAction("move", throwIfNotFound: true);
            m_gameplay_zoom = m_gameplay.FindAction("zoom", throwIfNotFound: true);
            m_gameplay_buttonZoom = m_gameplay.FindAction("buttonZoom", throwIfNotFound: true);
            m_gameplay_cursor = m_gameplay.FindAction("cursor", throwIfNotFound: true);
            m_gameplay_place = m_gameplay.FindAction("place", throwIfNotFound: true);
            m_gameplay_destroy = m_gameplay.FindAction("destroy", throwIfNotFound: true);
            // ui
            m_ui = asset.FindActionMap("ui", throwIfNotFound: true);
            m_ui_quit = m_ui.FindAction("quit", throwIfNotFound: true);
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

        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }

        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // gameplay
        private readonly InputActionMap m_gameplay;
        private List<IGameplayActions> m_GameplayActionsCallbackInterfaces = new List<IGameplayActions>();
        private readonly InputAction m_gameplay_move;
        private readonly InputAction m_gameplay_zoom;
        private readonly InputAction m_gameplay_buttonZoom;
        private readonly InputAction m_gameplay_cursor;
        private readonly InputAction m_gameplay_place;
        private readonly InputAction m_gameplay_destroy;
        public struct GameplayActions
        {
            private @HFInput m_Wrapper;
            public GameplayActions(@HFInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @move => m_Wrapper.m_gameplay_move;
            public InputAction @zoom => m_Wrapper.m_gameplay_zoom;
            public InputAction @buttonZoom => m_Wrapper.m_gameplay_buttonZoom;
            public InputAction @cursor => m_Wrapper.m_gameplay_cursor;
            public InputAction @place => m_Wrapper.m_gameplay_place;
            public InputAction @destroy => m_Wrapper.m_gameplay_destroy;
            public InputActionMap Get() { return m_Wrapper.m_gameplay; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
            public void AddCallbacks(IGameplayActions instance)
            {
                if (instance == null || m_Wrapper.m_GameplayActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_GameplayActionsCallbackInterfaces.Add(instance);
                @move.started += instance.OnMove;
                @move.performed += instance.OnMove;
                @move.canceled += instance.OnMove;
                @zoom.started += instance.OnZoom;
                @zoom.performed += instance.OnZoom;
                @zoom.canceled += instance.OnZoom;
                @buttonZoom.started += instance.OnButtonZoom;
                @buttonZoom.performed += instance.OnButtonZoom;
                @buttonZoom.canceled += instance.OnButtonZoom;
                @cursor.started += instance.OnCursor;
                @cursor.performed += instance.OnCursor;
                @cursor.canceled += instance.OnCursor;
                @place.started += instance.OnPlace;
                @place.performed += instance.OnPlace;
                @place.canceled += instance.OnPlace;
                @destroy.started += instance.OnDestroy;
                @destroy.performed += instance.OnDestroy;
                @destroy.canceled += instance.OnDestroy;
            }

            private void UnregisterCallbacks(IGameplayActions instance)
            {
                @move.started -= instance.OnMove;
                @move.performed -= instance.OnMove;
                @move.canceled -= instance.OnMove;
                @zoom.started -= instance.OnZoom;
                @zoom.performed -= instance.OnZoom;
                @zoom.canceled -= instance.OnZoom;
                @buttonZoom.started -= instance.OnButtonZoom;
                @buttonZoom.performed -= instance.OnButtonZoom;
                @buttonZoom.canceled -= instance.OnButtonZoom;
                @cursor.started -= instance.OnCursor;
                @cursor.performed -= instance.OnCursor;
                @cursor.canceled -= instance.OnCursor;
                @place.started -= instance.OnPlace;
                @place.performed -= instance.OnPlace;
                @place.canceled -= instance.OnPlace;
                @destroy.started -= instance.OnDestroy;
                @destroy.performed -= instance.OnDestroy;
                @destroy.canceled -= instance.OnDestroy;
            }

            public void RemoveCallbacks(IGameplayActions instance)
            {
                if (m_Wrapper.m_GameplayActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IGameplayActions instance)
            {
                foreach (var item in m_Wrapper.m_GameplayActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_GameplayActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public GameplayActions @gameplay => new GameplayActions(this);

        // ui
        private readonly InputActionMap m_ui;
        private List<IUiActions> m_UiActionsCallbackInterfaces = new List<IUiActions>();
        private readonly InputAction m_ui_quit;
        public struct UiActions
        {
            private @HFInput m_Wrapper;
            public UiActions(@HFInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @quit => m_Wrapper.m_ui_quit;
            public InputActionMap Get() { return m_Wrapper.m_ui; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(UiActions set) { return set.Get(); }
            public void AddCallbacks(IUiActions instance)
            {
                if (instance == null || m_Wrapper.m_UiActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_UiActionsCallbackInterfaces.Add(instance);
                @quit.started += instance.OnQuit;
                @quit.performed += instance.OnQuit;
                @quit.canceled += instance.OnQuit;
            }

            private void UnregisterCallbacks(IUiActions instance)
            {
                @quit.started -= instance.OnQuit;
                @quit.performed -= instance.OnQuit;
                @quit.canceled -= instance.OnQuit;
            }

            public void RemoveCallbacks(IUiActions instance)
            {
                if (m_Wrapper.m_UiActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IUiActions instance)
            {
                foreach (var item in m_Wrapper.m_UiActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_UiActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public UiActions @ui => new UiActions(this);
        public interface IGameplayActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnZoom(InputAction.CallbackContext context);
            void OnButtonZoom(InputAction.CallbackContext context);
            void OnCursor(InputAction.CallbackContext context);
            void OnPlace(InputAction.CallbackContext context);
            void OnDestroy(InputAction.CallbackContext context);
        }
        public interface IUiActions
        {
            void OnQuit(InputAction.CallbackContext context);
        }
    }
}
