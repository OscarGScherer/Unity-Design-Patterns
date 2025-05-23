using System;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns
{
    namespace StateMachine
    {
        public abstract class StateType 
        { 
            public abstract Type type {get;}
            public abstract State CreateNewState();
        }
        /// <summary> Class used to hold a type of state that the SM should transition to. </summary>
        /// <typeparam name="T"> The type of State held in this class </typeparam>
        public class StateType<T> : StateType where T : State, new()
        {
            public override Type type => typeof(T);
            public override State CreateNewState() => new T();
        }

        /// <summary>
        /// Base state class. Inherit from it to create your own state.
        /// Each state machine can only have one state of each type of State.
        /// </summary>
        public abstract class State
        {
            /// <summary>
            /// Implement this to get all necessary references for your state to run.
            /// This function will be called the first time the state machine entered the state.
            /// Called before OnEnter.
            /// </summary>
            /// <param name="gameObject"> The gameobject the outer SM is a component of </param>
            public abstract void Initialize(GameObject gameObject);
            /// <summary>
            /// Called when the SM enters this state.
            /// </summary>
            public virtual void OnEnter() {}
            /// <summary>
            /// Called every frame while the SM is in this state.
            /// Controls transitions to other states
            /// </summary>
            /// <returns>
            /// The state the SM should transition to. 
            /// Only the type of the returned state is used,
            /// if you return new StateA(), the SM will transition to
            /// its own instance of type StateA.
            /// </returns>
            public abstract StateType Update(float deltaTime);
            /// <summary>
            /// Called whenever the SM exits this state
            /// </summary>
            public virtual void OnExit() {}
            /// <returns> Text to be printed on the inpector, whenver the SM is in this state. </returns>
            public virtual string ToDebugString() => "You can override \"ToDebugString()\" to display custom debug information about your state here!";
        }

        /// <summary>
        /// Base state machine class. Inherit from it to create your own.
        /// </summary>
        public abstract class StateMachine : MonoBehaviour
        {
            public State currentState;
            protected Dictionary<Type,State> states = new Dictionary<Type, State>();

            /// <summary>
            /// Adds a state to the SM, the state will not be added if the SM already has a state of that type.
            /// </summary>
            protected void AddState(StateType stateType)
            {
                if(states.ContainsKey(stateType.type))
                {
                    Debug.LogWarning("Adding a duplicate state of type " + stateType.type + " on state machine in " + name);
                    return;
                }
                State newState = stateType.CreateNewState();
                newState.Initialize(gameObject);
                states.Add(newState.GetType(), newState);
            }
            /// <summary> Adds a state of type T to the SM, it will not be added if the SM already has a state of type T. </summary>
            protected void AddState<T>() where T : State, new() => AddState(new StateType<T>());
            /// <returns>
            /// The state of this SM of the same type as the given state.
            /// The state is added to the SM if the SM doesn't have one of its type.
            /// </returns>
            protected State GetStateOf(StateType stateType)
            {
                if(!states.ContainsKey(stateType.type)) AddState(stateType);
                return states[stateType.type];
            }
            /// <returns>
            /// The state of this SM of type T.
            /// A new state of type T is added to the SM if it doesn't have one already.
            /// </returns>
            protected State GetStateOfType<T>() where T : State, new()
            {
                if(!states.ContainsKey(typeof(T))) AddState<T>();
                return states[typeof(T)];
            }
            /// <summary>
            /// Immediatly transitions to the SM's state of type T.
            /// Adds new one of type T if the SM doesn't have one already.
            /// </summary>
            protected void SetStartingState<T>() where T : State, new()
            {
                if(!states.ContainsKey(typeof(T))) AddState<T>();
                currentState?.OnExit();
                currentState = GetStateOfType<T>();
                currentState.OnEnter();
            }
            /// <summary>
            /// Updates the SM and handles any transitions.
            /// </summary>
            /// <param name="deltaTime"></param>
            protected void UpdateStateMachine(float deltaTime)
            {
                StateType newState = currentState.Update(deltaTime);
                HandleTransition(newState);
            }
            /// <summary>
            /// Transitions to the SM's state of the same type of the given newState.
            /// Properly calls OnExit and OnEnter.
            /// </summary>
            /// <param name="stateType">
            /// The state of the type wanted to transition to.
            /// </param>
            protected void HandleTransition(StateType stateType)
            {
                if(stateType == null) return;
                if(stateType.type == currentState.GetType()) return;
                currentState.OnExit();
                currentState = GetStateOf(stateType);
                currentState.OnEnter();
            }
        }
    }
}
