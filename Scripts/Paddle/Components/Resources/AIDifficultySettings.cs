using System;
using Godot;

[GlobalClass]
public partial class AIDifficultySettings : Resource
{
  [Export] public float ErrorMargin { get; set; } = 80f;
  [Export] public float ReactionDelay { get; set; } = 0.4f;
  [Export] public float MaxBounces { get; set; } = 2f;
}