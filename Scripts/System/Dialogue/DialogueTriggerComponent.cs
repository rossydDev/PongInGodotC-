using Godot;
using System.Collections.Generic;

public partial class DialogueTriggerComponent : Node
{
  [ExportGroup("Gatilhos de Estado")]
  [Export] private DialogueSequence onMatchStart;
  [Export] private DialogueSequence onPlayerWin;
  [Export] private DialogueSequence onEnemyWin;

  [ExportGroup("Gatilhos de Score")]
  [Export] private DialogueScoreTrigger[] onPlayerScore;
  [Export] private DialogueScoreTrigger[] onEnemyScore;

  private readonly HashSet<int> firedPlayerScoreTriggers = new();
  private readonly HashSet<int> firedEnemyScoreTriggers = new();

  // Estado pendente aguardando fim do diálogo
  private GameState? pendingState = null;

  public override void _Ready()
  {
    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
  }

  public void Initialize()
  {
    ScoreControll.Instance.ScoreUpdate += OnScoreUpdate;
  }

  private void OnGameStateChanged()
  {
    switch (GameManager.Instance.CurrentState)
    {
      case GameState.Intro:
        firedPlayerScoreTriggers.Clear();
        firedEnemyScoreTriggers.Clear();
        TryTrigger(onMatchStart, null);
        break;

      case GameState.PlayerWinPending:
        TryTrigger(onPlayerWin, GameState.PlayerWin);
        break;

      case GameState.EnemyWinPending:
        TryTrigger(onEnemyWin, GameState.PlayerLoser);
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
      TryTrigger(trigger.Dialogue, null);
    }
  }

  // Se tem diálogo: intercepta e guarda o próximo estado para emitir ao terminar
  // Se não tem: emite o próximo estado direto
  private void TryTrigger(DialogueSequence sequence, GameState? stateAfter)
  {
    pendingState = stateAfter;

    if (sequence == null)
    {
      FlushPendingState();
      return;
    }

    DialogueManager.Instance.OnDialogueFinished += OnDialogueFinished;
    DialogueManager.Instance.StartDialogue(sequence);
  }

  private void OnDialogueFinished()
  {
    DialogueManager.Instance.OnDialogueFinished -= OnDialogueFinished;

    // Diálogos situacionais (stateAfter = null) voltam para Start
    if (pendingState == null)
      GameManager.Instance.SwitchState(GameState.Start);
    else
      FlushPendingState();
  }

  private void FlushPendingState()
  {
    if (pendingState == null) return;
    GameManager.Instance.SwitchState(pendingState.Value);
    pendingState = null;
  }
}