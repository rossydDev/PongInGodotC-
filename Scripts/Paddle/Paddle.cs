using Godot;
using System;

public partial class Paddle : Actor
{
  [Export] private float speed = 50f;

  public void Move(Vector2 direction, float delta)
  {
    Vector2 nextPosition = Position + direction * speed * delta;

    nextPosition.Y = GameManager.Instance.ClampY(nextPosition.Y, spriteWidth);

    Position = nextPosition;
  }

  public void MoveTowards(float targetX, float delta)
  {
    float dir = Mathf.Sign(targetX - GlobalPosition.X);
    Position += new Vector2(dir, 0) * speed * delta;
  }

  public void SetX(float x)
  {
    Position = new Vector2(x, Position.Y);
  }
}
