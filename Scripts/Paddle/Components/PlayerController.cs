using Godot;

public partial class PlayerController : Node
{
  [Export] private Paddle paddle;

  public override void _PhysicsProcess(double delta)
  {
    if (paddle == null) return;

    Vector2 direction = Vector2.Zero;

    direction.Y = Input.GetAxis("MoveUp", "MoveDown");

    paddle.Move(direction, (float)delta);
  }
}