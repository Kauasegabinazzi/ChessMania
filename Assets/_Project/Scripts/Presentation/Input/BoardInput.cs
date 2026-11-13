using UnityEngine;
using System.Collections.Generic;
using Game.Core;
using UnityEngine.InputSystem;
using System.Collections;
using Game.AI;
using System.Threading.Tasks;

namespace Game.Presentation
{
    public class BoardInput : MonoBehaviour
    {
        [SerializeField] BoardView boardView;
        [SerializeField] Camera mainCamera;
        [SerializeField] GameStatusView statusView;
        bool gameOver;

        [Header("AI")]
        [SerializeField] bool playAgainstAI = true;
        [SerializeField] PieceColor aiColor = PieceColor.Black;
        [SerializeField] float aiDelay = 0.5f;
        [SerializeField] AILevel aiLevel = AILevel.Greedy;
        [SerializeField, Range(1, 4)] int aiDepth = 2;
        [SerializeField, Range(0, 200)] int aiMargin = 0;
        [SerializeField, Range(0f, 1f)] float aiMistakeChance = 0f;
        [SerializeField] bool useDifficultyPresets = true;
        [SerializeField, Range(0, 3)] int difficulty = 1;

        IChessAI ai;
        bool aiThinking;

        Square? selected;
        List<Move> selectedMoves = new List<Move>();

        void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            //ai = aiLevel switch
            //{
            //    AILevel.Greedy => new GreedyAI(),
            //    AILevel.Minimax => new MinimaxAI(aiDepth) { ScoreMargin = aiMargin, MistakeChance = aiMistakeChance },
            //    _ => new RandomAI()
            //};

            CreateAI();
        }

        void Start()
        {
            RefreshStatus();
            TryStartAITurn();
        }

        void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.rKey.wasPressedThisFrame)
                {
                    Restart();
                    return;
                }

                int chosen = PressedDifficulty(keyboard);
                if (chosen >= 0)
                {
                    difficulty = chosen;
                    useDifficultyPresets = true;
                    Restart();
                    return;
                }
            }
            if (gameOver || aiThinking) return;

            Mouse mouse = Mouse.current;
            if (mouse == null || !mouse.leftButton.wasPressedThisFrame) return;

            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouse.position.ReadValue());

            if (boardView.TryWorldToGrid(worldPosition, out Square clicked))
                OnSquareClicked(clicked);
            else
                Deselect();
        }

        void Select(Square square)
        {
            selected = square;
            selectedMoves = MoveGenerator.GetLegalMoves(boardView.Board, square);
            boardView.ShowHighlights(square, selectedMoves);
        }

        void Deselect()
        {
            selected = null;
            selectedMoves.Clear();
            boardView.ClearHighlights();
        }

        void OnSquareClicked(Square square)
        {
            Board board = boardView.Board;
            if (board == null) return;

            if (selected.HasValue && TryFindMove(square, out Move move))
            {
                //board.MakeMove(move);
                //boardView.ApplyMove(move);
                //Deselect();
                //RefreshStatus();
                //return;

                PlayMove(move);
                return;
            }

            Piece piece = board.GetPiece(square);
            bool isOwnPiece = piece != null && piece.Color == board.SideToMove;

            if (isOwnPiece && selected != square)
                Select(square);
            else
                Deselect();
        }

        bool TryFindMove(Square to, out Move move)
        {
            foreach (var candidate in selectedMoves)
            {
                if (candidate.To == to)
                {
                    move = candidate;
                    return true;
                }
            }

            move = default;
            return false;
        }

        void RefreshStatus()
        {
            Board board = boardView.Board;
            if (board == null) return;

            GameStatus status = MoveGenerator.GetStatus(board);
            gameOver = status != GameStatus.InProgress && status != GameStatus.Check;

            if (statusView != null)
                statusView.Show(status, board.SideToMove);
        }

        void Restart()
        {
            StopAllCoroutines();
            CreateAI();
            aiThinking = false;
            var board = new Board();
            board.SetupInitialPosition();
            boardView.Initialize(board);
            Deselect();
            RefreshStatus();
            TryStartAITurn();
        }

        void PlayMove(Move move)
        {
            boardView.Board.MakeMove(move);
            boardView.ApplyMove(move);
            Deselect();
            RefreshStatus();
            TryStartAITurn();
        }

        void TryStartAITurn()
        {
            if (!playAgainstAI || gameOver) return;
            if (boardView.Board.SideToMove != aiColor) return;

            StartCoroutine(AITurn());
        }

        IEnumerator AITurn()
        {
            aiThinking = true;
            yield return new WaitForSeconds(aiDelay);

            Board board = boardView.Board;
            IChessAI thinker = ai;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            Task<Move?> task = Task.Run(() => thinker.ChooseMove(board));

            while (!task.IsCompleted)
                yield return null;

            watch.Stop();
            aiThinking = false;

            if (task.IsFaulted)
            {
                Debug.LogException(task.Exception);
                yield break;
            }

            if (thinker is MinimaxAI minimax)
                Debug.Log($"Minimax: {minimax.NodesSearched} posições em {watch.ElapsedMilliseconds} ms");

            if (task.Result.HasValue)
                PlayMove(task.Result.Value);
        }

        //    Move? move = ai.ChooseMove(boardView.Board);
        //    watch.Stop();

        //    if (ai is MinimaxAI minimax)
        //        Debug.Log($"Minimax: {minimax.NodesSearched} posições em {watch.ElapsedMilliseconds} ms");

        //    aiThinking = false;

        //    if (move.HasValue)
        //        PlayMove(move.Value);
        //}

        void CreateAI()
        {
            if (useDifficultyPresets)
            {
                ai = AIPresets.All[difficulty].Create();
            }
            else
            {
                ai = aiLevel switch
                {
                    AILevel.Greedy => new GreedyAI(),
                    AILevel.Minimax => new MinimaxAI(aiDepth) { ScoreMargin = aiMargin, MistakeChance = aiMistakeChance },
                    _ => new RandomAI()
                };
            }

            if (statusView != null)
                statusView.ShowDifficulty(useDifficultyPresets ? AIPresets.All[difficulty].Name : aiLevel.ToString());
        }

        static int PressedDifficulty(Keyboard keyboard)
        {
            if (keyboard.digit1Key.wasPressedThisFrame) return 0;
            if (keyboard.digit2Key.wasPressedThisFrame) return 1;
            if (keyboard.digit3Key.wasPressedThisFrame) return 2;
            if (keyboard.digit4Key.wasPressedThisFrame) return 3;
            return -1;
        }
    }
}
