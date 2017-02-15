using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

/*
# | f | f |eae|ead|epe|epd|
1 | 1 | 2 | 3 | 4 | 2 | 5 |
2 | 1 | 3 | 1 | 5 | 3 | 6 |
3 | 1 | 4 | 2 | 6 | 1 | 4 |
4 | 2 | 4 | 1 | 3 | 7 | 9 |
5 | 3 | 2 | 2 | 1 | 8 | 7 |
6 | 4 | 3 | 3 | 2 | 9 | 8 |
7 | 5 | 2 | 4 | 9 | 5 | 8 |
8 | 5 | 3 | 5 | 7 | 6 | 9 |
9 | 5 | 4 | 6 | 8 | 4 | 7 |

    3                   n*3
    \>                  <-
                        </  n*3-1
    />                  <\
    1                   <- n*3-2
 */

namespace CG_Final
{
    class ObjectBase
    {
        public static int InitialVertices = 3;

        private Matrix _transformation;
        private Face _top;
        private Face _down;

        private List<Vertex> _vertices;
        private List<Edge> _edges;
        public List<Face> Faces { get; }

        public ObjectBase()
        {
            _transformation = new Matrix();

            Faces = new List<Face>();
            _edges = new List<Edge>();
            _vertices = new List<Vertex>();

            for (var i = 0; i < 5; i++)
                Faces.Add(new Face());

            for (var i = 0; i < 9; i++)
                _edges.Add(new Edge());

            for (var i = 0; i < 6; i++)
                _vertices.Add(new Vertex());

            _top = Faces[0];
            _down = Faces[4];

            Faces[0].Edge = Faces[1].Edge = _edges[0];
            Faces[2].Edge = _edges[1];
            Faces[3].Edge = _edges[2];
            Faces[4].Edge = _edges[6];

            _edges[0].Init = _vertices[0];
            _edges[0].End = _vertices[1];
            _edges[1].Init = _vertices[1];
            _edges[1].End = _vertices[2];
            _edges[2].Init = _vertices[2];
            _edges[2].End = _vertices[0];
            _edges[3].Init = _vertices[0];
            _edges[3].End = _vertices[3];
            _edges[4].Init = _vertices[1];
            _edges[4].End = _vertices[4];
            _edges[5].Init = _vertices[2];
            _edges[5].End = _vertices[5];
            _edges[6].Init = _vertices[3];
            _edges[6].End = _vertices[4];
            _edges[7].Init = _vertices[4];
            _edges[7].End = _vertices[5];
            _edges[8].Init = _vertices[5];
            _edges[8].End = _vertices[3];
            
            _edges[4].LowerRight = _edges[3].LowerLeft = _edges[2].UpperLeft = _edges[1].LowerLeft = _edges[0];
            _edges[5].LowerRight = _edges[4].LowerLeft = _edges[2].LowerLeft = _edges[0].UpperLeft = _edges[1];
            _edges[5].LowerLeft = _edges[3].LowerRight = _edges[1].UpperLeft = _edges[0].LowerLeft = _edges[2];
            _edges[8].UpperLeft = _edges[6].LowerLeft = _edges[2].UpperRight = _edges[0].LowerRight = _edges[3];
            _edges[7].LowerLeft = _edges[6].UpperLeft = _edges[1].LowerRight = _edges[0].UpperRight = _edges[4];
            _edges[8].LowerLeft = _edges[7].UpperLeft = _edges[2].LowerRight = _edges[1].UpperRight = _edges[5];
            _edges[8].UpperRight = _edges[7].LowerRight = _edges[4].UpperRight = _edges[3].UpperLeft = _edges[6];
            _edges[8].LowerRight = _edges[6].UpperRight = _edges[5].UpperRight = _edges[4].UpperLeft = _edges[7];
            _edges[7].UpperRight = _edges[6].LowerRight = _edges[5].UpperLeft = _edges[3].UpperRight = _edges[8];
        }

        public void ChangeVertices()
        {
            
        }

    }
}
