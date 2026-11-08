using System.Collections.Generic;
namespace Game.Core
{
    public class Board
    {
        readonly Piece[,] squares = new Piece[8, 8];
        public Piece GetPiece (Square square)
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
            for(int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    squares[x, y] = null;
                }
            }
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
