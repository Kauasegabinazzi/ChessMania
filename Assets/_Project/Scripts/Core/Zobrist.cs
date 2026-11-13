using System;

namespace Game.Core
{
    public static class Zobrist
    {
        static readonly ulong[,] PieceKeys = new ulong[12, 64];
        static readonly ulong[] UnmovedKeys = new ulong[64];
        static readonly ulong BlackToMove;

        static Zobrist()
        {
            var random = new Random(20261113);

            for (int piece = 0; piece < 12; piece++)
                for (int square = 0; square < 64; square++)
                    PieceKeys[piece, square] = NextKey(random);

            for (int square = 0; square < 64; square++)
                UnmovedKeys[square] = NextKey(random);

            BlackToMove = NextKey(random);
        }

        static ulong NextKey(Random random)
        {
            var bytes = new byte[8];
            random.NextBytes(bytes);
            return BitConverter.ToUInt64(bytes, 0);
        }

        public static ulong Hash(Board board)
        {
            ulong hash = board.SideToMove == PieceColor.Black ? BlackToMove : 0UL;

            foreach (var (square, piece) in board.AllPieces())
            {
                int index = square.Y * 8 + square.X;
                int pieceIndex = (int)piece.Type + (piece.Color == PieceColor.White ? 0 : 6);

                hash ^= PieceKeys[pieceIndex, index];

                if ((piece.Type == PieceType.King || piece.Type == PieceType.Rook) && !piece.HasMoved)
                    hash ^= UnmovedKeys[index];
            }

            return hash;
        }
    }
}
