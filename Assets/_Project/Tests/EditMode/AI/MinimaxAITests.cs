using NUnit.Framework;
using Game.Core;

namespace Game.AI.Tests
{
    public class MinimaxAITests
    {
        static Square Sq(string name) => new Square(name[0] - 'a', name[1] - '1');

        static void Place(Board board, string square, PieceType type, PieceColor color)
        {
            board.SetPiece(Sq(square), new Piece(type, color));
        }

        static Board DefendedPawnPosition()
        {
            var board = new Board();
            Place(board, "h1", PieceType.King, PieceColor.White);
            Place(board, "d4", PieceType.Queen, PieceColor.White);
            Place(board, "a8", PieceType.King, PieceColor.Black);
            Place(board, "d5", PieceType.Pawn, PieceColor.Black);
            Place(board, "e6", PieceType.Pawn, PieceColor.Black);
            return board;
        }

        [Test]
        public void Depth1_TakesDefendedPawn()
        {
            Move? move = new MinimaxAI(1, 1).ChooseMove(DefendedPawnPosition());

            Assert.AreEqual(Sq("d5"), move.Value.To);
        }

        [Test]
        public void Depth2_AvoidsDefendedPawn()
        {
            Move? move = new MinimaxAI(2, 1).ChooseMove(DefendedPawnPosition());

            Assert.AreNotEqual(Sq("d5"), move.Value.To);
        }

        [Test]
        public void Depth2_FindsMateInOne()
        {
            var board = new Board();
            Place(board, "g1", PieceType.King, PieceColor.White);
            Place(board, "a1", PieceType.Rook, PieceColor.White);
            Place(board, "f3", PieceType.Knight, PieceColor.White);
            Place(board, "h8", PieceType.King, PieceColor.Black);
            Place(board, "g7", PieceType.Pawn, PieceColor.Black);
            Place(board, "h7", PieceType.Pawn, PieceColor.Black);
            Place(board, "h4", PieceType.Rook, PieceColor.Black);

            Move? move = new MinimaxAI(2, 1).ChooseMove(board);

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

            Assert.IsNull(new MinimaxAI(2).ChooseMove(board));
        }

        [Test]
        public void ChooseMove_DoesNotChangeBoard()
        {
            var board = new Board();
            board.SetupInitialPosition();
            new MinimaxAI(2).ChooseMove(board);

            Assert.AreEqual(0, board.MoveCount);
            Assert.AreEqual(PieceType.Pawn, board.GetPiece(Sq("e2")).Type);
        }
    }
}
