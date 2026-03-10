using Godot;

[GlobalClass]
public partial class DialogueSequence : Resource
{
  [Export] public DialogueLine[] Lines { get; set; }
}