using NUnit.Framework;
using Game.Core;

namespace Game.AI.Tests
{
    public class EvaluationTests
    {
        [Test]
        public void InitialPosition_IsZero()
        {
            var board = new Board();
            board.SetupInitialPosition();

            Assert.AreEqual(0, Evaluation.Evaluate(board, PieceColor.White));
        }

        [Test]
        public void MissingBlackQueen_IsPlus900ForWhite_Minus900ForBlack()
        {
            var board = new Board();
            board.SetupInitialPosition();
            board.SetPiece(new Square(3, 7), null);

            Assert.AreEqual(900, Evaluation.Evaluate(board, PieceColor.White));
            Assert.AreEqual(-900, Evaluation.Evaluate(board, PieceColor.Black));
        }
    }
}
