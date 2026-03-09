/// <summary>
/// Fornece acesso à bola atual em jogo.
/// Implementada por BallController. Injetada nos estados da IA via PaddleStateController.
/// Permite que a IA do Boss futuramente lide com múltiplas bolas sem mudar os estados.
/// </summary>
public interface IBallProvider
{
  BallBase CurrentBall { get; }
}
