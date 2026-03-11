using Godot;

/// <summary>
/// Node da UI que exibe uma DialogueLine com efeito typewriter estilo visual novel.
/// Portrait grande no lado esquerdo, caixa de texto na base com fundo próprio.
/// Estrutura da cena: veja dialogue_box.tscn
/// </summary>
public partial class DialogueBox : Control
{
  [Export] private TextureRect portraitTexture;
  [Export] private Label speakerNameLabel;
  [Export] private RichTextLabel dialogueText;
  [Export] private Label continueArrow;
  [Export] private ColorRect dimBackground;
  [Export] private Control portraitContainer;

  [Export] private float typewriterSpeed = 30f;
  [Export] private float portraitFadeDuration = 0.3f;

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
    // Portrait
    bool hasPortrait = line.Portrait != null;
    if (portraitTexture != null)
    {
      bool isNewPortrait = portraitTexture.Texture != line.Portrait;
      portraitTexture.Texture = line.Portrait;

      if (portraitContainer != null)
      {
        portraitContainer.Visible = hasPortrait;

        // Fade-in só quando troca o portrait
        if (hasPortrait && isNewPortrait)
        {
          portraitContainer.Modulate = new Color(1, 1, 1, 0);
          var tween = CreateTween();
          tween.TweenProperty(portraitContainer, "modulate:a", 1f, portraitFadeDuration);
        }
      }
    }

    // Nome do falante
    if (speakerNameLabel != null)
    {
      speakerNameLabel.Text = line.SpeakerName;
      speakerNameLabel.Visible = !string.IsNullOrEmpty(line.SpeakerName);
    }

    SetArrowVisible(false);

    // Inicia typewriter
    dialogueText.Text = line.Text;
    dialogueText.VisibleCharacters = 0;
    visibleChars = 0;
    charTimer = 0;
    IsTyping = true;
  }

  public void Skip()
  {
    if (!IsTyping) return;
    dialogueText.VisibleCharacters = -1;
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
    arrowTween?.Kill();

    if (!visible) return;

    arrowTween = CreateTween().SetLoops();
    arrowTween.TweenProperty(continueArrow, "modulate:a", 0f, 0.4f);
    arrowTween.TweenProperty(continueArrow, "modulate:a", 1f, 0.4f);
  }
}