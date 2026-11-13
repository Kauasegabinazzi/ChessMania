using System;

namespace Game.Core
{
    public readonly struct Move
    {
        public readonly Square From;
        public readonly Square To;
        public readonly Piece Piece;
        public readonly Piece Captured;
        public readonly bool PieceHadMoved;
        public readonly PieceType? Promotion;
        public readonly bool IsEnPassant;
        public bool IsCastling => Piece.Type == PieceType.King && Math.Abs(To.X - From.X) == 2;
        public Square CastlingRookFrom => new Square(To.X > From.X ? 7 : 0, From.Y);
        public Square CastlingRookTo => new Square((From.X + To.X) / 2, From.Y);
        public Square CapturedSquare => IsEnPassant ? new Square(To.X, From.Y) : To;

        public Move(Square from, Square to, Piece piece, Piece captured, PieceType? promotion = null, bool isEnPassant = false)
        {
            From = from;
            To = to;
            Piece = piece;
            Captured = captured;
            PieceHadMoved = piece.HasMoved;
            Promotion = promotion;
            IsEnPassant = isEnPassant;
        }

        public bool IsCapture => Captured != null;
        public bool IsPromotion => Promotion.HasValue;
        public override string ToString() =>
            $"{Piece.Type} {From}-{To}{(IsCapture ? " x" : "")}{(IsPromotion ? $"={Promotion}" : "")}";
    }
}
