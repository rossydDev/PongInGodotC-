using Godot;

/// <summary>
/// Autoload responsável por orquestrar sequências de diálogo.
/// Ao iniciar: salva o GameState atual e congela o jogo (Freeze).
/// Ao terminar: restaura o estado anterior, com uma exceção —
/// se o estado anterior era Intro, NÃO restaura, pois o MatchIntroComponent
/// é quem decide avançar (para o countdown e depois para Start).
/// </summary>
public partial class DialogueManager : Node
{
  public static DialogueManager Instance { get; private set; }

  [Signal] public delegate void OnDialogueFinishedEventHandler();

  [Export] private DialogueBox dialogueBox;

  private DialogueSequence currentSequence;
  private int currentLineIndex;
  private GameState stateBeforeDialogue;
  private bool isRunning;

  public bool IsRunning => isRunning;

  public override void _Ready()
  {
    Instance = this;
  }

  public void StartDialogue(DialogueSequence sequence)
  {
    if (sequence == null || sequence.Lines.Length == 0) return;
    if (isRunning) return;

    isRunning = true;
    currentSequence = sequence;
    currentLineIndex = 0;

    stateBeforeDialogue = GameManager.Instance.CurrentState;
    GameManager.Instance.SwitchState(GameState.Freeze);

    dialogueBox.Show();
    ShowCurrentLine();
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (!isRunning) return;
    if (!@event.IsActionPressed("Interact")) return;

    if (dialogueBox.IsTyping)
      dialogueBox.Skip();
    else
      AdvanceLine();
  }

  private void ShowCurrentLine()
  {
    dialogueBox.ShowLine(currentSequence.Lines[currentLineIndex]);
  }

  private void AdvanceLine()
  {
    currentLineIndex++;

    if (currentLineIndex < currentSequence.Lines.Length)
    {
      ShowCurrentLine();
      return;
    }

    FinishDialogue();
  }

  private void FinishDialogue()
  {
    isRunning = false;
    currentSequence = null;
    currentLineIndex = 0;

    dialogueBox.Hide();

    // Intro é a única exceção — o MatchIntroComponent escuta OnDialogueFinished
    // e decide o próximo passo (countdown → Start). Restaurar Intro aqui
    // causaria loop, pois o DialogueTriggerComponent dispararia o diálogo de novo.
    if (stateBeforeDialogue != GameState.Intro)
      GameManager.Instance.SwitchState(stateBeforeDialogue);

    EmitSignal(SignalName.OnDialogueFinished);
  }
}
