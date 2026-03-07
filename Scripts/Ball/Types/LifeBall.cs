using Godot;

public partial class LifeBall : BallBase
{
  [Export] private float lifetime = 8f;
  [Export] private float fadeWarningAt = 3f;

  private float _timer;
  private bool _isFading;

  public override void _Ready()
  {
    base._Ready();
    _timer = lifetime;
    score = false;
  }

  public override void _PhysicsProcess(double delta)
  {
    base._PhysicsProcess(delta);

    _timer -= (float)delta;

    if (!_isFading && _timer <= fadeWarningAt)
    {
      _isFading = true;
      StartFadeWarning();
    }

    if (_timer <= 0f)
    {
      SpawnExplosion();
      QueueFree();
    }
  }

  // Sobrescreve o Move para quicar também no eixo Y (topo e base da tela)
  protected override void Move(float delta)
  {
    Rotation = direction.Angle();
    Position += direction * (ballSpeed * delta);

    var rect = GetViewportRect();

    // Bounce nas paredes laterais (herdado da lógica do BallBase via IsOutOfBounds)
    if (GameManager.Instance.IsOutOfBounds(Position, spriteWidth))
    {
      direction.X *= -1;
      SpawnWallPulse();
    }

    // Bounce no topo e base da tela
    if (Position.Y - spriteWidth < rect.Position.Y)
    {
      Position = new Vector2(Position.X, rect.Position.Y + spriteWidth);
      direction.Y *= -1;
      SpawnWallPulse();
    }
    else if (Position.Y + spriteWidth > rect.End.Y)
    {
      Position = new Vector2(Position.X, rect.End.Y - spriteWidth);
      direction.Y *= -1;
      SpawnWallPulse();
    }
  }

  protected override void OnAreaEntered(Area2D area)
  {
    var owner = area.Owner;
    if (owner is Paddle paddle)
    {
      // Só cura se for o player
      var playerController = paddle.GetNodeOrNull<PlayerController>("PlayerController");
      if (playerController == null) return;

      playerController.HealthComponent.Heal(1);
      SpawnExplosion();
      QueueFree();
    }
  }

  private void StartFadeWarning()
  {
    // Pisca para avisar que vai sumir
    Tween tween = CreateTween();
    tween.SetLoops();
    tween.TweenProperty(this, "modulate:a", 0.2f, 0.3f);
    tween.TweenProperty(this, "modulate:a", 1.0f, 0.3f);
  }
}