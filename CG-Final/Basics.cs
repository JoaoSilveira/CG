using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CG_Final
{
    [Serializable]
    public class Vector
    {
        public double X;
        public double Y;
        public double Z;

        public Vector(double x = 0, double y = 0, double z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector(Point p) : this(p.X, p.Y, p.Z)
        {

        }

        public void Normalize()
        {
            var len = Math.Sqrt(X * X + Y * Y + Z * Z);

            X /= len;
            Y /= len;
            Z /= len;
        }

        public Vector VectorialProduct(Vector v)
        {
            return VectorialProduct(this, v);
        }

        public double DotProduct(Vector v)
        {
            return DotProduct(this, v);
        }

        public static Vector VectorialProduct(Vector v1, Vector v2)
        {
            return new Vector
            {
                X = v1.Y * v2.Z - v1.Z * v2.Y,
                Y = v1.Z * v2.X - v1.X * v2.Z,
                Z = v1.X * v2.Y - v1.Y * v2.X
            };
        }

        public static double DotProduct(Vector v1, Vector v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static Vector Normalize(Vector v)
        {
            var len = Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);

            return new Vector
            {
                X = v.X / len,
                Y = v.Y / len,
                Z = v.Z / len
            };
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector operator *(double scalar, Vector v)
        {
            return v * scalar;
        }

        public static Vector operator *(Vector v, double scalar)
        {
            return new Vector(v.X * scalar, v.Y * scalar, v.Z * scalar);
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector operator /(Vector a, double v)
        {
            return new Vector(a.X / v, a.Y / v, a.Z / v);
        }

        public override string ToString()
        {
            return $"({X:0.000}, {Y:0.000}, {Z:0.000})";
        }
    }

    [Serializable]
    public class Point
    {
        private const double e = .00001;
        public double X;
        public double Y;
        public double Z;

        public Point(Vector v) : this(v.X, v.Y, v.Z)
        {

        }

        public Point(double x = 0, double y = 0, double z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector operator +(Point p1, Point p2)
        {
            return new Vector
            (
                p1.X + p2.X,
                p1.Y + p2.Y,
                p1.Z + p2.Z
            );
        }

        public static Vector operator -(Point p1, Point p2)
        {
            return new Vector
            (
                p1.X - p2.X,
                p1.Y - p2.Y,
                p1.Z - p2.Z
            );
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point))
                return false;
            return Equals((Point)obj);
        }

        public bool Equals(Point other)
        {
            return Math.Abs(other.X - X) < e && Math.Abs(other.Y - Y) < e && Math.Abs(other.Z - Z) < e;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"({X:0.000}, {Y:0.000}, {Z:0.000})";
        }
    }

    [Serializable]
    public class Vertex
    {
        public double X;
        public double Y;
        public double Z;
        public Edge Edge;

        public static explicit operator Point(Vertex v)
        {
            return new Point(v.X, v.Y, v.Z);
        }

        public List<Face> GetSharedFaces()
        {
            var e = Edge;
            var first = Edge;
            var list = new List<Face>();

            do
            {
                if (this == Edge.Init)
                {
                    list.Add(e.Left);
                    e = e.LowerLeft;
                }
                else
                {
                    list.Add(e.Right);
                    e = e.UpperRight;
                }
            } while (e != first);

            return list;
        }

        public Vector GetAverageNormalVector()
        {
            var v = new Vector();
            var list = GetSharedFaces();
            var n = list.Count;

            foreach (var sharedFace in list)
            {
                v += sharedFace.NormalVector();
            }

            return Vector.Normalize(v / n);
        }
    }

    [Serializable]
    public class Matrix : IXmlSerializable
    {
        public const double DegreesToRadians = Math.PI / 180;

        private readonly double[,] _matrix;

        public Matrix()
        {
            _matrix = new double[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
        }

        public double this[int x, int y]
        {
            get { return _matrix[y, x]; }
            set { _matrix[y, x] = value; }
        }

        public void Concatenate(Matrix m)
        {
            var newV = (double[,])_matrix.Clone();

            this[0, 0] = newV[0, 0] * m[0, 0] + newV[0, 1] * m[0, 1] + newV[0, 2] * m[0, 2] + newV[0, 3] * m[0, 3];
            this[1, 0] = newV[0, 0] * m[1, 0] + newV[0, 1] * m[1, 1] + newV[0, 2] * m[1, 2] + newV[0, 3] * m[1, 3];
            this[2, 0] = newV[0, 0] * m[2, 0] + newV[0, 1] * m[2, 1] + newV[0, 2] * m[2, 2] + newV[0, 3] * m[2, 3];
            this[3, 0] = newV[0, 0] * m[3, 0] + newV[0, 1] * m[3, 1] + newV[0, 2] * m[3, 2] + newV[0, 3] * m[3, 3];

            this[0, 1] = newV[1, 0] * m[0, 0] + newV[1, 1] * m[0, 1] + newV[1, 2] * m[0, 2] + newV[1, 3] * m[0, 3];
            this[1, 1] = newV[1, 0] * m[1, 0] + newV[1, 1] * m[1, 1] + newV[1, 2] * m[1, 2] + newV[1, 3] * m[1, 3];
            this[2, 1] = newV[1, 0] * m[2, 0] + newV[1, 1] * m[2, 1] + newV[1, 2] * m[2, 2] + newV[1, 3] * m[2, 3];
            this[3, 1] = newV[1, 0] * m[3, 0] + newV[1, 1] * m[3, 1] + newV[1, 2] * m[3, 2] + newV[1, 3] * m[3, 3];

            this[0, 2] = newV[2, 0] * m[0, 0] + newV[2, 1] * m[0, 1] + newV[2, 2] * m[0, 2] + newV[2, 3] * m[0, 3];
            this[1, 2] = newV[2, 0] * m[1, 0] + newV[2, 1] * m[1, 1] + newV[2, 2] * m[1, 2] + newV[2, 3] * m[1, 3];
            this[2, 2] = newV[2, 0] * m[2, 0] + newV[2, 1] * m[2, 1] + newV[2, 2] * m[2, 2] + newV[2, 3] * m[2, 3];
            this[3, 2] = newV[2, 0] * m[3, 0] + newV[2, 1] * m[3, 1] + newV[2, 2] * m[3, 2] + newV[2, 3] * m[3, 3];

            this[0, 3] = newV[3, 0] * m[0, 0] + newV[3, 1] * m[0, 1] + newV[3, 2] * m[0, 2] + newV[3, 3] * m[0, 3];
            this[1, 3] = newV[3, 0] * m[1, 0] + newV[3, 1] * m[1, 1] + newV[3, 2] * m[1, 2] + newV[3, 3] * m[1, 3];
            this[2, 3] = newV[3, 0] * m[2, 0] + newV[3, 1] * m[2, 1] + newV[3, 2] * m[2, 2] + newV[3, 3] * m[2, 3];
            this[3, 3] = newV[3, 0] * m[3, 0] + newV[3, 1] * m[3, 1] + newV[3, 2] * m[3, 2] + newV[3, 3] * m[3, 3];
        }

        public void Translate(double x = 0, double y = 0, double z = 0)
        {
            Concatenate(TranslationMatrix(x, y, z));
        }

        public void Scale(double x = 1, double y = 1, double z = 1)
        {
            Concatenate(ScaleMatrix(x, y, z));
        }

        public void RotateX(double angle)
        {
            Concatenate(XRotationMatrix(angle));
        }

        public void RotateY(double angle)
        {
            Concatenate(YRotationMatrix(angle));
        }

        public void RotateZ(double angle)
        {
            Concatenate(ZRotationMatrix(angle));
        }

        public static Matrix TranslationMatrix(double x = 0, double y = 0, double z = 0)
        {
            return new Matrix
            {
                [3, 0] = x,
                [3, 1] = y,
                [3, 2] = z
            };
        }

        public static Matrix ScaleMatrix(double x = 1, double y = 1, double z = 1)
        {
            return new Matrix
            {
                [0, 0] = x,
                [1, 1] = y,
                [2, 2] = z
            };
        }

        public static Matrix XRotationMatrix(double angle)
        {
            var cos = Math.Cos(angle * DegreesToRadians);
            var sin = Math.Sin(angle * DegreesToRadians);

            return new Matrix
            {
                [1, 1] = cos,
                [2, 1] = -sin,
                [1, 2] = sin,
                [2, 2] = cos
            };
        }

        public static Matrix YRotationMatrix(double angle)
        {
            var cos = Math.Cos(angle * DegreesToRadians);
            var sin = Math.Sin(angle * DegreesToRadians);

            return new Matrix
            {
                [0, 0] = cos,
                [2, 0] = sin,
                [0, 2] = -sin,
                [2, 2] = cos
            };
        }

        public static Matrix ZRotationMatrix(double angle)
        {
            var cos = Math.Cos(angle * DegreesToRadians);
            var sin = Math.Sin(angle * DegreesToRadians);

            return new Matrix
            {
                [0, 0] = cos,
                [1, 0] = -sin,
                [0, 1] = sin,
                [1, 1] = cos
            };
        }

        public static Point operator *(Matrix m, Point p)
        {
            var val = new[]
            {
                m[0,0] * p.X + m[1,0] * p.Y + m[2,0] * p.Z + m[3,0],
                m[0,1] * p.X + m[1,1] * p.Y + m[2,1] * p.Z + m[3,1],
                m[0,2] * p.X + m[1,2] * p.Y + m[2,2] * p.Z + m[3,2],
                m[0,3] * p.X + m[1,3] * p.Y + m[2,3] * p.Z + m[3,3],
            };

            if (val[3] == 1)
                return new Point
                {
                    X = val[0],
                    Y = val[1],
                    Z = val[2]
                };

            val[0] /= val[3];
            val[1] /= val[3];
            val[2] /= val[3];

            return new Point
            {
                X = val[0],
                Y = val[1],
                Z = val[2]
            };
        }

        public override string ToString()
        {
            return $"{this[0, 0]} {this[1, 0]} {this[2, 0]} {this[3, 0]}\n" +
                $"{this[0, 1]} {this[1, 1]} {this[2, 1]} {this[3, 1]}\n" +
                $"{this[0, 2]} {this[1, 2]} {this[2, 2]} {this[3, 2]}\n" +
                $"{this[0, 3]} {this[1, 3]} {this[2, 3]} {this[3, 3]}";
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            List<string> l1 = null;
            List<string> l2 = null;
            List<string> l3 = null;
            List<string> l4 = null;

            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Element)
                    continue;
                switch (reader.Name)
                {
                    case "line1":
                        reader.Read();
                        l1 = reader.Value.Split().ToList();
                        break;
                    case "line2":
                        reader.Read();
                        l2 = reader.Value.Split().ToList();
                        break;
                    case "line3":
                        reader.Read();
                        l3 = reader.Value.Split().ToList();
                        break;
                    case "line4":
                        reader.Read();
                        l4 = reader.Value.Split().ToList();
                        break;
                }
            }

            for (var i = 0; i < 4; i++)
            {
                this[i, 0] = double.Parse(l1[i]);
                this[i, 1] = double.Parse(l2[i]);
                this[i, 2] = double.Parse(l3[i]);
                this[i, 3] = double.Parse(l4[i]);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("line1", $"{this[0, 0]} {this[1, 0]} {this[2, 0]} {this[3, 0]}");
            writer.WriteElementString("line2", $"{this[0, 1]} {this[1, 1]} {this[2, 1]} {this[3, 1]}");
            writer.WriteElementString("line3", $"{this[0, 2]} {this[1, 2]} {this[2, 2]} {this[3, 2]}");
            writer.WriteElementString("line4", $"{this[0, 3]} {this[1, 3]} {this[2, 3]} {this[3, 3]}");
        }
    }

    public class Edge
    {
        public Vertex Init;
        public Vertex End;
        public Face Left;
        public Face Right;
        public Edge UpperLeft;
        public Edge UpperRight;
        public Edge LowerLeft;
        public Edge LowerRight;
    }

    public class Face
    {
        public Edge Edge;

        public List<Edge> GetEdges()
        {
            var e = Edge;
            var list = new List<Edge>();

            do
            {
                list.Add(e);
                e = e.Left == this ? e.UpperLeft : e.LowerRight;
            } while (e != Edge);

            return list;
        }

        public List<Edge> GetEdgesClockWise()
        {
            var e = Edge;
            var list = new List<Edge>();

            do
            {
                list.Add(e);
                e = e.Left == this ? e.LowerLeft : e.UpperRight;
            } while (e != Edge);

            return list;
        }

        public List<Vertex> GetVerticesClockWise()
        {
            var e = Edge;
            var list = new List<Vertex>();

            do
            {
                list.Add(e.Left == this ? e.Init : e.End);
                e = e.Left == this ? e.LowerLeft : e.UpperRight;
            } while (e != Edge);

            return list;
        }

        public Vector NormalVector()
        {
            var p3 = (Point)(Edge.Left == this ? Edge.End : Edge.Init);
            var p2 = Edge.Left == this ? Edge.Init : Edge.End;
            var e = Edge.Left == this ? Edge.LowerLeft : Edge.UpperRight;
            var p1 = (Point)(e.Init == p2 ? e.End : e.Init);
            return
                Vector.Normalize(
                    (p3 - (Point)p2).VectorialProduct(p1 - (Point)p2));
            //var b = GetEdgeVector(Edge);
            //return
            //    Vector.Normalize(
            //        b.VectorialProduct(GetEdgeVector(Edge.Left == this ? Edge.LowerLeft : Edge.UpperRight)));
        }

        //public Vector GetEdgeVector(Edge e)
        //{
        //    if (this == e.Left)
        //        return (Point)(e.End) - (Point)(e.Init);

        //    return (Point)(e.Init) - (Point)(e.End);
        //}
    }
}