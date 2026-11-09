using System;
using System.Collections.Generic;

namespace Game.Core
{
    public static class MoveGenerator
    {
        static readonly (int dx, int dy)[] RookDirections =
        {
            (1, 0), (-1, 0), (0, 1), (0, -1)
        };

        static readonly (int dx, int dy)[] BishopDirections =
        {
            (1, 1), (1, -1), (-1, 1), (-1, -1)
        };

        static readonly (int dx, int dy)[] KnightOffsets =
        {
          (1, 2), (2, 1), (2, -1), (1, -2),(-1, -2), (-2, -1), (-2, 1), (-1, 2)
        };

        public static List<Move> GetMoves(Board board, Square from)
        {
            var moves = new List<Move>();
            Piece piece = board.GetPiece(from);
            if (piece == null) return moves;

            switch (piece.Type)
            {
                case PieceType.Rook:
                    AddSlidingMoves(board, from, piece, RookDirections, moves);
                    break;
                case PieceType.Bishop:
                    AddSlidingMoves(board, from, piece, BishopDirections, moves);
                    break;
                case PieceType.Queen:
                    AddSlidingMoves(board, from, piece, RookDirections, moves);
                    AddSlidingMoves(board, from, piece, BishopDirections, moves);
                    break;
                case PieceType.Knight:
                    AddJumpMoves(board, from, piece, KnightOffsets, moves);
                    break;
            }

            return moves;
        }

        private static void AddJumpMoves(Board board, Square from, Piece piece, (int dx, int dy)[] offsets, List<Move> moves)
        {
            foreach(var(dx, dy) in offsets)
            {
                var to = new Square(from.X + dx, from.Y + dy);

                if (!to.IsValid) continue;

                Piece target = board.GetPiece(to);
                if (target == null || target.Color != piece.Color)
                    moves.Add(new Move(from, to, piece, target));
            }
        }

        private static void AddSlidingMoves(Board board, Square from, Piece piece, (int dx, int dy)[] directions, List<Move> moves)
        {
            foreach(var (dx, dy) in directions)
            {
                var to = new Square(from.X + dx, from.Y + dy);

                while (to.IsValid)
                {
                    Piece target = board.GetPiece(to);

                    if(target == null)
                    {
                        moves.Add(new Move(from, to, piece, null));
                    }
                    else
                    {
                        if (target.Color != piece.Color)
                            moves.Add(new Move(from, to, piece, target));
                        break;
                    }

                    to = new Square(to.X + dx, to.Y + dy);
                }
            }
        }
    }
}
