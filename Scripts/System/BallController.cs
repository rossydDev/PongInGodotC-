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
    if (GameManager.Instance.CurrentState == GameState.Start)
    {
      SpawnBall();
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

  public async void SpawnBall()
  {
    await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
    CreateBall();
  }
}