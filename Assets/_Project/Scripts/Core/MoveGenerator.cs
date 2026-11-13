using System.Collections.Generic;
using System;

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

        static readonly (int dx, int dy)[] KingOffsets =
        {
          (1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (1, -1), (-1, 1), (-1, -1)
        };

        static readonly int[] PawnCaptureFiles = { -1, 1 };

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
                case PieceType.Pawn:
                    AddPawnMoves(board, from, piece, moves);
                    break;
                case PieceType.King:
                    AddJumpMoves(board, from, piece, KingOffsets, moves);
                    AddCastlingMoves(board, from, piece, moves);
                    break;
            }

            return moves;
        }

        static void AddCastlingMoves(Board board, Square from, Piece king, List<Move> moves)
        {
            int homeRank = king.Color == PieceColor.White ? 0 : 7;
            if (king.HasMoved || from.X != 4 || from.Y != homeRank) return;

            PieceColor enemy = king.Color.Opponent();
            if (IsSquareAttacked(board, from, enemy)) return;

            TryAddCastling(board, from, king, 7, enemy, moves);
            TryAddCastling(board, from, king, 0, enemy, moves);
        }

        static void TryAddCastling(Board board, Square from, Piece king, int rookFile, PieceColor enemy, List<Move> moves)
        {
            Piece rook = board.GetPiece(new Square(rookFile, from.Y));
            if (rook == null || rook.Type != PieceType.Rook || rook.Color != king.Color || rook.HasMoved) return;

            int step = rookFile > from.X ? 1 : -1;

            for (int x = from.X + step; x != rookFile; x += step)
            {
                if (board.GetPiece(new Square(x, from.Y)) != null) return;
            }

            var passing = new Square(from.X + step, from.Y);
            if (IsSquareAttacked(board, passing, enemy)) return;

            var target = new Square(from.X + 2 * step, from.Y);
            moves.Add(new Move(from, target, king, null));
        }

        public static List<Move> GetLegalMoves(Board board, Square from)
        {
            var legalMoves = new List<Move>();

            foreach (var move in GetMoves(board, from))
            {
                board.MakeMove(move);
                bool leavesKingInCheck = IsInCheck(board, move.Piece.Color);
                board.UndoMove();

                if (!leavesKingInCheck)
                    legalMoves.Add(move);

            }

            return legalMoves;
        }

        public static bool IsInCheck(Board board, PieceColor color)
        {
            Square? king = board.FindKing(color);
            if (!king.HasValue) return false;

            return IsSquareAttacked(board, king.Value, color.Opponent());
        }

        //static bool IsSquareAttacked(Board board, Square square, PieceColor attacker)
        //{
        //    foreach (var (from, piece) in board.AllPieces())
        //    {
        //        if (piece.Color != attacker) continue;

        //        foreach (var move in GetMoves(board, from))
        //        {
        //            if (move.To == square)
        //                return true;
        //        }
        //    }

        //    return false;
        //}

        public static bool IsSquareAttacked(Board board, Square square, PieceColor attacker)
        {
            int pawnDirection = attacker == PieceColor.White ? 1 : -1;
            foreach (int dx in PawnCaptureFiles)
            {
                if (HasPiece(board, new Square(square.X + dx, square.Y - pawnDirection), PieceType.Pawn, attacker))
                    return true;
            }

            foreach (var (dx, dy) in KnightOffsets)
            {
                if (HasPiece(board, new Square(square.X + dx, square.Y + dy), PieceType.Knight, attacker))
                    return true;
            }

            foreach (var (dx, dy) in KingOffsets)
            {
                if (HasPiece(board, new Square(square.X + dx, square.Y + dy), PieceType.King, attacker))
                    return true;
            }

            return IsAttackedAlongLines(board, square, attacker, RookDirections, PieceType.Rook) || IsAttackedAlongLines(board, square, attacker, BishopDirections, PieceType.Bishop);
        }

        static bool HasPiece(Board board, Square square, PieceType type, PieceColor color)
        {
            if (!square.IsValid) return false;

            Piece piece = board.GetPiece(square);
            return piece != null && piece.Type == type && piece.Color == color;
        }

        static bool IsAttackedAlongLines(Board board, Square square, PieceColor attacker, (int dx, int dy)[] directions, PieceType slider)
        {
            foreach (var (dx, dy) in directions)
            {
                var current = new Square(square.X + dx, square.Y + dy);

                while (current.IsValid)
                {
                    Piece piece = board.GetPiece(current);
                    if (piece != null)
                    {
                        if (piece.Color == attacker && (piece.Type == slider || piece.Type == PieceType.Queen))
                            return true;
                        break;
                    }

                    current = new Square(current.X + dx, current.Y + dy);
                }
            }

            return false;
        }

        //static PieceColor Opponent(PieceColor color)
        //{
        //    return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        //}

        private static void AddJumpMoves(Board board, Square from, Piece piece, (int dx, int dy)[] offsets, List<Move> moves)
        {
            foreach (var (dx, dy) in offsets)
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
            foreach (var (dx, dy) in directions)
            {
                var to = new Square(from.X + dx, from.Y + dy);

                while (to.IsValid)
                {
                    Piece target = board.GetPiece(to);

                    if (target == null)
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

        static void AddPawnMoves(Board board, Square from, Piece piece, List<Move> moves)
        {
            int direction = piece.Color == PieceColor.White ? 1 : -1;
            int startRank = piece.Color == PieceColor.White ? 1 : 6;

            var oneStep = new Square(from.X, from.Y + direction);
            if (oneStep.IsValid && board.GetPiece(oneStep) == null)
            {
                //moves.Add(new Move(from, oneStep, piece, null));
                AddPawnMove(from, oneStep, piece, null, moves);
                var twoStep = new Square(from.X, from.Y + 2 * direction);
                if (from.Y == startRank && board.GetPiece(twoStep) == null)
                {
                    moves.Add(new Move(from, twoStep, piece, null));
                }
            }

            foreach (int dx in PawnCaptureFiles)
            {
                var target = new Square(from.X + dx, from.Y + direction);
                if (!target.IsValid) continue;

                Piece enemy = board.GetPiece(target);
                if (enemy != null && enemy.Color != piece.Color)
                {
                    //moves.Add(new Move(from, target, piece, enemy));
                    AddPawnMove(from, target, piece, enemy, moves);
                }
            }

            AddEnPassant(board, from, piece, direction, moves);
        }

        static void AddEnPassant(Board board, Square from, Piece piece, int direction, List<Move> moves)
        {
            Move? last = board.LastMove;
            if (!last.HasValue) return;

            Move previous = last.Value;
            if (previous.Piece.Type != PieceType.Pawn || previous.Piece.Color == piece.Color) return;
            if (Math.Abs(previous.To.Y - previous.From.Y) != 2) return;
            if (previous.To.Y != from.Y || Math.Abs(previous.To.X - from.X) != 1) return;

            var target = new Square(previous.To.X, from.Y + direction);
            moves.Add(new Move(from, target, piece, previous.Piece, isEnPassant: true));
        }

        static readonly PieceType[] PromotionTypes =
        {
            PieceType.Queen, PieceType.Rook, PieceType.Bishop, PieceType.Knight
        };

        static void AddPawnMove(Square from, Square to, Piece piece, Piece captured, List<Move> moves)
        {
            int lastRank = piece.Color == PieceColor.White ? 7 : 0;

            if (to.Y != lastRank)
            {
                moves.Add(new Move(from, to, piece, captured));
                return;
            }

            foreach (PieceType type in PromotionTypes)
                moves.Add(new Move(from, to, piece, captured, type));
        }

        public static List<Move> GetAllLegalMoves(Board board, PieceColor color)
        {
            var pieces = new List<(Square square, Piece piece)>(board.AllPieces());
            var moves = new List<Move>();

            foreach (var (square, piece) in pieces)
            {
                if (piece.Color == color)
                    moves.AddRange(GetLegalMoves(board, square));
            }

            return moves;
        }

        public static GameStatus GetStatus(Board board)
        {
            PieceColor side = board.SideToMove;
            bool inCheck = IsInCheck(board, side);
            bool hasMoves = GetAllLegalMoves(board, side).Count > 0;

            if (!hasMoves)
                return inCheck ? GameStatus.Checkmate : GameStatus.Stalemate;
            if (DrawRules.IsInsufficientMaterial(board))
                return GameStatus.DrawInsufficientMaterial;
            if (DrawRules.IsFiftyMoveRule(board))
                return GameStatus.DrawFiftyMoves;
            if (DrawRules.IsThreefoldRepetition(board))
                return GameStatus.DrawRepetition;

            return inCheck ? GameStatus.Check : GameStatus.InProgress;
        }
    }
}
