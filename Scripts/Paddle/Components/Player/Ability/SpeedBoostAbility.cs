using Godot;
using System.Threading;

public partial class SpeedBoostAbility : AbilityComponent
{
  [Export] public float BoostMultiplier = 2.0f;
  [Export] public float BoostDuration = 2.0f;
  [Export] Color glowColor = new Color(0.3f, 0.8f, 1f);

  [Signal] public delegate void OnBoostStartedEventHandler(float duration);
  [Signal] public delegate void OnBoostEndedEventHandler();

  private bool isBoosting = false;
  private Tween glowTween = null;
  private GpuParticles2D trail;
  private CancellationTokenSource boostCts;

  public override void _Ready()
  {
    base._Ready();
    trail = CreateTrailParticles();
    paddle.CallDeferred("add_child", trail);
  }

  private GpuParticles2D CreateTrailParticles()
  {
    var material = new ParticleProcessMaterial();
    material.EmissionShape = ParticleProcessMaterial.EmissionShapeEnum.Box;
    material.EmissionBoxExtents = new Vector3(paddle.GetNode<Sprite2D>("Sprite2D").Texture.GetWidth() / 2f, 2f, 0f);
    material.Direction = new Vector3(-1, 0, 0);
    material.Spread = 10f;
    material.InitialVelocityMin = 20f;
    material.InitialVelocityMax = 60f;
    material.Gravity = Vector3.Zero;
    material.ScaleMin = 0.5f;
    material.ScaleMax = 1.2f;

    var gradient = new Gradient();
    gradient.SetColor(0, new Color(glowColor.R, glowColor.G, glowColor.B, 0.6f));
    gradient.SetColor(1, new Color(glowColor.R, glowColor.G, glowColor.B, 0f));
    var gradientTex = new GradientTexture1D();
    gradientTex.Gradient = gradient;
    material.ColorRamp = gradientTex;

    return new GpuParticles2D
    {
      ProcessMaterial = material,
      Amount = 20,
      Lifetime = 0.3f,
      Emitting = false,
      Explosiveness = 0f
    };
  }

  protected override void Activate()
  {
    if (isBoosting) return;
    _ = RunBoost();
  }

  private async System.Threading.Tasks.Task RunBoost()
  {
    boostCts = new CancellationTokenSource();
    var token = boostCts.Token;
    isBoosting = true;
    paddle.Speed *= BoostMultiplier;
    trail.Emitting = true;
    EmitSignal(SignalName.OnBoostStarted, BoostDuration);
    StartEmissiveGlow();

    await ToSignal(
        GetTree().CreateTimer(BoostDuration, true),
        SceneTreeTimer.SignalName.Timeout
    );

    if (token.IsCancellationRequested) return;

    StopBoostEffects();
    paddle.Speed /= BoostMultiplier;
    isBoosting = false;
    EmitSignal(SignalName.OnBoostEnded);
  }

  private void StartEmissiveGlow()
  {
    glowTween?.Kill();

    // Multiplica só RGB, mantém alpha em 1.0
    Color targetColor = new Color(
        glowColor.R * 2.5f,
        glowColor.G * 2.5f,
        glowColor.B * 2.5f,
        1.0f // alpha fixo
    );

    GD.Print($"Cor alvo corrigida: {targetColor}");

    glowTween = paddle.CreateTween();
    glowTween.SetTrans(Tween.TransitionType.Sine);
    glowTween.SetEase(Tween.EaseType.Out);
    glowTween.TweenProperty(paddle, "modulate", targetColor, 0.15f);
  }

  private void StopEmissiveGlow()
  {
    glowTween?.Kill();
    glowTween = null;

    Tween t = paddle.CreateTween();
    t.SetTrans(Tween.TransitionType.Sine);
    t.SetEase(Tween.EaseType.Out);
    t.TweenProperty(paddle, "modulate", Colors.White, 0.3f);
  }

  private void StopBoostEffects()
  {
    trail.Emitting = false;
    StopEmissiveGlow();
  }

  public override void ForceStop()
  {
    boostCts?.Cancel();
    isBoosting = false;
    paddle.Speed /= BoostMultiplier;
    StopBoostEffects();
  }
}