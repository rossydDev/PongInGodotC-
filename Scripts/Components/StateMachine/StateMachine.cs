using Godot;
using System.Collections.Generic;

public partial class StateMachine : Node
{
  [Export] private State currentState;
  private List<State> states = new List<State>();

  public State CurrentState => currentState;

  public override void _Ready()
  {
    GetOwner().Ready += Initialize;
  }

  private void Initialize()
  {
    foreach (State state in GetChildren())
    {
      if (state != null)
      {
        state.Initialize(this);
        states.Add(state);

        if (state.GetChildCount() > 0)
        {
          foreach (State s in state.GetChildren())
          {
            s.Initialize(this);
            states.Add(s);
          }
        }
      }
    }

    currentState.Enter();
  }

  public override void _PhysicsProcess(double delta)
  {
    currentState.Update((float)delta);
  }

  public void SwitchState<T>()
  {
    State newState = null;

    foreach (State state in states)
    {
      if (state is T)
      {
        newState = state;
      }
    }

    if (newState == null) return;

    currentState.Exit();
    currentState = newState;
    currentState.Enter();
  }

  public void SwitchState(State newState)
  {
    if (currentState == newState) return;

    foreach (State state in states)
    {
      if (state == newState)
      {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
        break;
      }
    }
  }

}
