using System;
using System.Linq;
using CoreLoop.Interfaces;
using UnityEngine;
using Zenject;

namespace CoreLoop
{
    public class StateSwichInitiator : MonoBehaviour
    {
        [Inject] private IGameStateMachine gameStateMachine;
        [Inject] private DiContainer container;

        [SerializeField] private SceneStateSelection initialState;
        
        private void Start()
        {
            LoadState();
        }
        
        private void LoadState()
        {
            initialState.ValidateAndInitialize();

            if (initialState.stateType == SceneStateType.None) return;

            if (!SceneStateRegistry.TryGetDescriptor(initialState.stateType, out var descriptor)) return;

            if (descriptor.HasPayload && initialState.payload == null)
            {
                Debug.LogError($"Scene state '{initialState.stateType}' requires payload '{descriptor.PayloadType.Name}', but payload could not be created.");
                return;
            }

            object[] extraArgs = descriptor.HasPayload ? new object[] { initialState.payload } : Array.Empty<object>();
            var state = container.Instantiate(descriptor.StateType, extraArgs) as State;

            if (state == null)
            {
                Debug.LogError($"Failed to instantiate scene state '{descriptor.StateType.Name}'.");
                return;
            }

            ChangeState(state, descriptor);
        }

        private void ChangeState(State state, SceneStateDescriptor descriptor)
        {
            if (!descriptor.HasPayload)
            {
                gameStateMachine.ChangeState(state);
                return;
            }

            var stateWithPayloadType = typeof(State<>).MakeGenericType(descriptor.PayloadType);
            if (!stateWithPayloadType.IsInstanceOfType(state))
            {
                Debug.LogError($"State '{descriptor.StateType.Name}' is registered with payload '{descriptor.PayloadType.Name}', but does not inherit from the matching State<TPayload> type.");
                return;
            }

            var changeStateMethod = typeof(IGameStateMachine).GetMethods()
                .First(method => method.Name == nameof(IGameStateMachine.ChangeState) && method.IsGenericMethodDefinition)
                .MakeGenericMethod(descriptor.PayloadType);

            changeStateMethod.Invoke(gameStateMachine, new object[] { state, initialState.payload });
        }
    }
}