using System.Linq;
using UnityEngine;

namespace UI
{
    public class PlayerLabel : MonoBehaviour
    {
        public void DrawLabel(Camera camera)
        {
            if (camera == null)
                return;

            var style = new GUIStyle();
            style.normal.background = Texture2D.redTexture;
            style.normal.textColor = Color.blue;

            var position = camera.WorldToScreenPoint(transform.position);

            var collider = GetComponent<Collider>();
            if (collider != null && camera.Visible(collider))
            {
                var nameString = name;
                string[] names = nameString.Split('-');
                var nameResult = names[0];
                GUI.Label(new Rect(new Vector2(position.x, Screen.height - position.y), new Vector2(10, nameResult.Length * 10.5f)), nameResult, style);
            }
        }
    }
}
