using Godot;
using System.Collections.Generic;

/// <summary>
/// Componente declarativo que define todos os gatilhos de diálogo de um campo.
/// Adicione como filho do Camp e preencha os [Export] no Inspector.
/// Escuta GameManager e ScoreControll por conta própria — ninguém precisa chamá-lo.
///
/// Gatilhos disponíveis:
///   - Início da partida
///   - Vitória do player / do inimigo
///   - Score do player ou inimigo bater um valor específico
/// </summary>
public partial class DialogueTriggerComponent : Node
{
  [ExportGroup("Gatilhos de Estado")]
  [Export] private DialogueSequence onMatchStart;
  [Export] private DialogueSequence onPlayerWin;
  [Export] private DialogueSequence onEnemyWin;

  [ExportGroup("Gatilhos de Score")]
  [Export] private DialogueScoreTrigger[] onPlayerScore;
  [Export] private DialogueScoreTrigger[] onEnemyScore;

  [ExportGroup("Dependências")]
  [Export] private ScoreControll scoreControll;

  // Controle de gatilhos já disparados nesta partida
  private readonly HashSet<int> firedPlayerScoreTriggers = new();
  private readonly HashSet<int> firedEnemyScoreTriggers = new();

  public override void _Ready()
  {
    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
  }

  // Chamado pelo CampInitializer após o SetupScore — garante que o ScoreControll já foi inicializado
  public void Initialize()
  {
    if (scoreControll != null)
      scoreControll.ScoreUpdate += OnScoreUpdate;
  }

  private void OnGameStateChanged()
  {
    switch (GameManager.Instance.CurrentState)
    {
      case GameState.CampReady:
        TryTrigger(onMatchStart);
        break;

      case GameState.PlayerWin:
        TryTrigger(onPlayerWin);
        break;

      case GameState.PlayerLoser:
        TryTrigger(onEnemyWin);
        break;

      // Nova partida — reseta gatilhos de score
      case GameState.Scored:
        break;
    }
  }

  private void OnScoreUpdate(int playerScore, int enemyScore)
  {
    CheckScoreTriggers(onPlayerScore, playerScore, firedPlayerScoreTriggers);
    CheckScoreTriggers(onEnemyScore, enemyScore, firedEnemyScoreTriggers);
  }

  private void CheckScoreTriggers(
    DialogueScoreTrigger[] triggers,
    int currentScore,
    HashSet<int> fired)
  {
    if (triggers == null) return;

    foreach (var trigger in triggers)
    {
      if (trigger?.Dialogue == null) continue;
      if (fired.Contains(trigger.AtScore)) continue;
      if (currentScore < trigger.AtScore) continue;

      fired.Add(trigger.AtScore);
      TryTrigger(trigger.Dialogue);
    }
  }

  private void TryTrigger(DialogueSequence sequence)
  {
    if (sequence == null) return;
    DialogueManager.Instance.StartDialogue(sequence);
  }
}
