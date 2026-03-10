using Godot;

[GlobalClass]
public partial class DialogueLine : Resource
{
  [Export] public string SpeakerName { get; set; } = "";
  [Export] public Texture2D Portrait { get; set; }
  [Export(PropertyHint.MultilineText)]
  public string Text { get; set; } = "";
}