using UnityEngine;

namespace MrRabbit
{
    // A lightweight, replaceable placeholder drawn in a single UI mesh.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class RabbitGraphic : UnityEngine.UI.MaskableGraphic
    {
        private static readonly Color Ink = new Color32(76, 66, 58, 255);
        private static readonly Color Fur = new Color32(255, 252, 242, 255);
        private static readonly Color Pink = new Color32(238, 170, 164, 255);
        private static readonly Color Blue = new Color32(111, 164, 169, 255);

        protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper mesh)
        {
            mesh.Clear();
            Ellipse(mesh, .5f, .055f, .32f, .035f, new Color32(92, 73, 57, 40));
            Outlined(mesh, .27f, .82f, .09f, .17f, Fur, -10);
            Outlined(mesh, .69f, .82f, .09f, .17f, Fur, 9);
            Ellipse(mesh, .27f, .82f, .041f, .12f, Pink, -10);
            Ellipse(mesh, .69f, .82f, .041f, .12f, Pink, 9);
            Outlined(mesh, .70f, .24f, .12f, .078f, Fur);
            Outlined(mesh, .35f, .09f, .15f, .06f, Fur);
            Outlined(mesh, .62f, .09f, .15f, .06f, Fur);
            Outlined(mesh, .49f, .29f, .235f, .195f, Blue);
            Outlined(mesh, .26f, .34f, .08f, .11f, Fur, -17);
            Outlined(mesh, .72f, .34f, .08f, .11f, Fur, 17);
            Ellipse(mesh, .49f, .31f, .12f, .115f, new Color32(153, 194, 190, 255));
            Outlined(mesh, .49f, .57f, .30f, .19f, Fur);
            Ellipse(mesh, .31f, .535f, .055f, .032f, new Color32(248, 203, 190, 255));
            Ellipse(mesh, .67f, .535f, .055f, .032f, new Color32(248, 203, 190, 255));
            Ellipse(mesh, .39f, .60f, .025f, .032f, Ink);
            Ellipse(mesh, .59f, .60f, .025f, .032f, Ink);
            Ellipse(mesh, .49f, .545f, .035f, .026f, Pink);
            Ellipse(mesh, .46f, .505f, .026f, .012f, Ink, -12);
            Ellipse(mesh, .52f, .505f, .026f, .012f, Ink, 12);
        }

        private void Outlined(UnityEngine.UI.VertexHelper mesh, float x, float y,
            float rx, float ry, Color fill, float angle = 0)
        {
            Ellipse(mesh, x, y, rx + .012f, ry + .009f, Ink, angle);
            Ellipse(mesh, x, y, rx, ry, fill, angle);
        }

        private void Ellipse(UnityEngine.UI.VertexHelper mesh, float x, float y,
            float rx, float ry, Color fill, float angle = 0)
        {
            const int segments = 24;
            Rect rect = rectTransform.rect;
            var center = new Vector2(rect.xMin + x * rect.width, rect.yMin + y * rect.height);
            int first = mesh.currentVertCount;
            Color tint = fill * color;
            mesh.AddVert(center, tint, Vector2.zero);
            float rotation = angle * Mathf.Deg2Rad;
            float sin = Mathf.Sin(rotation);
            float cos = Mathf.Cos(rotation);
            for (int i = 0; i < segments; i++)
            {
                float a = i * Mathf.PI * 2f / segments;
                var point = new Vector2(Mathf.Cos(a) * rx * rect.width, Mathf.Sin(a) * ry * rect.height);
                point = new Vector2(point.x * cos - point.y * sin, point.x * sin + point.y * cos);
                mesh.AddVert(center + point, tint, Vector2.zero);
                mesh.AddTriangle(first, first + 1 + i, first + 1 + (i + 1) % segments);
            }
        }
    }
}
