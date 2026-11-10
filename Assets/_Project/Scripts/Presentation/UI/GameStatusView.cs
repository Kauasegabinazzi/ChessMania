using UnityEngine;
using TMPro;
using Game.Core;

namespace Game.Presentation
{
    public class GameStatusView : MonoBehaviour
    {
        [SerializeField] TextMeshPro label;

        public void Show(GameStatus status, PieceColor sideToMove)
        {
            label.text = status switch
            {
                GameStatus.Check => $"{sideToMove} in check",
                GameStatus.Checkmate => $"Checkmate! {sideToMove.Opponent()} wins",
                GameStatus.Stalemate => "Stalemate - draw",
                _ => $"{sideToMove} to move"
            };
        }
    }
}
