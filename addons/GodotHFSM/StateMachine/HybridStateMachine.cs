﻿namespace GodotHFSM;

using System;

/// <summary>
/// A StateMachine that is also like a normal State in the sense that it allows you to run
/// custom code on enter, on logic, ... besides its active state's code.
/// It is especially handy for hierarchical state machines, as it allows you to factor out
/// common code from the sub states into the HybridStateMachines, essentially removing
/// duplicate code.
/// The HybridStateMachine can also be seen as a StateWrapper around a normal StateMachine.
/// </summary>
/// <typeparam name="TOwnId"></typeparam>
/// <typeparam name="TStateId"></typeparam>
/// <typeparam name="TEvent"></typeparam>
public class HybridStateMachine<TOwnId, TStateId, TEvent> : StateMachine<TOwnId, TStateId, TEvent> {
    private readonly Action<HybridStateMachine<TOwnId, TStateId, TEvent>>
        _beforeOnEnter, _afterOnEnter,
        _beforeOnExit, _afterOnExit;

    private readonly Action<HybridStateMachine<TOwnId, TStateId, TEvent>, double>
        _beforeOnLogic, _afterOnLogic;

    /// <summary>
    /// Lazily initialized
    /// </summary>
    private ActionStorage<TEvent> _actionStorage;

    public Timer Timer { get; } = new Timer();

    /// <summary>Initializes a new instance of the HybridStateMachine class.</summary>
    /// <param name="beforeOnEnter">A function that is called before running the sub-state's OnEnter.</param>
    /// <param name="afterOnEnter">A function that is called after running the sub-state's OnEnter.</param>
    /// <param name="beforeOnLogic">A function that is called before running the sub-state's OnLogic.</param>
    /// <param name="afterOnLogic">A function that is called after running the sub-state's OnLogic.</param>
    /// <param name="beforeOnExit">A function that is called before running the sub-state's OnExit.</param>
    /// <param name="afterOnExit">A function that is called after running the sub-state's OnExit.</param>
    /// <param name="needsExitTime">(Only for hierarchical states):
    /// 	Determines whether the state machine as a state of a parent state machine is allowed to instantly
    /// 	exit on a transition (false), or if it should wait until an explicit exit transition occurs.</param>
    /// <inheritdoc cref="StateBase{T}(bool, bool)"/>
    public HybridStateMachine(
                Action<HybridStateMachine<TOwnId, TStateId, TEvent>> beforeOnEnter = null,
                Action<HybridStateMachine<TOwnId, TStateId, TEvent>> afterOnEnter = null,

                Action<HybridStateMachine<TOwnId, TStateId, TEvent>, double> beforeOnLogic = null,
                Action<HybridStateMachine<TOwnId, TStateId, TEvent>, double> afterOnLogic = null,

                Action<HybridStateMachine<TOwnId, TStateId, TEvent>> beforeOnExit = null,
                Action<HybridStateMachine<TOwnId, TStateId, TEvent>> afterOnExit = null,

                bool needsExitTime = false,
                bool isGhostState = false) : base(needsExitTime, isGhostState) {
        _beforeOnEnter = beforeOnEnter;
        _afterOnEnter = afterOnEnter;

        _beforeOnLogic = beforeOnLogic;
        _afterOnLogic = afterOnLogic;

        _beforeOnExit = beforeOnExit;
        _afterOnExit = afterOnExit;
    }

    public override void OnEnter() {
        _beforeOnEnter?.Invoke(this);
        base.OnEnter();

        Timer.Reset();
        _afterOnEnter?.Invoke(this);
    }

    public override void OnLogic(double delta) {
        _beforeOnLogic?.Invoke(this, delta);
        base.OnLogic(delta);
        _afterOnLogic?.Invoke(this, delta);
    }

    public override void OnExit() {
        _beforeOnExit?.Invoke(this);
        base.OnExit();
        _afterOnExit?.Invoke(this);
    }

    public override void OnAction(TEvent trigger) {
        _actionStorage?.RunAction(trigger);
        base.OnAction(trigger);
    }

    public override void OnAction<TData>(TEvent trigger, TData data) {
        _actionStorage?.RunAction(trigger, data);
        base.OnAction(trigger, data);
    }

    /// <summary>
    /// Adds an action that can be called with OnAction(). Actions are like the builtin events
    /// OnEnter / OnLogic / ... but are defined by the user.
    /// The action is run before the sub-state's action.
    /// </summary>
    /// <param name="trigger">Name of the action</param>
    /// <param name="action">Function that should be called when the action is run</param>
    /// <returns>Itself</returns>
    public HybridStateMachine<TOwnId, TStateId, TEvent> AddAction(TEvent trigger, Action action) {
        _actionStorage ??= new ActionStorage<TEvent>();
        _actionStorage.AddAction(trigger, action);

        // Fluent interface
        return this;
    }

    /// <summary>
    /// Adds an action that can be called with OnAction<T>(). This overload allows you to
    /// run a function that takes one data parameter.
    /// The action is run before the sub-state's action.
    /// </summary>
    /// <param name="trigger">Name of the action</param>
    /// <param name="action">Function that should be called when the action is run</param>
    /// <typeparam name="TData">Data type of the parameter of the function</typeparam>
    /// <returns>Itself</returns>
    public HybridStateMachine<TOwnId, TStateId, TEvent> AddAction<TData>(TEvent trigger, Action<TData> action) {
        _actionStorage ??= new ActionStorage<TEvent>();
        _actionStorage.AddAction(trigger, action);

        // Fluent interface
        return this;
    }
}

/// <inheritdoc />
public class HybridStateMachine<TStateId, TEvent> : HybridStateMachine<TStateId, TStateId, TEvent> {
    /// <inheritdoc />
    public HybridStateMachine(
        Action<HybridStateMachine<TStateId, TStateId, TEvent>> beforeOnEnter = null,
        Action<HybridStateMachine<TStateId, TStateId, TEvent>> afterOnEnter = null,

        Action<HybridStateMachine<TStateId, TStateId, TEvent>, double> beforeOnLogic = null,
        Action<HybridStateMachine<TStateId, TStateId, TEvent>, double> afterOnLogic = null,

        Action<HybridStateMachine<TStateId, TStateId, TEvent>> beforeOnExit = null,
        Action<HybridStateMachine<TStateId, TStateId, TEvent>> afterOnExit = null,

        bool needsExitTime = false,
        bool isGhostState = false) : base(
            beforeOnEnter, afterOnEnter,
            beforeOnLogic, afterOnLogic,
            beforeOnExit, afterOnExit,
            needsExitTime,
            isGhostState
        ) {
    }
}

/// <inheritdoc />
public class HybridStateMachine<TStateId> : HybridStateMachine<TStateId, TStateId, string> {
    /// <inheritdoc />
    public HybridStateMachine(
        Action<HybridStateMachine<TStateId, TStateId, string>> beforeOnEnter = null,
        Action<HybridStateMachine<TStateId, TStateId, string>> afterOnEnter = null,

        Action<HybridStateMachine<TStateId, TStateId, string>, double> beforeOnLogic = null,
        Action<HybridStateMachine<TStateId, TStateId, string>, double> afterOnLogic = null,

        Action<HybridStateMachine<TStateId, TStateId, string>> beforeOnExit = null,
        Action<HybridStateMachine<TStateId, TStateId, string>> afterOnExit = null,

        bool needsExitTime = false,
        bool isGhostState = false) : base(
            beforeOnEnter, afterOnEnter,
            beforeOnLogic, afterOnLogic,
            beforeOnExit, afterOnExit,
            needsExitTime,
            isGhostState
        ) {
    }
}

/// <inheritdoc />
public class HybridStateMachine : HybridStateMachine<string, string, string> {
    /// <inheritdoc />
    public HybridStateMachine(
        Action<HybridStateMachine<string, string, string>> beforeOnEnter = null,
        Action<HybridStateMachine<string, string, string>> afterOnEnter = null,

        Action<HybridStateMachine<string, string, string>, double> beforeOnLogic = null,
        Action<HybridStateMachine<string, string, string>, double> afterOnLogic = null,

        Action<HybridStateMachine<string, string, string>> beforeOnExit = null,
        Action<HybridStateMachine<string, string, string>> afterOnExit = null,

        bool needsExitTime = false,
        bool isGhostState = false) : base(
            beforeOnEnter, afterOnEnter,
            beforeOnLogic, afterOnLogic,
            beforeOnExit, afterOnExit,
            needsExitTime,
            isGhostState
        ) {
    }
}
