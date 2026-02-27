using Godot;

public partial class State : Node
{
  protected StateMachine stateMachine;

  public virtual void Enter() { }
  public virtual void Update(float delta) { }
  public virtual void Exit() { }

  public virtual void Initialize(StateMachine stateMachine)
  {
    this.stateMachine = stateMachine;
  }
}