using Godot;

[GlobalClass]
public partial class DialogueScoreTrigger : Resource
{
  [Export] public int AtScore { get; set; } = 1;
  [Export] public DialogueSequence Dialogue { get; set; }
}