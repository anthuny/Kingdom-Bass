// GENERATED AUTOMATICALLY FROM 'Assets/Controller.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controller : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controller()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controller"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""a595bd0f-f984-49a2-aaab-eba4aa766e54"",
            ""actions"": [
                {
                    ""name"": ""Shield"",
                    ""type"": ""Button"",
                    ""id"": ""bd817223-95c1-4f8b-b220-83f3d91ab5a8"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""4a33fe8f-43d8-41c0-8ad6-a687993e952b"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""BlastL"",
                    ""type"": ""Button"",
                    ""id"": ""fa13bc88-4663-4835-97d8-3c034c958e07"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""BlastR"",
                    ""type"": ""Button"",
                    ""id"": ""47e89aa3-8938-4b1a-bee3-c0500e70d959"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""424f04ea-4585-44da-bc2e-045249ebc579"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shield"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""691adba1-3cf8-4773-9d65-87ad289a26bb"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5d6cdf30-e452-4169-84c8-d46eb75e8016"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BlastL"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8bba6969-9e55-49f1-bdd6-914c40daecdc"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BlastR"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Shield = m_Gameplay.FindAction("Shield", throwIfNotFound: true);
        m_Gameplay_Move = m_Gameplay.FindAction("Move", throwIfNotFound: true);
        m_Gameplay_BlastL = m_Gameplay.FindAction("BlastL", throwIfNotFound: true);
        m_Gameplay_BlastR = m_Gameplay.FindAction("BlastR", throwIfNotFound: true);
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

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_Shield;
    private readonly InputAction m_Gameplay_Move;
    private readonly InputAction m_Gameplay_BlastL;
    private readonly InputAction m_Gameplay_BlastR;
    public struct GameplayActions
    {
        private @Controller m_Wrapper;
        public GameplayActions(@Controller wrapper) { m_Wrapper = wrapper; }
        public InputAction @Shield => m_Wrapper.m_Gameplay_Shield;
        public InputAction @Move => m_Wrapper.m_Gameplay_Move;
        public InputAction @BlastL => m_Wrapper.m_Gameplay_BlastL;
        public InputAction @BlastR => m_Wrapper.m_Gameplay_BlastR;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @Shield.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnShield;
                @Shield.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnShield;
                @Shield.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnShield;
                @Move.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @BlastL.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBlastL;
                @BlastL.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBlastL;
                @BlastL.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBlastL;
                @BlastR.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBlastR;
                @BlastR.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBlastR;
                @BlastR.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBlastR;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Shield.started += instance.OnShield;
                @Shield.performed += instance.OnShield;
                @Shield.canceled += instance.OnShield;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @BlastL.started += instance.OnBlastL;
                @BlastL.performed += instance.OnBlastL;
                @BlastL.canceled += instance.OnBlastL;
                @BlastR.started += instance.OnBlastR;
                @BlastR.performed += instance.OnBlastR;
                @BlastR.canceled += instance.OnBlastR;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    public interface IGameplayActions
    {
        void OnShield(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
        void OnBlastL(InputAction.CallbackContext context);
        void OnBlastR(InputAction.CallbackContext context);
    }
}
