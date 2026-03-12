using Godot;

/// <summary>
/// Node da UI que exibe uma DialogueLine estilo Hades/Undertale.
/// Portrait grande integrado com gradiente, caixa de texto na base,
/// nome com barra colorida lateral, seta piscante.
/// </summary>
public partial class DialogueBox : Control
{
  [Export] private TextureRect portraitTexture;
  [Export] private Control portraitContainer;
  [Export] private ColorRect dimBackground;
  [Export] private Label speakerNameLabel;
  [Export] private RichTextLabel dialogueText;
  [Export] private Label continueArrow;

  [Export] private float typewriterSpeed = 30f;
  [Export] private float portraitFadeDuration = 0.25f;

  public bool IsTyping { get; private set; }

  private double charTimer;
  private int visibleChars;
  private Tween arrowTween;
  private Texture2D lastPortrait;

  public override void _Ready()
  {
    Hide();
  }

  public void ShowLine(DialogueLine line)
  {
    bool hasPortrait = line.Portrait != null;

    // Portrait — fade só quando troca
    if (portraitTexture != null)
    {
      bool isNew = lastPortrait != line.Portrait;
      lastPortrait = line.Portrait;
      portraitTexture.Texture = line.Portrait;

      if (portraitContainer != null)
      {
        portraitContainer.Visible = hasPortrait;

        if (hasPortrait && isNew)
        {
          portraitContainer.Modulate = new Color(1, 1, 1, 0);
          var tween = CreateTween();
          tween.TweenProperty(portraitContainer, "modulate:a", 1f, portraitFadeDuration);
        }
      }
    }

    // Nome — barra colorida some quando é narrador
    if (speakerNameLabel != null)
    {
      speakerNameLabel.Text = line.SpeakerName;
      var nameBar = speakerNameLabel.GetParent<PanelContainer>();
      if (nameBar != null)
        nameBar.Visible = !string.IsNullOrEmpty(line.SpeakerName);
    }

    SetArrowVisible(false);

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