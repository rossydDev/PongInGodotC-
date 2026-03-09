using Godot;

public partial class PaddleIdleState : PaddleState
{
  public override void Update(float delta)
  {
    float center = WorldBounds.Width / 2f;
    paddle.MoveTowards(center, delta);

    //Bola voltou para mim? Começar a reagir
    BallBase ball = paddle.Controller.BallProvider?.CurrentBall;

    if (ball == null) return;

    float paddleY = paddle.GlobalPosition.Y;
    bool ballComingToMe = Mathf.Sign(ball.Velocity.Y) == Mathf.Sign(paddleY - ball.GlobalPosition.Y);

    if (ballComingToMe)
    {
      stateMachine.SwitchState<PaddleReactState>();
    }
  }
}