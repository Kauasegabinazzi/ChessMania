using System.Collections.Generic;

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
            //string current = PositionKey(board);
            ulong current = Zobrist.Hash(board);
            int plies = board.HalfmoveClock;
            int count = 1;
            var undone = new Stack<Move>();

            for (int i = 0; i < plies; i++)
            {
                undone.Push(board.LastMove.Value);
                board.UndoMove();

                if (Zobrist.Hash(board) == current)
                    count++;
            }

            while (undone.Count > 0)
                board.MakeMove(undone.Pop());

            return count >= 3;
        }
    }
}
