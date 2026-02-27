public partial class PaddleTrackingState : PaddleState
{
  public override void Update(float delta)
  {
    float target = GameManager.Instance.CurrentBall.GlobalPosition.X;

    paddle.MoveTowards(target, delta);
  }
}