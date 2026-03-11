using Godot;

/// <summary>
/// Substitui o AnimationPlayer do countdown.
/// Orquestra a sequência de intro de uma partida:
///   GameState.Intro → Diálogo (opcional) → Countdown → GameState.Start
///
/// Adicione como filho do Camp (dentro do HUD) e configure no Inspector.
/// O CountLabel existente na cena é reutilizado — só remova o AnimationPlayer.
/// </summary>
public partial class MatchIntroComponent : Node
{
  [ExportGroup("Referências")]
  [Export] private Label countLabel;

  [ExportGroup("Countdown")]
  [Export] private bool showCountdown = true;
  [Export] private int countFrom = 3;
  [Export] private float intervalBetweenCounts = 1f;

  [ExportGroup("Game Feel")]
  [Export] private float punchScale = 1.8f;
  [Export] private float punchDuration = 0.12f;
  [Export] private float shrinkDuration = 0.35f;
  [Export] private Color numberColor = new Color(1f, 1f, 1f);
  [Export] private Color goColor = new Color(0.2f, 1f, 0.4f);

  private bool introCompleted = false;

  public override void _Ready()
  {
    if (countLabel != null)
      countLabel.Visible = false;

    DialogueManager.Instance.OnDialogueFinished += OnDialogueFinished;
  }

  // Chamado pelo Camp quando GameState.Intro é emitido
  public void Begin()
  {
    if (DialogueManager.Instance.IsRunning)
      return; // OnDialogueFinished já está inscrito — vai capturar quando terminar

    StartCountdown();
  }

  private void OnDialogueFinished()
  {
    if (introCompleted) return;
    StartCountdown();
  }

  private async void StartCountdown()
  {
    introCompleted = true;
    if (!showCountdown)
    {
      GameManager.Instance.SwitchState(GameState.Start);
      return;
    }

    if (countLabel == null)
    {
      GameManager.Instance.SwitchState(GameState.Start);
      return;
    }

    countLabel.Visible = true;

    for (int i = countFrom; i >= 1; i--)
    {
      await AnimateCount(i.ToString(), numberColor);
      await ToSignal(
        GetTree().CreateTimer(intervalBetweenCounts - punchDuration - shrinkDuration, true),
        SceneTreeTimer.SignalName.Timeout
      );
    }

    await AnimateCount("Go!", goColor);

    await ToSignal(
      GetTree().CreateTimer(0.4f, true),
      SceneTreeTimer.SignalName.Timeout
    );

    countLabel.Visible = false;
    GameManager.Instance.SwitchState(GameState.Start);
  }

  private async System.Threading.Tasks.Task AnimateCount(string text, Color color)
  {
    countLabel.Text = text;
    countLabel.Modulate = color;
    countLabel.Scale = Vector2.Zero;
    countLabel.PivotOffset = countLabel.Size / 2f;

    // Punch: escala estoura rápido
    Tween punch = CreateTween();
    punch.SetEase(Tween.EaseType.Out);
    punch.SetTrans(Tween.TransitionType.Back);
    punch.TweenProperty(countLabel, "scale", Vector2.One * punchScale, punchDuration);
    await ToSignal(punch, Tween.SignalName.Finished);

    // Shrink: volta ao tamanho normal suavemente
    Tween shrink = CreateTween();
    shrink.SetEase(Tween.EaseType.In);
    shrink.SetTrans(Tween.TransitionType.Cubic);
    shrink.TweenProperty(countLabel, "scale", Vector2.One, shrinkDuration);
    await ToSignal(shrink, Tween.SignalName.Finished);
  }
}
