using Game.Core;

namespace Game.AI
{
    public static class Evaluation
    {
        public const int MateScore = 100000;

        public static int PieceValue(PieceType type) => type switch
        {
            PieceType.Pawn => 100,
            PieceType.Knight => 320,
            PieceType.Bishop => 330,
            PieceType.Rook => 500,
            PieceType.Queen => 900,
            _ => 0
        };

        public static int Evaluate(Board board, PieceColor perspective)
        {
            int score = 0;

            foreach(var (_, piece) in board.AllPieces())
            {
                int value = PieceValue(piece.Type);
                score += piece.Color == perspective ? value : -value;
            }

            return score;
        }
    }
}
