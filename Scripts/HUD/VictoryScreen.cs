using Godot;

public partial class VictoryScreen : Control
{
  [Export] private Label playerScoreLabel;
  [Export] private Label enemyScoreLabel;
  [Export] private Control abilityPanel;
  [Export] private Label abilityNameLabel;
  [Export] private TextureRect abilityIcon;
  [Export] private Button continueButton;

  [Signal] public delegate void OnContinueEventHandler();

  public override void _Ready()
  {
    Hide();
    continueButton.Pressed += OnContinuePressed;
    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
  }

  private void OnGameStateChanged()
  {
    if (GameManager.Instance.CurrentState != GameState.PlayerWin) return;
    ShowVictory();
  }

  private void ShowVictory()
  {
    playerScoreLabel.Text = ScoreControll.Instance.PlayerScore.ToString();
    enemyScoreLabel.Text = ScoreControll.Instance.EnemyScore.ToString();
    abilityPanel.Visible = false;
    Show();
  }

  public void SetAbility(string name, Texture2D icon)
  {
    if (abilityPanel == null) return;
    abilityNameLabel.Text = name;
    abilityIcon.Texture = icon;
    abilityPanel.Visible = true;
  }

  private void OnContinuePressed()
  {
    Hide();
    EmitSignal(SignalName.OnContinue);
  }
}