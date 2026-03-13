using Godot;
using System.Threading.Tasks;

public partial class GameOverScreen : Control
{
  [ExportGroup("Referências")]
  [Export] private Control panel;
  [Export] private ColorRect dimBg;
  [Export] private Label titleLabel;
  [Export] private Label playerScoreLabel;
  [Export] private Label enemyScoreLabel;
  [Export] private Button retryButton;
  [Export] private Button quitButton;

  [ExportGroup("Animação")]
  [Export] private float dimDuration = 0.5f;
  [Export] private float panelDropDuration = 0.45f;
  [Export] private float titlePunchDuration = 0.18f;
  [Export] private float scoreCountDuration = 0.7f;
  [Export] private float buttonStagger = 0.15f;

  [Signal] public delegate void OnRetryEventHandler();
  [Signal] public delegate void OnQuitEventHandler();

  private Vector2 _panelRestPosition;

  public override void _Ready()
  {
    Hide();
    retryButton.Pressed += OnRetryPressed;
    quitButton.Pressed += OnQuitPressed;
  }

  // Chamado pela Main após animação do ScoreHud
  public async void ShowGameOver()
  {
    int playerScore = ScoreControll.Instance.PlayerScore;
    int enemyScore = ScoreControll.Instance.EnemyScore;

    PrepareForAnimation();
    Show();

    await FadeInDim();
    await DropPanel();
    await PunchTitle(new Color(0.9f, 0.15f, 0.15f)); // vermelho
    await ShakeTitle();
    await CountScores(playerScore, enemyScore);
    await RevealButton(retryButton);
    await ToSignal(GetTree().CreateTimer(buttonStagger), SceneTreeTimer.SignalName.Timeout);
    await RevealButton(quitButton);
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
      panel.Position = _panelRestPosition - new Vector2(0, 100); // vem de cima
      panel.Modulate = new Color(1, 1, 1, 0);
    }

    if (titleLabel != null)
    {
      titleLabel.Scale = Vector2.Zero;
      titleLabel.PivotOffset = titleLabel.Size / 2f;
    }

    retryButton.Modulate = new Color(1, 1, 1, 0);
    retryButton.Position += new Vector2(0, 20);
    quitButton.Modulate = new Color(1, 1, 1, 0);
    quitButton.Position += new Vector2(0, 20);
  }

  private async Task FadeInDim()
  {
    if (dimBg == null) return;
    var t = CreateTween();
    t.SetTrans(Tween.TransitionType.Cubic);
    t.TweenProperty(dimBg, "modulate:a", 1f, dimDuration);
    await ToSignal(t, Tween.SignalName.Finished);
  }

  private async Task DropPanel()
  {
    if (panel == null) return;
    var t = CreateTween().SetParallel();
    t.TweenProperty(panel, "position", _panelRestPosition, panelDropDuration)
     .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Bounce);
    t.TweenProperty(panel, "modulate:a", 1f, panelDropDuration * 0.5f);
    await ToSignal(t, Tween.SignalName.Finished);
  }

  private async Task PunchTitle(Color color)
  {
    if (titleLabel == null) return;

    titleLabel.AddThemeColorOverride("font_color", color);

    var expand = CreateTween();
    expand.SetEase(Tween.EaseType.Out);
    expand.SetTrans(Tween.TransitionType.Back);
    expand.TweenProperty(titleLabel, "scale", Vector2.One * 1.4f, titlePunchDuration);
    await ToSignal(expand, Tween.SignalName.Finished);

    var settle = CreateTween();
    settle.SetEase(Tween.EaseType.Out);
    settle.SetTrans(Tween.TransitionType.Cubic);
    settle.TweenProperty(titleLabel, "scale", Vector2.One, 0.15f);
    await ToSignal(settle, Tween.SignalName.Finished);
  }

  private async Task ShakeTitle()
  {
    if (titleLabel == null) return;

    Vector2 origin = titleLabel.Position;
    var t = CreateTween();
    t.SetLoops(4);
    t.TweenProperty(titleLabel, "position", origin + new Vector2(8, 0), 0.04f);
    t.TweenProperty(titleLabel, "position", origin - new Vector2(8, 0), 0.04f);
    await ToSignal(t, Tween.SignalName.Finished);
    titleLabel.Position = origin;
  }

  private async Task CountScores(int playerScore, int enemyScore)
  {
    var playerTask = CountLabel(playerScoreLabel, playerScore, scoreCountDuration * 0.7f);
    await ToSignal(GetTree().CreateTimer(0.12f), SceneTreeTimer.SignalName.Timeout);
    var enemyTask = CountLabel(enemyScoreLabel, enemyScore, scoreCountDuration);

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
    t.TweenProperty(button, "modulate:a", 1f, 0.25f);
    t.TweenProperty(button, "position", button.Position - new Vector2(0, 20), 0.3f)
     .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);
    await ToSignal(t, Tween.SignalName.Finished);
  }

  private void OnRetryPressed()
  {
    Hide();
    EmitSignal(SignalName.OnRetry);
  }

  private void OnQuitPressed()
  {
    Hide();
    EmitSignal(SignalName.OnQuit);
  }
}