using System;
using Godot;

public partial class BallController : Node
{
  public static BallController Instance { get; private set; }

  [Export] private Node2D spawnPosition;
  [Export] private PackedScene ballScene;

  private BallBase currentBall;

  public BallBase CurrentBall => currentBall;

  public override void _Ready()
  {
    Instance = this;

    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
  }

  private void OnGameStateChanged()
  {
    GameState currentState = GameManager.Instance.CurrentState;

    if (currentState == GameState.Start || currentState == GameState.Scored)
    {
      CreateBall();
    }
  }

  private void CreateBall()
  {
    if (currentBall != null)
    {
      currentBall.QueueFree();
      currentBall = null;
    }

    BallBase ball = ballScene.Instantiate<BallBase>();

    AddChild(ball);

    ball.GlobalPosition = spawnPosition.GlobalPosition;

    currentBall = ball;
  }
}