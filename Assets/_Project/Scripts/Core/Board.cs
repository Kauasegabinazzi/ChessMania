using System.Collections.Generic;
using System;

namespace Game.Core
{
    public class Board
    {
        readonly Piece[,] squares = new Piece[8, 8];
        readonly Stack<Move> history = new Stack<Move>();
        public int MoveCount => history.Count;
        public Move? LastMove => history.Count > 0 ? history.Peek() : (Move?)null;

        public int HalfmoveClock
        {
            get
            {
                int count = 0;
                foreach(Move move in history)
                {
                    if (move.IsCapture || move.IsPromotion || move.Piece.Type == PieceType.Pawn) break;
                    count++;
                }
                return count;
            }
        }

        public PieceColor SideToMove => history.Count % 2 == 0 ? PieceColor.White : PieceColor.Black;
        public Piece GetPiece(Square square)
        {
            return square.IsValid ? squares[square.X, square.Y] : null;
        }
        public void SetPiece(Square square, Piece piece)
        {
            if (!square.IsValid) return;
            squares[square.X, square.Y] = piece;
        }
        public void Clear()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    squares[x, y] = null;
                }
            }

            history.Clear();
        }
        public IEnumerable<(Square square, Piece piece)> AllPieces()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Piece piece = squares[x, y];
                    if (piece != null)
                    {
                        yield return (new Square(x, y), piece);
                    }
                }
            }
        }

        public Square? FindKing(PieceColor color)
        {
            foreach (var (square, piece) in AllPieces())
            {
                if (piece.Type == PieceType.King && piece.Color == color)
                {
                    return square;
                }
            }
            return null;
        }

        public void MakeMove(Move move)
        {
            if (GetPiece(move.From) != move.Piece)
                throw new InvalidOperationException($"MakeMove: {move} does not match the board.");
            SetPiece(move.To, move.Piece);
            SetPiece(move.From, null);
            if (move.IsEnPassant)
                SetPiece(move.CapturedSquare, null);

            move.Piece.HasMoved = true;

            if (move.IsPromotion)
                move.Piece.Type = move.Promotion.Value;

            if (move.IsCastling)
            {
                Piece rook = GetPiece(move.CastlingRookFrom);
                SetPiece(move.CastlingRookTo, rook);
                SetPiece(move.CastlingRookFrom, null);
                rook.HasMoved = true;
            }

            history.Push(move);
        }

        public bool UndoMove()
        {
            if (history.Count == 0) return false;

            Move move = history.Pop();

            SetPiece(move.From, move.Piece);
            //SetPiece(move.To, move.Captured);

            SetPiece(move.To, null);
            SetPiece(move.CapturedSquare, move.Captured);

            move.Piece.HasMoved = move.PieceHadMoved;

            if (move.IsPromotion)
                move.Piece.Type = PieceType.Pawn;

            if (move.IsCastling)
            {
                Piece rook = GetPiece(move.CastlingRookTo);
                SetPiece(move.CastlingRookFrom, rook);
                SetPiece(move.CastlingRookTo, null);
                rook.HasMoved = false;
            }

            return true;
        }

        public void SetupInitialPosition()
        {
            Clear();

            PieceType[] backRank =
            {
                PieceType.Rook,
                PieceType.Knight,
                PieceType.Bishop,
                PieceType.Queen,
                PieceType.King,
                PieceType.Bishop,
                PieceType.Knight,
                PieceType.Rook
            };

            for (int x = 0; x < 8; x++)
            {
                SetPiece(new Square(x, 0), new Piece(backRank[x], PieceColor.White));
                SetPiece(new Square(x, 1), new Piece(PieceType.Pawn, PieceColor.White));

                SetPiece(new Square(x, 6), new Piece(PieceType.Pawn, PieceColor.Black));
                SetPiece(new Square(x, 7), new Piece(backRank[x], PieceColor.Black));
            }
        }


    }
}
