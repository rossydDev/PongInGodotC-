using Godot;

public partial class Gol : Node2D
{
  private Area2D area2D;
  [Export] private Paddle adversaryPaddle;

  public override void _Ready()
  {
    area2D = GetNode<Area2D>("Area2D");

    area2D.AreaEntered += OnAreaEntered;
  }

  private void OnAreaEntered(Area2D area)
  {
    if (area.GetOwner<BallBase>() == null) return;

    BallBase ball = area.GetOwner<BallBase>();

    if (ball.Score == false) return;

    GameManager.Instance.Scored(adversaryPaddle);
  }

  public void SetAdversaryPaddle(Paddle paddle)
  {
    adversaryPaddle = paddle;
  }
}
