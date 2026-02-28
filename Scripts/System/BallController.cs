using System;
using Godot;

public partial class BallController : Node
{
  public static BallController Instance { get; private set; }

  [Export] private Node2D spawnPosition;
  [Export] private PackedScene ballScene;

  private Ball currentBall;

  public Ball CurrentBall => currentBall;

  public override void _Ready()
  {
    Instance = this;

    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
  }

  private void OnGameStateChanged()
  {
    if (GameManager.Instance.CurrentState == GameState.Start)
    {
      GD.Print("Testando!");
      SpawnBall();
    }
  }

  private void SpawnBall()
  {
    if (currentBall != null)
    {
      currentBall.QueueFree();
      currentBall = null;
    }

    Ball ball = ballScene.Instantiate<Ball>();

    AddChild(ball);

    ball.GlobalPosition = spawnPosition.GlobalPosition;

    currentBall = ball;
  }
}