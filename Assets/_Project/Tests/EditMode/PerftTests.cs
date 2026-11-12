using NUnit.Framework;

namespace Game.Core.Tests
{
    public class PerftTests
    {
        static int Perft(Board board, int depth)
        {
            if (depth == 0) return 1;

            int count = 0;
            foreach (var move in MoveGenerator.GetAllLegalMoves(board, board.SideToMove))
            {
                board.MakeMove(move);
                count += Perft(board, depth - 1);
                board.UndoMove();
            }

            return count;
        }

        [TestCase(1, 20)]
        [TestCase(2, 400)]
        [TestCase(3, 8902)]
        [TestCase(4, 197281)]
        public void InitialPosition_Perft(int depth, int expected)
        {
            var board = new Board();
            board.SetupInitialPosition();

            Assert.AreEqual(expected, Perft(board, depth));
        }
    }
}
