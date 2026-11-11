using UnityEngine;
using System.Collections.Generic;
using Game.Core;
using UnityEngine.InputSystem;
using System.Collections;
using Game.AI;

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

        IChessAI ai;
        bool aiThinking;

        Square? selected;
        List<Move> selectedMoves = new List<Move>();

        void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            ai = new RandomAI();
        }

        void Start()
        {
            RefreshStatus();
            TryStartAITurn();
        }

        void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if(keyboard != null && keyboard.rKey.wasPressedThisFrame)
            {
                Restart();
                return;
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
            foreach(var candidate in selectedMoves)
            {
                if(candidate.To == to)
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
            gameOver = status == GameStatus.Checkmate || status == GameStatus.Stalemate;

            if (statusView != null)
                statusView.Show(status, board.SideToMove);
        }

        void Restart()
        {
            StopAllCoroutines();
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

            Move? move = ai.ChooseMove(boardView.Board);
            aiThinking = false;

            if (move.HasValue)
                PlayMove(move.Value);
        }

    }
}
