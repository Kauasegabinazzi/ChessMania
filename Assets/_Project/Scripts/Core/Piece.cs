using System.Drawing;

namespace Game.Core
{
    public enum PieceType
    {
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King
    }
    public enum PieceColor
    {
        White,
        Black
    }

    public class Piece
    {
        public PieceType Type { get; internal set; } // set allows to get promotion type
        public PieceColor Color { get; }
        public bool HasMoved { get; internal set; }
        public Piece(PieceType type, PieceColor color)
        {
            Type = type;
            Color = color;
        }
    }

    public static class PieceColorExtensions
    {
        public static PieceColor Opponent(this PieceColor color)
        {
            return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        }
    }
}