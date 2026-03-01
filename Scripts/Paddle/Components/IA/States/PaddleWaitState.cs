using Godot;

public partial class PaddleWaitState : PaddleState
{
  public override void Update(float delta)
  {
    BallBase ball = BallController.Instance.CurrentBall;
    if (ball == null) return;

    float paddleY = paddle.GlobalPosition.Y;
    bool ballComingToMe = Mathf.Sign(ball.Velocity.Y) == Mathf.Sign(paddleY - ball.GlobalPosition.Y);

    // Bola voltou — reage novamente com delay fresh
    if (ballComingToMe)
      stateMachine.SwitchState<PaddleReactState>();
  }
}