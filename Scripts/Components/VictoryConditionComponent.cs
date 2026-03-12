using Godot;
using System;

/// <summary>
/// Intercepta PlayerScore e EnemyScore para verificar condição de vitória.
/// Priority=10 — roda depois do DialogueTriggerComponent (Priority=0).
///
/// Fluxo:
///   RequestStateChange(PlayerScore)
///     → DialogueTrigger intercepta se tiver diálogo de score → confirma
///     → VictoryCondition: score >= scoreToWin?
///         sim → RequestStateChange(PlayerWin) — passa pelo pipeline de novo
///         não → confirma PlayerScore normalmente
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
      // +1 antecipado para avaliar APÓS o score ser incrementado
      int scoreAfter = ScoreControll.Instance.PlayerScore + 1;

      if (scoreAfter >= scoreToWin)
      {
        // Confirma o score primeiro, depois pede vitória
        confirm();
        GameManager.Instance.RequestStateChange(GameState.PlayerWin);
        return;
      }
    }
    else if (requestedState == GameState.EnemyScore)
    {
      int scoreAfter = ScoreControll.Instance.EnemyScore + 1;

      if (scoreAfter >= scoreToWin)
      {
        confirm();
        GameManager.Instance.RequestStateChange(GameState.PlayerLoser);
        return;
      }
    }

    // Score não atingiu o limite — deixa a transição continuar normalmente
    confirm();
  }
}