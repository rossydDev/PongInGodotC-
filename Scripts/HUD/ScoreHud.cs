using Godot;
using System;

public partial class ScoreHud : CanvasLayer
{
  [Export] Label playerScoreLabel;
  [Export] Label enemyScoreLabel;

  // Adicione dois ColorRect fullscreen no editor (cor branca, z_index alto)
  // e exporte aqui para o flash de tela
  [Export] ColorRect flashRect;

  [Signal] public delegate void OnScoreAnimationFinishedEventHandler();

  public override void _Ready()
  {
    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;

    playerScoreLabel.PivotOffset = playerScoreLabel.Size / 2;
    enemyScoreLabel.PivotOffset = enemyScoreLabel.Size / 2;
    playerScoreLabel.Scale = Vector2.Zero;
    enemyScoreLabel.Scale = Vector2.Zero;

    if (flashRect != null)
      flashRect.Modulate = new Color(1, 1, 1, 0);

    TreeExiting += () => GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
  }

  private async void OnGameStateChanged()
  {
    GameState currentState = GameManager.Instance.CurrentState;

    bool isScore = currentState == GameState.PlayerScore
                || currentState == GameState.EnemyScore;
    bool isVictory = currentState == GameState.PlayerWin
                  || currentState == GameState.PlayerLoser;

    if (!isScore && !isVictory) return;

    // Para vitória: quem marcou o ponto decisivo é sempre o "scorer"
    bool playerScored = currentState == GameState.PlayerScore
                     || currentState == GameState.PlayerWin;

    Label scorer = playerScored ? playerScoreLabel : enemyScoreLabel;
    Label loser = playerScored ? enemyScoreLabel : playerScoreLabel;

    MomentumPause();
    FlashScreen();

    await ToSignal(GetTree().CreateTimer(0.12f, true, false, true), SceneTreeTimer.SignalName.Timeout);
    if (!IsInstanceValid(this)) return;

    AnimateScore(scorer, scored: true);
    AnimateScore(loser, scored: false);
    ShakeLabel(loser);

    Tween tweenScorer = AnimateScore(scorer, scored: true);
    await ToSignal(tweenScorer, Tween.SignalName.Finished);

    await PulseLabel(scorer);

    EmitSignal(SignalName.OnScoreAnimationFinished);
  }

  private Tween AnimateScore(Label label, bool scored)
  {
    label.Visible = true;
    label.PivotOffset = label.Size / 2;

    Tween tween = CreateTween();
    tween.SetEase(Tween.EaseType.Out);
    tween.SetTrans(Tween.TransitionType.Back);

    float targetScale = scored ? 1.25f : 1.0f;

    tween.TweenProperty(label, "scale", Vector2.One * targetScale, 0.4f)
         .From(Vector2.Zero);

    if (scored)
    {
      tween.TweenProperty(label, "scale", Vector2.One * 1.25f, 0.15f)
           .From(Vector2.One * 1.4f);
      AnimateColor(label);
    }

    return tween;
  }

  private void AnimateColor(Label label)
  {
    Tween colorTween = CreateTween();
    colorTween.SetEase(Tween.EaseType.InOut);
    colorTween.SetTrans(Tween.TransitionType.Sine);
    colorTween.TweenProperty(label, "modulate", Colors.Yellow, 0.1f);
    colorTween.TweenProperty(label, "modulate", Colors.White, 0.4f);
  }

  // Escala rapidinho para cima e volta — sinal de "vai sumir"
  private async System.Threading.Tasks.Task PulseLabel(Label label)
  {
    Tween tween = CreateTween();
    tween.SetEase(Tween.EaseType.InOut);
    tween.SetTrans(Tween.TransitionType.Sine);
    tween.TweenProperty(label, "scale", Vector2.One * 1.5f, 0.12f);
    await ToSignal(tween, Tween.SignalName.Finished);
  }

  // Chacoalha o label horizontalmente
  private async void ShakeLabel(Label label)
  {
    Vector2 origin = label.Position;
    Tween tween = CreateTween();
    tween.SetLoops(5);
    tween.TweenProperty(label, "position", origin + new Vector2(10, 0), 0.04f);
    tween.TweenProperty(label, "position", origin - new Vector2(10, 0), 0.04f);
    await ToSignal(tween, Tween.SignalName.Finished);
    if (!IsInstanceValid(this)) return;
    label.Position = origin; // garante volta à posição original
  }

  // Flash branco de impacto
  private void FlashScreen()
  {
    if (flashRect == null) return;
    Tween tween = CreateTween();
    tween.TweenProperty(flashRect, "modulate:a", 0.55f, 0.04f).From(0f);
    tween.TweenProperty(flashRect, "modulate:a", 0.0f, 0.35f);
  }

  // Slow motion por um instante
  private async void MomentumPause()
  {
    Engine.TimeScale = 0.08f;
    await ToSignal(GetTree().CreateTimer(0.1f, true, false, true), SceneTreeTimer.SignalName.Timeout);

    if (!IsInstanceValid(this))
    {
      Engine.TimeScale = 1.0f; // garante que o TimeScale volta mesmo se o nó morreu
      return;
    }

    Tween tween = CreateTween();
    tween.TweenProperty(Engine.Singleton, "time_scale", 1.0f, 0.2f);
  }

  public void SetScore(int playerScore, int enemyScore)
  {
    playerScoreLabel.Text = playerScore.ToString();
    enemyScoreLabel.Text = enemyScore.ToString();
  }

  public void HideScore()
  {
    playerScoreLabel.Visible = false;
    enemyScoreLabel.Visible = false;
  }
}