namespace Game.Core
{
    public readonly struct Move
    {
        public readonly Square From;
        public readonly Square To;
        public readonly Piece Piece;
        public readonly Piece Captured;
        public readonly bool PieceHadMoved;

        public Move(Square from, Square to, Piece piece, Piece captured)
        {
            From = from;
            To = to;
            Piece = piece;
            Captured = captured;
            PieceHadMoved = piece.HasMoved;
        }

        public bool IsCapture => Captured != null;
        public override string ToString() => $"{Piece.Type} {From}-{To}{(IsCapture ? " x" : "")}";
    }
}
