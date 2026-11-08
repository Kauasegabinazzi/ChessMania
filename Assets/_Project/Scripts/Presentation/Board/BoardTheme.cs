using UnityEngine;

namespace Game.Presentation
{
    [CreateAssetMenu(fileName = "NewBoardTheme", menuName = "Game/Board Theme")]
    public class BoardTheme : ScriptableObject
    {
        [SerializeField] string themeId;
        [SerializeField] Sprite boardSprite;

        public string Id => themeId;
        public Sprite BoardSprite => boardSprite;
    }
}
