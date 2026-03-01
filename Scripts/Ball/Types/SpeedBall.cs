using Godot;

public partial class SpeedBall : BallBase
{
  [Export] private float speedMultiplierOnBounce = 1.1f;

  protected override void OnBouncePaddle()
  {
    base.OnBouncePaddle();

    ballSpeed *= speedMultiplierOnBounce;
  }
}