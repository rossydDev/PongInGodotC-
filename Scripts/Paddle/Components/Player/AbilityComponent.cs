using Godot;

public abstract partial class AbilityComponent : Node
{
  [Export] public float HeartCost = 0.5f;
  [Export] public float Cooldown = 0f;

  [Signal] public delegate void OnAbilityActivatedEventHandler();

  protected HealthComponent health;
  protected Paddle paddle;

  private float cooldownTimer = 0f;
  public bool IsReady => cooldownTimer <= 0f;

  public override void _Ready()
  {
    paddle = GetParent<Paddle>();
    health = paddle.GetNode<HealthComponent>("HealthComponent");
  }

  public override void _Process(double delta)
  {
    if (cooldownTimer < 0f)
    {
      cooldownTimer -= (float)delta;
    }
  }

  public void TryActivate()
  {
    if (!IsReady) return;
    if (!health.TrySpend(HeartCost)) return;

    Activate();
    cooldownTimer = Cooldown;
    EmitSignal(SignalName.OnAbilityActivated);
  }

  protected abstract void Activate();
  public abstract void ForceStop();
}