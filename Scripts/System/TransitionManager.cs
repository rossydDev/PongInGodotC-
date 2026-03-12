using Godot;
using System;
using System.Threading.Tasks;

public partial class TransitionManager : Node
{
  public static TransitionManager Instance { get; private set; }

  [Export] private ColorRect transitionEffect;
  [Export] private float transitionDuration = 0.5f;

  private ShaderMaterial shaderMaterial;

  public override void _Ready()
  {
    Instance = this;
    shaderMaterial = transitionEffect.Material as ShaderMaterial;
    transitionEffect.Visible = false;
  }

  public async Task PlayTransition(Action onMidpoint)
  {
    transitionEffect.Visible = true;
    shaderMaterial.SetShaderParameter("seed", GD.Randf());

    // Fase 1: 0.0 → 0.5 (cobre a tela)
    await TweenProgress(0f, 0.5f, transitionDuration);

    onMidpoint?.Invoke();

    // Fase 2: 0.5 → 1.0 (revela a nova cena)
    await TweenProgress(0.5f, 1f, transitionDuration);

    transitionEffect.Visible = false;
    SetProgress(0f);
  }

  private async Task TweenProgress(float from, float to, float duration)
  {
    var tween = CreateTween();
    tween.TweenMethod(
      Callable.From((float v) => SetProgress(v)),
      from, to, duration
    );
    await ToSignal(tween, Tween.SignalName.Finished);
  }

  private void SetProgress(float value)
  {
    float bg = Math.Abs(1f - value * 2f) - 0.5f;
    float col = Math.Min(1f, Math.Abs(-4f + value * 8f)) * 0.48f;

    shaderMaterial.SetShaderParameter("progress", value);
    shaderMaterial.SetShaderParameter("background_threshold", bg);
    shaderMaterial.SetShaderParameter("color_threshold", col);
  }
}