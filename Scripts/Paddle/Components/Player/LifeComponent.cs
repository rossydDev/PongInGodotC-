// LifeComponent.cs
using Godot;

public partial class LifeComponent : Node
{
  // Cena de explosão de partículas — mesma estrutura do BallExplosion mas vermelha
  [Export] PackedScene deathExplosionScene;
  [Export] float respawnDelay = 2f;

  [Signal] public delegate void OnDeathEventHandler();
  [Signal] public delegate void OnRespawnEventHandler();

  private Paddle paddle;
  private HealthComponent health;
  private Vector2 spawnPosition;
  private bool isDead = false;

  public override void _Ready()
  {
    paddle = GetParent<Paddle>();
    health = paddle.GetNode<HealthComponent>("HealthComponent");
    health.OnHealthChanged += OnHealthChanged;

    // Guarda a posição inicial como ponto de respawn
    spawnPosition = paddle.GlobalPosition;
  }

  // Pode ser atualizado pelo Camp se quiser um spawn position fixo
  public void SetSpawnPosition(Vector2 pos) => spawnPosition = pos;

  private void OnHealthChanged(float current, float max)
  {
    if (current <= 0f && !isDead)
      _ = DeathSequence();
  }

  private async System.Threading.Tasks.Task DeathSequence()
  {
    isDead = true;
    EmitSignal(SignalName.OnDeath);

    // Explosão de partículas vermelhas na posição atual
    SpawnDeathExplosion();

    // Esconde o paddle imediatamente
    paddle.Visible = false;
    paddle.SetPhysicsProcess(false);
    paddle.SetProcessUnhandledInput(false);

    // Aguarda o delay de respawn
    await ToSignal(
        GetTree().CreateTimer(respawnDelay, true),
        SceneTreeTimer.SignalName.Timeout
    );

    // Reseta e reposiciona
    health.Reset();
    paddle.GlobalPosition = spawnPosition;
    paddle.Visible = true;
    paddle.SetPhysicsProcess(true);
    paddle.SetProcessUnhandledInput(true);
    isDead = false;

    // Animação de spawn (a mesma do _Ready da bola)
    await SpawnAnimation();

    EmitSignal(SignalName.OnRespawn);
  }

  private void SpawnDeathExplosion()
  {
    if (deathExplosionScene == null) return;

    var explosion = deathExplosionScene.Instantiate<BallExplosion>();
    GetTree().Root.AddChild(explosion);
    explosion.Explode(paddle.GlobalPosition, new Color(1f, 0.15f, 0.15f), 2f);
  }

  private async System.Threading.Tasks.Task SpawnAnimation()
  {
    paddle.Scale = Vector2.Zero;
    paddle.Modulate = new Color(1, 1, 1, 0f);

    Tween tween = paddle.CreateTween();
    tween.SetParallel(true);
    tween.SetEase(Tween.EaseType.Out);
    tween.SetTrans(Tween.TransitionType.Elastic);
    tween.TweenProperty(paddle, "scale", Vector2.One, 0.6f).From(Vector2.Zero);
    tween.TweenProperty(paddle, "modulate:a", 1.0f, 0.2f).From(0f);

    await ToSignal(tween, Tween.SignalName.Finished);
  }
}