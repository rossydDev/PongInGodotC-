using Godot;
using System;

public partial class Ball : Actor
{
  [Export] private float ballSpeed = 25f;
  [Export] private Vector2 direction = Vector2.Left;
  [Export] private Area2D area2D;

  public override void _Ready()
  {
    base._Ready();

    area2D.AreaEntered += OnAreaEntered;
  }

  private void OnAreaEntered(Area2D area)
  {
    if (area.GetOwner<Paddle>() is not null)
    {
      PongTheBall(true);
    }
  }

  public override void _PhysicsProcess(double delta)
  {
    Move((float)delta);
  }

  private void Move(float delta)
  {
    Position += direction * (ballSpeed * delta);

    if (GameManager.Instance.IsOutOfBounds(Position, spriteHeihgt))
    {
      PongTheBall();
    }
  }

  private void PongTheBall(bool colliderWithPaddle = false)
  {
    if (colliderWithPaddle)
    {
      direction.X *= -1;
    }

    direction.Y *= -1;
  }

}
