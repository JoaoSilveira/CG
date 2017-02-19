using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CG_Final
{
    public class ObjectBase
    {
        public const int InitialVertices = 3;

        public static int count = 0;

        #region Fields
        private readonly Matrix _transformation;
        private readonly List<Vertex> _vertices;
        private readonly List<Edge> _edges;
        private Face _top;
        private Face _down;
        #endregion

        #region Properties
        public int Id { get; }
        public List<Face> Faces { get; }
        public List<Edge> Edges => _edges;
        public List<Vertex> Vertices => _vertices; 
        #endregion

        public ObjectBase()
        {
            Id = count++;
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

            InitializeBasicObject();
        }

        private void InitializeBasicObject()
        {
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

            _edges[0].Left = _edges[1].Left = _edges[2].Left = Faces[0];
            _edges[0].Right = _edges[3].Left = _edges[6].Left = _edges[4].Right = Faces[1];
            _edges[4].Left = _edges[7].Left = _edges[1].Right = _edges[5].Right = Faces[2];
            _edges[5].Left = _edges[8].Left = _edges[2].Right = _edges[3].Right = Faces[3];
            _edges[6].Right = _edges[7].Right = _edges[8].Right = Faces[4];

            _edges[4].LowerRight = _edges[3].LowerLeft = _edges[2].UpperLeft = _edges[1].LowerLeft = _edges[0];
            _edges[5].LowerRight = _edges[4].LowerLeft = _edges[2].LowerLeft = _edges[0].UpperLeft = _edges[1];
            _edges[5].LowerLeft = _edges[3].LowerRight = _edges[1].UpperLeft = _edges[0].LowerLeft = _edges[2];
            _edges[8].UpperLeft = _edges[6].LowerLeft = _edges[2].UpperRight = _edges[0].LowerRight = _edges[3];
            _edges[7].LowerLeft = _edges[6].UpperLeft = _edges[1].LowerRight = _edges[0].UpperRight = _edges[4];
            _edges[8].LowerLeft = _edges[7].UpperLeft = _edges[2].LowerRight = _edges[1].UpperRight = _edges[5];
            _edges[8].UpperRight = _edges[7].LowerRight = _edges[4].UpperRight = _edges[3].UpperLeft = _edges[6];
            _edges[8].LowerRight = _edges[6].UpperRight = _edges[5].UpperRight = _edges[4].UpperLeft = _edges[7];
            _edges[7].UpperRight = _edges[6].LowerRight = _edges[5].UpperLeft = _edges[3].UpperRight = _edges[8];

            _vertices[0].X = 50;
            _vertices[0].Y = -25;
            _vertices[0].Z = 0;
            _vertices[1].X = -25;
            _vertices[1].Y = -25;
            _vertices[1].Z = 43.3015;
            _vertices[2].X = -25;
            _vertices[2].Y = -25;
            _vertices[2].Z = -43.3015;
            _vertices[3].X = 50;
            _vertices[3].Y = 25;
            _vertices[3].Z = 0;
            _vertices[4].X = -25;
            _vertices[4].Y = 25;
            _vertices[4].Z = 43.3015;
            _vertices[5].X = -25;
            _vertices[5].Y = 25;
            _vertices[5].Z = -43.3015;
        }

        public void ChangeVertices(int n)
        {
            if (_vertices.Count > 6)
                _vertices.RemoveRange(6, _vertices.Count - 6);

            if (_edges.Count > 9)
                _edges.RemoveRange(9, _edges.Count - 9);

            if (Faces.Count > 5)
                Faces.RemoveRange(5, Faces.Count - 5);

            if (n == 0)
            {
                InitializeBasicObject();
                return;
            }

            var newv = new List<Vertex>();
            var newe = new List<Edge>();
            var newf = new List<Face>();

            for (var i = 0; i < n; i++)
            {
                newf.Add(new Face());
            }

            for (var i = 0; i < n * 2; i++)
            {
                newv.Add(new Vertex { X = 50, Y = i % 2 == 0 ? -25 : 25 });
            }

            for (var i = 0; i < n * 3; i++)
            {
                var e = new Edge { Left = newf[i / 3] };

                switch (i % 3)
                {
                    case 0:
                        e.Right = Faces[0];
                        e.Init = i == 0 ? _vertices[0] : newv[(i - 1) / 3 * 2];
                        e.End = newv[i / 3 * 2];
                        e.Left.Edge = e;
                        break;
                    case 1:
                        e.Right = i == n * 3 - 2 ? Faces[3] : newf[i / 3 + 1];
                        e.Init = newv[i / 3 * 2];
                        e.End = newv[i / 3 * 2 + 1];
                        break;
                    case 2:
                        e.Right = Faces[4];
                        e.End = i == 2 ? _vertices[3] : newv[(i - 3) / 3 * 2 + 1];
                        e.Init = newv[i / 3 * 2 + 1];
                        break;
                }
                e.Init.Edge = e;
                e.End.Edge = e;
                newe.Add(e);
            }

            for (var i = 0; i < n * 3; i++)
            {
                switch (i % 3)
                {
                    case 0:
                        if (i == 0)
                        {
                            newe[i].LowerLeft = _edges[3];
                            newe[i].LowerRight = _edges[0];
                            _edges[0].LowerLeft = newe[i];
                            _edges[3].LowerRight = newe[i];
                        }
                        else
                        {
                            newe[i].LowerLeft = newe[i - 2];
                            newe[i].LowerRight = newe[i - 3];
                        }
                        if (i == (n - 1) * 3)
                        {
                            newe[i].UpperLeft = newe[i + 1];
                            newe[i].UpperRight = _edges[2];
                            _edges[2].UpperLeft = newe[i];
                        }
                        else
                        {
                            newe[i].UpperLeft = newe[i + 1];
                            newe[i].UpperRight = newe[i + 3];
                        }
                        break;
                    case 1:
                        newe[i].UpperLeft = newe[i + 1];
                        newe[i].LowerLeft = newe[i - 1];
                        if (i == n * 3 - 2)
                        {
                            newe[i].LowerRight = _edges[2];
                            newe[i].UpperRight = _edges[8];
                            _edges[2].UpperRight = newe[i];
                            _edges[8].UpperLeft = newe[i];
                        }
                        else
                        {
                            newe[i].LowerRight = newe[i + 2];
                            newe[i].UpperRight = newe[i + 4];
                        }
                        break;
                    case 2:
                        if (i == 2)
                        {
                            newe[i].UpperLeft = _edges[3];
                            newe[i].UpperRight = _edges[6];
                            _edges[3].UpperRight = newe[i];
                            _edges[6].LowerRight = newe[i];
                        }
                        else
                        {
                            newe[i].UpperLeft = newe[i - 4];
                            newe[i].UpperRight = newe[i - 3];
                        }
                        newe[i].LowerLeft = newe[i - 1];

                        newe[i].LowerRight = i == n * 3 - 1 ? _edges[8] : newe[i + 3];
                        break;
                }
            }

            _edges[8].UpperRight = newe[n * 3 - 1];
            _edges[8].End = newv[n * 2 - 1];
            _edges[2].End = newv[(n - 1) * 2];
            _edges[3].Right = n == 0 ? Faces[3] : newf[0];

            var angle = 360.0 / (n + 3);
            var total = n * 2 - 2;

            for (var i = 0; i <= total; i += 2)
            {
                var m = Matrix.YRotationMatrix(angle * ((total - i) / 2 + 3));
                var p1 = m * (Point)newv[i];

                newv[i].X = p1.X;
                newv[i + 1].X = p1.X;
                newv[i].Z = p1.Z;
                newv[i + 1].Z = p1.Z;
            }

            _vertices[1].X = _vertices[4].X = _vertices[2].X = _vertices[5].X = _vertices[0].X;
            _vertices[1].Z = _vertices[4].Z = _vertices[2].Z = _vertices[5].Z = _vertices[0].Z;

            for (var i = 1; i < 3; i++)
            {
                var m = Matrix.YRotationMatrix(angle * i);
                var p1 = m * (Point)_vertices[i];

                _vertices[i].X = p1.X;
                _vertices[i + 3].X = p1.X;
                _vertices[i].Z = p1.Z;
                _vertices[i + 3].Z = p1.Z;
            }

            _vertices.AddRange(newv);
            _edges.AddRange(newe);
            Faces.AddRange(newf);
        }

        public Point TransformVertex(Vertex v)
        {
            return _transformation * (Point)v;
        }

        public override string ToString()
        {
            return $"Object - {Id}";
        }
    }
}
