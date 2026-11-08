using Game.Core;
using TMPro;
using UnityEngine;

namespace Game.Presentation
{
    [RequireComponent(typeof(BoardView))]
    public class BoardCoordinates : MonoBehaviour
    {
        [SerializeField] TMP_FontAsset font;
        [SerializeField] float fontSize = 4f;
        [SerializeField] Color color = Color.white;
        [SerializeField] int sortingOrder = 2;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            BoardView boardView = GetComponent<BoardView>();

            for(int i = 0; i < 8; i++)
            {
                string file = ((char)('a' + i)).ToString();
                string rank = (i + 1).ToString();

                CreateLabel(file, boardView.GridToWorld(new Square(i, 0)) + Vector3.down);
                CreateLabel(rank, boardView.GridToWorld(new Square(0, i)) + Vector3.left);
            }
        }

        void CreateLabel(string text, Vector3 position)
        {
            var labelObject = new GameObject($"Label {text}");
            labelObject.transform.SetParent(transform, false);
            labelObject.transform.position = position;

            var label = labelObject.AddComponent<TextMeshPro>();
            label.text = text;
            if(font != null)
                label.font = font;
            label.fontSize = fontSize;
            label.color = color;
            label.alignment = TextAlignmentOptions.Center;
            label.sortingOrder = sortingOrder;
            label.rectTransform.sizeDelta = Vector2.one;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
