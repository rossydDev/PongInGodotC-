public partial class PaddleIdleState : PaddleState
{
  public override void Update(float delta)
  {
    float center = GameManager.Instance.ScreenWidth / 2f;
    paddle.MoveTowards(center, delta);
  }
}