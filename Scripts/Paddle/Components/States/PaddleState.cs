using Godot;

public partial class PaddleState : State
{
  protected Paddle paddle;

  public override void Initialize(StateMachine stateMachine)
  {
    base.Initialize(stateMachine);

    if (stateMachine.GetOwner() is Paddle)
    {
      paddle = (Paddle)stateMachine.GetOwner();
    }
  }
}