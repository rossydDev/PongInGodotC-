using Godot;

public partial class PlayerController : Node
{
  [Export] private Paddle paddle;

  public override void _PhysicsProcess(double delta)
  {
    if (paddle == null) return;

    Vector2 direction = Vector2.Zero;

    direction.X = Input.GetAxis("MoveLeft", "MoveRight");

    paddle.Move(direction, (float)delta);
  }
}