using Godot;

public partial class PaddleState : State
{
  protected PaddleIA paddle;

  public override void Initialize(StateMachine stateMachine)
  {
    base.Initialize(stateMachine);

    if (stateMachine.GetOwner() is Paddle)
    {
      paddle = (PaddleIA)stateMachine.GetOwner();
    }
  }
}