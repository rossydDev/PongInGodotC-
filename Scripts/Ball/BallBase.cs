using Godot;
using System;
using System.Xml.Serialization;

public partial class BallBase : Actor
{
  [Export] protected float ballSpeed = 100f;
  [Export] protected Area2D area2D;

  protected Vector2 direction = Vector2.Zero;
  public Vector2 Velocity => direction * ballSpeed;

  public override void _Ready()
  {
    base._Ready();

    area2D.AreaEntered += OnAreaEntered;
    PickRandomOriginDirection();
  }

  public override void _PhysicsProcess(double delta)
  {
    Move((float)delta);
  }

  protected virtual void Move(float delta)
  {
    Rotation = direction.Angle();
    Position += direction * (ballSpeed * delta);

    if (GameManager.Instance.IsOutOfBounds(Position, spriteWidth))
    {
      OnBounceWall();
    }
  }

  protected virtual void OnBounceWall()
  {
    direction.X *= -1;
  }

  protected virtual void OnBouncePaddle()
  {
    direction.Y *= -1;
  }

  protected virtual void OnAreaEntered(Area2D area)
  {
    var owner = area.Owner;

    if (owner is Paddle)
      OnBouncePaddle();
  }

  protected void PickRandomOriginDirection()
  {
    // 45°, 135°, 225°, 315° — sempre diagonal, nunca reto para parede
    float[] diagonalAngles =
    [
        Mathf.DegToRad(45f),
        Mathf.DegToRad(135f),
        Mathf.DegToRad(225f),
        Mathf.DegToRad(315f)
    ];

    float angle = diagonalAngles[(int)GD.RandRange(0, 3)];
    direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
  }


}
