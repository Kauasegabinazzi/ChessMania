using NUnit.Framework;

namespace Game.Core.Tests
{
    public class ZobristTests
    {
        static Square Sq(string name) => new Square(name[0] - 'a', name[1] - '1');

        static void Place(Board board, string square, PieceType type, PieceColor color)
        {
            board.SetPiece(Sq(square), new Piece(type, color));
        }

        static void Play(Board board, string from, string to)
        {
            Move move = MoveGenerator.GetLegalMoves(board, Sq(from)).Find(m => m.To == Sq(to));
            board.MakeMove(move);
        }

        static Board Initial()
        {
            var board = new Board();
            board.SetupInitialPosition();
            return board;
        }

        [Test]
        public void SamePosition_SameHash()
        {
            Assert.AreEqual(Zobrist.Hash(Initial()), Zobrist.Hash(Initial()));
        }

        [Test]
        public void MakeThenUndo_RestoresHash()
        {
            var board = Initial();
            ulong before = Zobrist.Hash(board);

            foreach (var move in MoveGenerator.GetAllLegalMoves(board, PieceColor.White))
            {
                board.MakeMove(move);
                Assert.AreNotEqual(before, Zobrist.Hash(board));
                board.UndoMove();
                Assert.AreEqual(before, Zobrist.Hash(board));
            }
        }

        [Test]
        public void KnightsGoAndReturn_SameHash()
        {
            var board = Initial();
            ulong before = Zobrist.Hash(board);

            Play(board, "g1", "f3");
            Play(board, "g8", "f6");
            Play(board, "f3", "g1");
            Play(board, "f6", "g8");

            Assert.AreEqual(before, Zobrist.Hash(board));
        }

        [Test]
        public void KingGoesAndReturns_DifferentHash_BecauseCastlingIsLost()
        {
            var board = new Board();
            Place(board, "e1", PieceType.King, PieceColor.White);
            Place(board, "h1", PieceType.Rook, PieceColor.White);
            Place(board, "e8", PieceType.King, PieceColor.Black);
            ulong before = Zobrist.Hash(board);

            Play(board, "e1", "f1");
            Play(board, "e8", "d8");
            Play(board, "f1", "e1");
            Play(board, "d8", "e8");

            Assert.AreNotEqual(before, Zobrist.Hash(board));
        }
    }
}
