using Godot;

public partial class PaddleIA : Paddle
{
  [Export] private PaddleStateController controller;

  public PaddleStateController Controller => controller;
}