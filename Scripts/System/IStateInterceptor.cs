using System;

/// <summary>
/// Contrato para componentes que querem interceptar transições de estado
/// antes que o GameManager as confirme.
///
/// O pipeline chama interceptadores em ordem crescente de Priority.
/// O interceptador chama confirm() quando terminar seu trabalho.
/// Se não chamar confirm(), a transição fica pausada indefinidamente —
/// use isso só em casos onde você garante que confirm() será chamado.
/// </summary>
public interface IStateInterceptor
{
  /// <summary>
  /// Menor número = maior prioridade. Executado primeiro no pipeline.
  /// Convenção: Diálogo=0, Vitória=10, outros=20+
  /// </summary>
  int Priority { get; }

  /// <summary>
  /// Retorna true se este interceptador quer agir sobre a transição.
  /// </summary>
  bool CanIntercept(GameState requestedState);

  /// <summary>
  /// Executa o trabalho do interceptador.
  /// DEVE chamar confirm() quando terminar para o pipeline continuar.
  /// </summary>
  void Intercept(GameState requestedState, Action confirm);
}