using Godot;
using System.Threading.Tasks;

/// <summary>
/// Tela de vitória com animação sequencial:
/// 1. DimBg faz fade in
/// 2. Painel sobe de baixo com overshoot (Back easing)
/// 3. Título faz punch (escala 0 → grande → normal) com cor dourada
/// 4. Scores contam progressivamente com mini-punch a cada número
/// 5. Botão aparece com fade + slide vindo de baixo
/// </summary>
public partial class VictoryScreen : Control
{
  [ExportGroup("Referências")]
  [Export] private Control panel;
  [Export] private ColorRect dimBg;
  [Export] private Label titleLabel;
  [Export] private Label playerScoreLabel;
  [Export] private Label enemyScoreLabel;
  [Export] private Button continueButton;

  [ExportGroup("Animação")]
  [Export] private float dimDuration = 0.35f;
  [Export] private float panelRiseDuration = 0.5f;
  [Export] private float titlePunchDuration = 0.2f;
  [Export] private float scoreCountDuration = 0.7f;
  [Export] private float buttonDelay = 0.15f;

  [Signal] public delegate void OnContinueEventHandler();

  private Vector2 _panelRestPosition;

  public override void _Ready()
  {
    Hide();
    continueButton.Pressed += OnContinuePressed;
  }

  public async void ShowVictory()
  {
    int playerScore = ScoreControll.Instance.PlayerScore;
    int enemyScore = ScoreControll.Instance.EnemyScore;

    PrepareForAnimation();
    Show();

    await FadeInDim();
    await RaisePanel();
    await PunchTitle(new Color(1f, 0.85f, 0.1f));
    await CountScores(playerScore, enemyScore);
    await ToSignal(GetTree().CreateTimer(buttonDelay), SceneTreeTimer.SignalName.Timeout);
    await RevealButton(continueButton);
  }

  // ── Etapas de animação ────────────────────────────────────────────────────

  private void PrepareForAnimation()
  {
    playerScoreLabel.Text = "0";
    enemyScoreLabel.Text = "0";

    if (dimBg != null)
      dimBg.Modulate = new Color(1, 1, 1, 0);

    if (panel != null)
    {
      _panelRestPosition = panel.Position;
      panel.Position = _panelRestPosition + new Vector2(0, 80);
      panel.Modulate = new Color(1, 1, 1, 0);
    }

    if (titleLabel != null)
    {
      titleLabel.Scale = Vector2.Zero;
      titleLabel.PivotOffset = titleLabel.Size / 2f;
    }

    continueButton.Modulate = new Color(1, 1, 1, 0);
    continueButton.Position += new Vector2(0, 20);
  }

  private async Task FadeInDim()
  {
    if (dimBg == null) return;
    var t = CreateTween();
    t.TweenProperty(dimBg, "modulate:a", 1f, dimDuration);
    await ToSignal(t, Tween.SignalName.Finished);
  }

  private async Task RaisePanel()
  {
    if (panel == null) return;
    var t = CreateTween().SetParallel();
    t.TweenProperty(panel, "position", _panelRestPosition, panelRiseDuration)
     .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);
    t.TweenProperty(panel, "modulate:a", 1f, panelRiseDuration * 0.6f);
    await ToSignal(t, Tween.SignalName.Finished);
  }

  private async Task PunchTitle(Color color)
  {
    if (titleLabel == null) return;

    titleLabel.AddThemeColorOverride("font_color", color);

    var expand = CreateTween();
    expand.SetEase(Tween.EaseType.Out);
    expand.SetTrans(Tween.TransitionType.Back);
    expand.TweenProperty(titleLabel, "scale", Vector2.One * 1.35f, titlePunchDuration);
    await ToSignal(expand, Tween.SignalName.Finished);

    var settle = CreateTween();
    settle.SetEase(Tween.EaseType.Out);
    settle.SetTrans(Tween.TransitionType.Elastic);
    settle.TweenProperty(titleLabel, "scale", Vector2.One, 0.35f);
    await ToSignal(settle, Tween.SignalName.Finished);
  }

  private async Task CountScores(int playerScore, int enemyScore)
  {
    var playerTask = CountLabel(playerScoreLabel, playerScore, scoreCountDuration);
    await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
    var enemyTask = CountLabel(enemyScoreLabel, enemyScore, scoreCountDuration * 0.7f);
    await playerTask;
    await enemyTask;
  }

  private async Task CountLabel(Label label, int target, float duration)
  {
    float interval = target > 0 ? duration / target : 0f;

    for (int i = 0; i <= target; i++)
    {
      label.Text = i.ToString();
      label.Scale = Vector2.One * 1.4f;
      label.PivotOffset = label.Size / 2f;

      var punch = CreateTween();
      punch.SetEase(Tween.EaseType.Out);
      punch.SetTrans(Tween.TransitionType.Back);
      punch.TweenProperty(label, "scale", Vector2.One, interval > 0 ? interval * 0.9f : 0.1f);

      if (i < target)
        await ToSignal(GetTree().CreateTimer(interval), SceneTreeTimer.SignalName.Timeout);
    }
  }

  private async Task RevealButton(Button button)
  {
    var t = CreateTween().SetParallel();
    t.TweenProperty(button, "modulate:a", 1f, 0.3f);
    t.TweenProperty(button, "position", button.Position - new Vector2(0, 20), 0.35f)
     .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);
    await ToSignal(t, Tween.SignalName.Finished);
  }

  private void OnContinuePressed()
  {
    Hide();
    EmitSignal(SignalName.OnContinue);
  }
}