// PlayerController.cs
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
  [Export] private HealthComponent healthComponent;
  [Export] private PackedScene healEffectScene; // <- arrasta o HealEffect.tscn no Inspector

  public AbilityComponent CurrentAbility => ability;
  public LifeComponent LifeComponent => lifeComponent;
  public HealthComponent HealthComponent => healthComponent;

  public override void _Ready()
  {
    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    lifeComponent.OnDeath += OnPaddleDeath;

    if (lifeComponent != null)
    {
      lifeComponent.OnDeath += () => currentStatus = PlayerStatus.Idle;
      lifeComponent.OnRespawn += OnGameStateChanged;
    }

    healthComponent.OnHealed += OnHealed;
  }

  private void OnHealed(float amount, float current, float max)
  {
    if (healEffectScene == null) return;
    var effect = healEffectScene.Instantiate<HealEffect>();
    GetTree().Root.AddChild(effect);
    effect.Play(paddle);
  }

  private void OnPaddleDeath()
  {
    ability?.ForceStop();
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (@event.IsActionPressed("Ability") && currentStatus == PlayerStatus.Move)
      ability?.TryActivate();
  }

  private void OnGameStateChanged()
  {
    GameState currentGameState = GameManager.Instance.CurrentState;
    currentStatus = (currentGameState == GameState.Start || currentGameState == GameState.Scored)
        ? PlayerStatus.Move
        : PlayerStatus.Idle;
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