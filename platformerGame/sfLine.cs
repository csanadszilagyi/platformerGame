using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;

namespace platformerGame
{
    class sfLine : Drawable
    {

        private VertexArray vertices;
        //private float thickness;
        //private Color color;
        public sfLine(Vector2f point1, Vector2f point2, Color color, float thickness)
        {
            Vector2f direction = point2 - point1;
            Vector2f unitDirection = direction / (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
            Vector2f unitPerpendicular = new Vector2f(-unitDirection.Y, unitDirection.X);

            Vector2f offset = (thickness / 2.0f) * unitPerpendicular;
            vertices = new VertexArray(PrimitiveType.Quads);
            vertices.Append(new Vertex(point1 + offset, color));
            vertices.Append(new Vertex(point2 + offset, color));
            vertices.Append(new Vertex(point2 - offset, color));
            vertices.Append(new Vertex(point1 - offset, color));
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(vertices, states);
        }
    }
}
