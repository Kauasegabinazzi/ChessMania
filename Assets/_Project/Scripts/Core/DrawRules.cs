using System.Collections.Generic;
using System.Text;

namespace Game.Core
{
    public static class DrawRules
    {
        public static bool IsInsufficientMaterial(Board board)
        {
            int minorPieces = 0;

            foreach (var (_, piece) in board.AllPieces())
            {
                switch (piece.Type)
                {
                    case PieceType.King:
                        break;
                    case PieceType.Bishop:
                    case PieceType.Knight:
                        minorPieces++;
                        break;
                    default:
                        return false;
                }
            }

            return minorPieces <= 1;
        }

        public static bool IsFiftyMoveRule(Board board)
        {
            return board.HalfmoveClock >= 100;
        }

        public static bool IsThreefoldRepetition(Board board)
        {
            string current = PositionKey(board);
            int plies = board.HalfmoveClock;
            int count = 1;
            var undone = new Stack<Move>();

            for (int i = 0; i < plies; i++)
            {
                undone.Push(board.LastMove.Value);
                board.UndoMove();

                if (PositionKey(board) == current)
                    count++;
            }

            while (undone.Count > 0)
                board.MakeMove(undone.Pop());

            return count >= 3;
        }

        static string PositionKey(Board board)
        {
            var key = new StringBuilder(80);
            key.Append(board.SideToMove == PieceColor.White ? 'w' : 'b');

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Piece piece = board.GetPiece(new Square(x, y));
                    if (piece == null)
                    {
                        key.Append('.');
                        continue;
                    }

                    char letter = Letter(piece.Type);
                    key.Append(piece.Color == PieceColor.White ? char.ToUpper(letter) : letter);

                    if ((piece.Type == PieceType.King || piece.Type == PieceType.Rook) && !piece.HasMoved)
                        key.Append("*");
                }
            }

            return key.ToString();
        }

        static char Letter(PieceType type) => type switch
        {
            PieceType.Pawn => 'p',
            PieceType.Knight => 'n',
            PieceType.Bishop => 'b',
            PieceType.Rook => 'r',
            PieceType.Queen => 'q',
            PieceType.King => 'k',
            _ => '?'
        };
    }
}
