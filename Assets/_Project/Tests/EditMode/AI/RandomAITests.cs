using Game.Core;
using NUnit.Framework;

namespace Game.AI.Tests
{
    public class RandomAITests
    {
        static Square Sq(string name) => new Square(name[0] - 'a', name[1] - '1');

        static Board InitialBoard()
        {
            var board = new Board();
            board.SetupInitialPosition();
            return board;
        }

        [Test]
        public void InitialPosition_ReturnsLegalMove()
        {
            var board = InitialBoard();
            Move? move = new RandomAI().ChooseMove(board);

            Assert.IsTrue(move.HasValue);
            var legal = MoveGenerator.GetAllLegalMoves(board, PieceColor.White);
            Assert.IsTrue(legal.Exists(m => m.From == move.Value.From && m.To == move.Value.To));
        }

        [Test]
        public void Stalemate_ReturnsNull()
        {
            var board = new Board();
            board.SetPiece(Sq("a1"), new Piece(PieceType.King, PieceColor.White));
            board.SetPiece(Sq("b3"), new Piece(PieceType.Queen, PieceColor.Black));
            board.SetPiece(Sq("h8"), new Piece(PieceType.King, PieceColor.Black));

            Assert.IsNull(new RandomAI().ChooseMove(board));
        }

        [Test]
        public void ChooseMove_DoesNotChangeBoard()
        {
            var board = InitialBoard();
            new RandomAI().ChooseMove(board);

            Assert.AreEqual(0, board.MoveCount);
            Assert.AreEqual(PieceType.Pawn, board.GetPiece(Sq("e2")).Type);
        }

        [Test]
        public void SameSeed_ChoosesSameMove()
        {
            Move? a = new RandomAI(42).ChooseMove(InitialBoard());
            Move? b = new RandomAI(42).ChooseMove(InitialBoard());

            Assert.AreEqual(a.Value.From, b.Value.From);
            Assert.AreEqual(a.Value.To, b.Value.To);
        }
    }
}
