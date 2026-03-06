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
  [Export] private AbilityComponent ability;
  [Export] private LifeComponent lifeComponent;

  public AbilityComponent CurrentAbility => ability;

  public override void _Ready()
  {
    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    lifeComponent.OnDeath += OnPaddleDeath;

    var life = paddle.GetNodeOrNull<LifeComponent>("LifeComponent");
    if (life != null)
    {
      life.OnDeath += () => currentStatus = PlayerStatus.Idle;
      life.OnRespawn += OnGameStateChanged;
    }
  }

  private void OnPaddleDeath()
  {
    ability?.ForceStop();
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (@event.IsActionPressed("Ability") && currentStatus == PlayerStatus.Move)
    {
      ability?.TryActivate();
    }
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