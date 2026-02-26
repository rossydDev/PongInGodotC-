using Godot;
using System;

public partial class Paddle : Actor
{
  [Export] private float speed = 50f;

  public void Move(Vector2 direction, float delta)
  {
    Vector2 nextPosition = Position + direction * speed * delta;

    nextPosition.Y = GameManager.Instance.ClampY(nextPosition.Y, spriteHeihgt);

    Position = nextPosition;
  }
}
