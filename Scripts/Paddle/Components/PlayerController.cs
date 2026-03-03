using System;
using System.Diagnostics;
using Godot;

public enum PlayerStatus
{
  Idle,
  Move
}

public partial class PlayerController : Node
{
  private PlayerStatus currentStatus = PlayerStatus.Idle;
  [Export] private Paddle paddle;

  public override void _Ready()
  {
    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
  }

  private void OnGameStateChanged()
  {
    GameState currentGameState = GameManager.Instance.CurrentState;

    if (currentGameState == GameState.Start || currentGameState == GameState.Scored)
    {
      currentStatus = PlayerStatus.Move;
    }
    else
    {
      currentStatus = PlayerStatus.Idle;
    }
  }

  public override void _PhysicsProcess(double delta)
  {
    if (paddle == null) return;

    switch (currentStatus)
    {
      case PlayerStatus.Idle:
        float center = GameManager.Instance.ScreenWidth / 2f;
        paddle.MoveTowards(center, (float)delta);
        break;
      case PlayerStatus.Move:
        Move((float)delta);
        break;
    }
  }

  private void Move(float delta)
  {
    Vector2 direction = Vector2.Zero;

    direction.X = Input.GetAxis("MoveLeft", "MoveRight");

    paddle.Move(direction, delta);
  }
}