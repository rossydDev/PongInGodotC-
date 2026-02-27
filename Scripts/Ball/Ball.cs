using Godot;
using System;

public partial class Ball : Actor
{
  [Export] private float ballSpeed = 25f;
  [Export] private Vector2 direction = Vector2.Left;
  [Export] private Area2D area2D;

  public Vector2 Velocity => direction * ballSpeed;

  public override void _Ready()
  {
    base._Ready();

    GameManager.Instance.SetCurrentBall(this);

    area2D.AreaEntered += OnAreaEntered;
  }

  private void OnAreaEntered(Area2D area)
  {
    if (area.GetOwner<Paddle>() is not null)
    {
      PongTheBallInPaddle();
    }
  }

  public override void _PhysicsProcess(double delta)
  {
    Move((float)delta);
  }

  private void Move(float delta)
  {
    Position += direction * (ballSpeed * delta);

    if (GameManager.Instance.IsOutOfBounds(Position, spriteWidth))
    {
      PongTheBall();
    }
  }

  private void PongTheBall()
  {
    direction.X *= -1;
  }

  private void PongTheBallInPaddle()
  {
    direction.Y *= -1;
  }

}
