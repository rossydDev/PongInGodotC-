// HealthComponent.cs
using Godot;

public partial class HealthComponent : Node2D
{
  [Export] public float MaxHearts = 3f;

  [Signal] public delegate void OnHealthChangedEventHandler(float current, float max);
  [Signal] public delegate void OnHealedEventHandler(float amount, float current, float max);

  public float CurrentHearts { get; private set; }
  public bool IsFull => CurrentHearts >= MaxHearts;
  public bool IsDead => CurrentHearts <= 0f;

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

  public float Heal(float hearts)
  {
    if (hearts <= 0f || CurrentHearts >= MaxHearts) return 0f;

    float actual = Mathf.Min(hearts, MaxHearts - CurrentHearts);
    CurrentHearts += actual;
    EmitSignal(SignalName.OnHealthChanged, CurrentHearts, MaxHearts);
    EmitSignal(SignalName.OnHealed, actual, CurrentHearts, MaxHearts); // <--
    return actual;
  }

  public void Reset()
  {
    CurrentHearts = MaxHearts;
    EmitSignal(SignalName.OnHealthChanged, CurrentHearts, MaxHearts);
  }
}