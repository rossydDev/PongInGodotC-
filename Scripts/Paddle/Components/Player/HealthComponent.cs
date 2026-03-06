using Godot;

public partial class HealthComponent : Node2D
{
  [Export] public float MaxHearts = 3f;
  [Signal] public delegate void OnHealthChangedEventHandler(float current, float max);

  public float CurrentHearts { get; private set; }

  public override void _Ready()
  {
    CurrentHearts = MaxHearts;
  }

  public bool TrySpend(float amount)
  {
    if (CurrentHearts < amount) return false;

    CurrentHearts -= amount;
    EmitSignal(SignalName.OnHealthChanged, CurrentHearts, MaxHearts);
    return true;
  }

  public void Reset()
  {
    CurrentHearts = MaxHearts;
    EmitSignal(SignalName.OnHealthChanged, CurrentHearts, MaxHearts);
  }
}