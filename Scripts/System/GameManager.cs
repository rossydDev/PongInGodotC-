using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
  [Signal]
  public delegate void OnGameStateChangedEventHandler();

  public static GameManager Instance { get; private set; }

  private GameState currentState = GameState.Freeze;
  public GameState CurrentState => currentState;

  // Interceptadores registrados ordenados por prioridade
  private readonly List<IStateInterceptor> interceptors = new();

  public override void _Ready()
  {
    Instance = this;
    WorldBounds.Initialize(GetTree().Root.GetVisibleRect().Size.X);
    EmitSignal(SignalName.OnGameStateChanged);
  }

  // ── Registro de interceptadores ──────────────────────────────────────────

  public void RegisterInterceptor(IStateInterceptor interceptor)
  {
    interceptors.Add(interceptor);
    interceptors.Sort((a, b) => a.Priority.CompareTo(b.Priority));
  }

  public void UnregisterInterceptor(IStateInterceptor interceptor)
  {
    interceptors.Remove(interceptor);
  }

  // ── Ponto de entrada público ──────────────────────────────────────────────

  /// <summary>
  /// Solicita uma transição de estado.
  /// Passa pelo pipeline de interceptadores antes de confirmar.
  /// Use este método em vez de SwitchState de fora do GameManager.
  /// </summary>
  public void RequestStateChange(GameState newState)
  {
    RunPipeline(newState, 0);
  }

  // ── Pipeline interno ──────────────────────────────────────────────────────

  private void RunPipeline(GameState requestedState, int interceptorIndex)
  {
    // Percorre interceptadores a partir do índice atual
    for (int i = interceptorIndex; i < interceptors.Count; i++)
    {
      var interceptor = interceptors[i];

      if (!interceptor.CanIntercept(requestedState)) continue;

      // Captura o índice para o closure do confirm
      int nextIndex = i + 1;

      interceptor.Intercept(requestedState, () =>
      {
        // Quando o interceptador confirmar, continua o pipeline
        RunPipeline(requestedState, nextIndex);
      });

      // Pipeline pausado — o interceptador vai chamar confirm() quando terminar
      return;
    }

    // Nenhum interceptador agiu — confirma a transição
    ConfirmState(requestedState);
  }

  private void ConfirmState(GameState newState)
  {
    if (newState == currentState) return;

    currentState = newState;
    EmitSignal(SignalName.OnGameStateChanged);
  }

  // ── Helpers ───────────────────────────────────────────────────────────────

  /// <summary>
  /// Transição direta sem pipeline. Use apenas internamente
  /// ou em casos onde o pipeline já foi executado.
  /// </summary>
  public void SwitchState(GameState newState)
  {
    ConfirmState(newState);
  }

  public void Scored(Paddle scoredPaddle)
  {
    BallController.Instance.CurrentBall.SpawnExplosion();

    if (scoredPaddle is PaddleIA)
      RequestStateChange(GameState.EnemyScore);
    else
      RequestStateChange(GameState.PlayerScore);
  }
}