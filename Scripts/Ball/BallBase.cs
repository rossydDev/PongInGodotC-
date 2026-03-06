using Godot;
using System;
using System.Xml.Serialization;

public partial class BallBase : Actor
{
  [Export] protected float ballSpeed = 100f;
  [Export] protected Area2D area2D;

  // Preloads das cenas de efeito
  static readonly PackedScene ExplosionScene = GD.Load<PackedScene>("res://Scenes/Effects/ball_explosion.tscn");
  static readonly PackedScene WallPulseScene = GD.Load<PackedScene>("res://Scenes/Effects/wall_pulse.tscn");

  protected Vector2 direction = Vector2.Zero;
  public Vector2 Velocity => direction * ballSpeed;

  public override void _Ready()
  {
    base._Ready();

    SetPhysicsProcess(false);
    area2D.AreaEntered += OnAreaEntered;
    PickRandomOriginDirection();

    _ = SpawnAnimation();
  }

  public override void _PhysicsProcess(double delta)
  {
    Move((float)delta);
  }

  protected virtual void Move(float delta)
  {
    Rotation = direction.Angle();
    Position += direction * (ballSpeed * delta);

    if (GameManager.Instance.IsOutOfBounds(Position, spriteWidth))
    {
      OnBounceWall();
    }
  }

  private async System.Threading.Tasks.Task SpawnAnimation()
  {
    // Começa invisível e pequeno
    Scale = Vector2.Zero;
    Modulate = new Color(1, 1, 1, 0f);

    // Pequeno delay antes de aparecer
    await ToSignal(GetTree().CreateTimer(0.3f, true), SceneTreeTimer.SignalName.Timeout);

    // Aparece com overshoot elástico
    Tween tween = CreateTween();
    tween.SetParallel(true); // escala e alpha ao mesmo tempo
    tween.SetEase(Tween.EaseType.Out);
    tween.SetTrans(Tween.TransitionType.Elastic);

    tween.TweenProperty(this, "scale", Vector2.One, 0.6f).From(Vector2.Zero);
    tween.TweenProperty(this, "modulate:a", 1.0f, 0.2f).From(0f); // fade in mais rápido

    await ToSignal(tween, Tween.SignalName.Finished);

    // Só começa a se mover após a animação
    SetPhysicsProcess(true);
  }

  protected virtual void OnBounceWall()
  {
    direction.X *= -1;
    SpawnWallPulse();
  }

  protected virtual void OnBouncePaddle()
  {
    direction.Y *= -1;
  }

  protected virtual void OnAreaEntered(Area2D area)
  {
    var owner = area.Owner;

    if (owner is Paddle)
      OnBouncePaddle();
  }

  protected void PickRandomOriginDirection()
  {
    // 45°, 135°, 225°, 315° — sempre diagonal, nunca reto para parede
    float[] diagonalAngles =
    [
        Mathf.DegToRad(45f),
        Mathf.DegToRad(135f),
        Mathf.DegToRad(225f),
        Mathf.DegToRad(315f)
    ];

    float angle = diagonalAngles[(int)GD.RandRange(0, 3)];
    direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
  }

  // Chamado externamente pelo GameManager ao pontuar
  public void SpawnExplosion()
  {
    var explosion = ExplosionScene.Instantiate<BallExplosion>();
    GetTree().Root.AddChild(explosion);

    // Pega a cor do sprite — se usar modulate, pega ela
    Color ballColor = Colors.White;
    explosion.Explode(GlobalPosition, ballColor);
  }

  private void SpawnWallPulse()
  {
    var pulse = WallPulseScene.Instantiate<WallPulse>();
    GetTree().Root.AddChild(pulse);
    pulse.Pulse(GlobalPosition);
  }



}
