using Godot;

/// <summary>
/// Node da UI que exibe uma DialogueLine com efeito typewriter estilo Undertale.
/// Estrutura da cena: veja dialogue_box.tscn
/// </summary>
public partial class DialogueBox : PanelContainer
{
  [Export] private TextureRect portraitTexture;
  [Export] private Label speakerNameLabel;
  [Export] private RichTextLabel dialogueText;
  [Export] private Label continueArrow;

  [Export] private float typewriterSpeed = 30f; // caracteres por segundo

  public bool IsTyping { get; private set; }

  private double charTimer;
  private int visibleChars;
  private Tween arrowTween;

  public override void _Ready()
  {
    Hide();
  }

  public void ShowLine(DialogueLine line)
  {
    // Retrato
    if (portraitTexture != null)
    {
      portraitTexture.Texture = line.Portrait;
      portraitTexture.GetParent<PanelContainer>().Visible = line.Portrait != null;
    }

    // Nome do falante
    if (speakerNameLabel != null)
    {
      speakerNameLabel.Text = line.SpeakerName;
      speakerNameLabel.Visible = !string.IsNullOrEmpty(line.SpeakerName);
    }

    // Esconde seta enquanto digita
    SetArrowVisible(false);

    // Inicia typewriter
    dialogueText.Text = line.Text;
    dialogueText.VisibleCharacters = 0;
    visibleChars = 0;
    charTimer = 0;
    IsTyping = true;
  }

  // Completa o texto instantaneamente
  public void Skip()
  {
    if (!IsTyping) return;

    dialogueText.VisibleCharacters = -1; // -1 = mostra tudo
    IsTyping = false;
    SetArrowVisible(true);
  }

  public override void _Process(double delta)
  {
    if (!IsTyping) return;

    charTimer += delta;
    int charsToAdd = (int)(charTimer * typewriterSpeed);

    if (charsToAdd < 1) return;

    charTimer -= charsToAdd / typewriterSpeed;
    visibleChars += charsToAdd;

    if (visibleChars >= dialogueText.Text.Length)
    {
      visibleChars = dialogueText.Text.Length;
      dialogueText.VisibleCharacters = visibleChars;
      IsTyping = false;
      SetArrowVisible(true);
      return;
    }

    dialogueText.VisibleCharacters = visibleChars;
  }

  private void SetArrowVisible(bool visible)
  {
    if (continueArrow == null) return;

    continueArrow.Visible = visible;

    if (!visible)
    {
      arrowTween?.Kill();
      return;
    }

    // Seta piscante estilo Undertale
    arrowTween?.Kill();
    arrowTween = CreateTween().SetLoops();
    arrowTween.TweenProperty(continueArrow, "modulate:a", 0f, 0.4f);
    arrowTween.TweenProperty(continueArrow, "modulate:a", 1f, 0.4f);
  }
}
