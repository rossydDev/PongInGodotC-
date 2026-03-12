using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Intercepta transições de estado para disparar diálogos antes de confirmá-las.
/// Priority=0 — executa primeiro no pipeline, antes do VictoryConditionComponent.
///
/// Gatilhos de Score:
///   Intercepta PlayerScore/EnemyScore → verifica se há diálogo configurado
///   para o score atual → roda diálogo → confirma a transição original
///
/// Gatilhos de Vitória/Derrota:
///   Intercepta PlayerWin/PlayerLoser → roda diálogo → confirma
///
/// Gatilho de Intro:
///   Escuta OnGameStateChanged passivamente (não intercepta, só dispara diálogo)
/// </summary>
public partial class DialogueTriggerComponent : Node, IStateInterceptor
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

  public int Priority => 0;

  public override void _Ready()
  {
    GameManager.Instance.RegisterInterceptor(this);
    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;

    TreeExiting += () =>
    {
      GameManager.Instance.UnregisterInterceptor(this);
      GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    };
  }

  public void Initialize()
  {
    // Mantido para compatibilidade com CampInitializer
    // Registro no pipeline já acontece no _Ready
  }

  // ── IStateInterceptor ─────────────────────────────────────────────────────

  public bool CanIntercept(GameState requestedState)
  {
    return requestedState == GameState.PlayerScore
        || requestedState == GameState.EnemyScore
        || requestedState == GameState.PlayerWin
        || requestedState == GameState.PlayerLoser;
  }

  public void Intercept(GameState requestedState, Action confirm)
  {
    DialogueSequence dialogue = null;

    switch (requestedState)
    {
      case GameState.PlayerScore:
        // +1 antecipado para checar o score que vai ser registrado
        dialogue = FindScoreTrigger(
          onPlayerScore,
          ScoreControll.Instance.PlayerScore + 1,
          firedPlayerScoreTriggers
        );
        break;

      case GameState.EnemyScore:
        dialogue = FindScoreTrigger(
          onEnemyScore,
          ScoreControll.Instance.EnemyScore + 1,
          firedEnemyScoreTriggers
        );
        break;

      case GameState.PlayerWin:
        dialogue = onPlayerWin;
        break;

      case GameState.PlayerLoser:
        dialogue = onEnemyWin;
        break;
    }

    if (dialogue == null)
    {
      confirm();
      return;
    }

    // Roda diálogo e confirma quando terminar
    DialogueManager.Instance.OnDialogueFinished += OnDialogueFinished;
    _pendingConfirm = confirm;
    DialogueManager.Instance.StartDialogue(dialogue);
  }

  // ── Listener passivo (Intro) ──────────────────────────────────────────────

  private void OnGameStateChanged()
  {
    if (GameManager.Instance.CurrentState != GameState.Intro) return;

    firedPlayerScoreTriggers.Clear();
    firedEnemyScoreTriggers.Clear();

    if (onMatchStart == null) return;

    // Intro não intercepta o pipeline — apenas dispara o diálogo
    // MatchIntroComponent aguarda OnDialogueFinished para iniciar o countdown
    DialogueManager.Instance.StartDialogue(onMatchStart);
  }

  // ── Controle interno de diálogo ───────────────────────────────────────────

  private Action _pendingConfirm;

  private void OnDialogueFinished()
  {
    DialogueManager.Instance.OnDialogueFinished -= OnDialogueFinished;

    var confirm = _pendingConfirm;
    _pendingConfirm = null;
    confirm?.Invoke();
  }

  // ── Helpers ───────────────────────────────────────────────────────────────

  private DialogueSequence FindScoreTrigger(
    DialogueScoreTrigger[] triggers,
    int score,
    HashSet<int> fired)
  {
    if (triggers == null) return null;

    foreach (var trigger in triggers)
    {
      if (trigger?.Dialogue == null) continue;
      if (fired.Contains(trigger.AtScore)) continue;
      if (score != trigger.AtScore) continue; // exato, não >=

      fired.Add(trigger.AtScore);
      return trigger.Dialogue;
    }

    return null;
  }
}