using NUnit.Framework;
using Game.Core;

namespace Game.AI.Tests
{
    public class GreedyAITests
    {
        static Square Sq(string name) => new Square(name[0] - 'a', name[1] - '1');

        static void Place(Board board, string square, PieceType type, PieceColor color)
        {
            board.SetPiece(Sq(square), new Piece(type, color));
        }

        [Test]
        public void TakesQueenInsteadOfPawn()
        {
            var board = new Board();
            Place(board, "e1", PieceType.King, PieceColor.White);
            Place(board, "a1", PieceType.Rook, PieceColor.White);
            Place(board, "c4", PieceType.Knight, PieceColor.White);
            Place(board, "h7", PieceType.King, PieceColor.Black);
            Place(board, "a8", PieceType.Queen, PieceColor.Black);
            Place(board, "b2", PieceType.Pawn, PieceColor.Black);

            Move? move = new GreedyAI(1).ChooseMove(board);

            Assert.AreEqual(Sq("a1"), move.Value.From);
            Assert.AreEqual(Sq("a8"), move.Value.To);
        }

        [Test]
        public void PrefersCheckmateOverCapture()
        {
            var board = new Board();
            Place(board, "g1", PieceType.King, PieceColor.White);
            Place(board, "a1", PieceType.Rook, PieceColor.White);
            Place(board, "f3", PieceType.Knight, PieceColor.White);
            Place(board, "h8", PieceType.King, PieceColor.Black);
            Place(board, "g7", PieceType.Pawn, PieceColor.Black);
            Place(board, "h7", PieceType.Pawn, PieceColor.Black);
            Place(board, "h4", PieceType.Rook, PieceColor.Black);

            Move? move = new GreedyAI(1).ChooseMove(board);

            Assert.AreEqual(Sq("a1"), move.Value.From);
            Assert.AreEqual(Sq("a8"), move.Value.To);
        }

        [Test]
        public void NoLegalMoves_ReturnsNull()
        {
            var board = new Board();
            Place(board, "a1", PieceType.King, PieceColor.White);
            Place(board, "b3", PieceType.Queen, PieceColor.Black);
            Place(board, "h8", PieceType.King, PieceColor.Black);

            Assert.IsNull(new GreedyAI().ChooseMove(board));
        }

        [Test]
        public void ChooseMove_DoesNotChangeBoard()
        {
            var board = new Board();
            board.SetupInitialPosition();
            new GreedyAI().ChooseMove(board);

            Assert.AreEqual(0, board.MoveCount);
            Assert.AreEqual(PieceType.Pawn, board.GetPiece(Sq("e2")).Type);
        }
    }
}
