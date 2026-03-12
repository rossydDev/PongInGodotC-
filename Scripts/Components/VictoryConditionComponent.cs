using Godot;

public partial class VictoryConditionComponent : Node
{
  [ExportGroup("Condição de Vitória")]
  [Export] private int scoreToWin = 3;

  public override void _Ready()
  {
    ScoreControll.Instance.ScoreUpdate += OnScoreUpdate;
  }

  private void OnScoreUpdate(int playerScore, int enemyScore)
  {
    if (playerScore >= scoreToWin)
    {
      GD.Print("Testando!");
      GameManager.Instance.SwitchState(GameState.PlayerWinPending);
      return;
    }

    if (enemyScore >= scoreToWin)
    {
      GameManager.Instance.SwitchState(GameState.EnemyWinPending);
    }
  }
}