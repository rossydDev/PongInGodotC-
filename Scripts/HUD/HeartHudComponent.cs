using Godot;

public partial class HeartHudComponent : HBoxContainer
{
  [Export] Texture2D heartFull;
  [Export] Texture2D heartHalf;
  [Export] Texture2D heartEmpty;

  const float HideDelay = 2f;

  private HealthComponent healthComponent;
  private int totalHeartSlots;
  private Tween hideTween;
  private SceneTreeTimer hideTimer;

  public override void _Ready()
  {
    Modulate = new Color(1, 1, 1, 0f); // começa invisível
  }

  public void Initializer(Paddle player)
  {
    healthComponent = player.GetNode<HealthComponent>("HealthComponent");
    totalHeartSlots = (int)healthComponent.MaxHearts;
    healthComponent.OnHealthChanged += Refresh;
    BuildHearts();
    Refresh(healthComponent.CurrentHearts, healthComponent.MaxHearts);

    // Conecta o sinal do boost para mostrar a HUD automaticamente
    var ability = player.GetNodeOrNull<SpeedBoostAbility>("SpeedBoostAbility");
    ability?.Connect(
        SpeedBoostAbility.SignalName.OnBoostStarted,
        Callable.From((float _) => ShowTemporarily())
    );
  }

  public void ShowTemporarily()
  {
    // Cancela tween de fade out se estava rodando
    hideTween?.Kill();

    // Cancela o timer anterior — desconecta o sinal em vez de reconectar
    if (hideTimer != null)
    {
      if (hideTimer.IsConnected(SceneTreeTimer.SignalName.Timeout, Callable.From(FadeOut)))
        hideTimer.Disconnect(SceneTreeTimer.SignalName.Timeout, Callable.From(FadeOut));
    }

    // Fade in
    Tween tween = CreateTween();
    tween.SetTrans(Tween.TransitionType.Sine);
    tween.TweenProperty(this, "modulate:a", 1f, 0.2f);

    // Agenda novo fade out
    hideTimer = GetTree().CreateTimer(HideDelay, true);
    hideTimer.Connect(SceneTreeTimer.SignalName.Timeout, Callable.From(FadeOut));
  }

  private void FadeOut()
  {
    hideTween = CreateTween();
    hideTween.SetTrans(Tween.TransitionType.Sine);
    hideTween.TweenProperty(this, "modulate:a", 0f, 0.4f);
  }

  private void BuildHearts()
  {
    foreach (Node child in GetChildren())
      child.QueueFree();

    for (int i = 0; i < totalHeartSlots; i++)
    {
      var icon = new TextureRect();
      icon.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
      icon.CustomMinimumSize = new Vector2(32, 32);
      AddChild(icon);
    }
  }

  private void Refresh(float current, float max)
  {
    var slots = GetChildren();
    for (int i = 0; i < slots.Count; i++)
    {
      if (slots[i] is not TextureRect icon) continue;

      float value = current - i;

      if (value >= 1f) icon.Texture = heartFull;
      else if (value >= 0.5f) icon.Texture = heartHalf;
      else icon.Texture = heartEmpty;

      if (value < 1f && value != 0f)
        AnimateHeartLoss(icon);
    }
  }

  private void AnimateHeartLoss(TextureRect icon)
  {
    Tween tween = CreateTween();
    tween.SetTrans(Tween.TransitionType.Back);
    tween.SetEase(Tween.EaseType.Out);
    tween.TweenProperty(icon, "scale", Vector2.One * 1.3f, 0.1f).From(Vector2.One);
    tween.TweenProperty(icon, "scale", Vector2.One, 0.15f);
  }

  public void ConnectAbility(AbilityComponent ability)
  {
    ability.Connect(
        AbilityComponent.SignalName.OnAbilityActivated,
        Callable.From(ShowTemporarily)
    );
  }
}