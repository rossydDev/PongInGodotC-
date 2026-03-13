using Godot;
using System;

/// <summary>
/// Intercepta PlayerScore e EnemyScore para verificar condição de vitória.
/// Priority=10 — roda depois do DialogueTriggerComponent (Priority=0).
///
/// Quando o score atinge o limite, NÃO confirma o estado de score —
/// redireciona direto para PlayerWin ou PlayerLoser.
/// O ScoreControll incrementa ao ouvir PlayerWin/PlayerLoser.
/// </summary>
public partial class VictoryConditionComponent : Node, IStateInterceptor
{
  [ExportGroup("Condição de Vitória")]
  [Export] private int scoreToWin = 3;

  public int Priority => 10;

  public override void _Ready()
  {
    GameManager.Instance.RegisterInterceptor(this);
    TreeExiting += () => GameManager.Instance.UnregisterInterceptor(this);
  }

  public bool CanIntercept(GameState requestedState)
  {
    return requestedState == GameState.PlayerScore
        || requestedState == GameState.EnemyScore;
  }

  public void Intercept(GameState requestedState, Action confirm)
  {
    if (requestedState == GameState.PlayerScore)
    {
      int scoreAfter = ScoreControll.Instance.PlayerScore + 1;
      if (scoreAfter >= scoreToWin)
      {
        // Sem confirm() — PlayerScore não é confirmado
        GameManager.Instance.RequestStateChange(GameState.PlayerWin);
        return;
      }
    }
    else if (requestedState == GameState.EnemyScore)
    {
      int scoreAfter = ScoreControll.Instance.EnemyScore + 1;
      if (scoreAfter >= scoreToWin)
      {
        // Sem confirm() — EnemyScore não é confirmado
        GameManager.Instance.RequestStateChange(GameState.PlayerLoser);
        return;
      }
    }

    confirm();
  }
}