using Godot;

public partial class Ball : Actor
{
  [Export] private float ballSpeed = 25f;
  [Export] private Vector2 direction = Vector2.Left;
  [Export] private Area2D area2D;

  private Vector2[] vectorsDirections = [
    new Vector2(1,1),
    new Vector2(1,-1),
    new Vector2(-1, 1),
    new Vector2 (-1, -1)
  ];

  public Vector2 Velocity => direction * ballSpeed;

  public override void _Ready()
  {
    base._Ready();

    area2D.AreaEntered += OnAreaEntered;
    PickRandomOriginDirection();
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

  private void PickRandomOriginDirection()
  {
    var index = (int)GD.Randi() % vectorsDirections.Length;

    direction = vectorsDirections[index];
  }

}
