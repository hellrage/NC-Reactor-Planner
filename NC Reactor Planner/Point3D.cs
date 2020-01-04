using System;


namespace NC_Reactor_Planner
{
    public struct Point3D
        : IEquatable<Point3D>
    {
        public static bool operator ==(Point3D left, Point3D right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }

        public static bool operator !=(Point3D left, Point3D right)
        {
            return !(left == right);
        }

        public static Point3D operator +(Point3D left, Vector3D right)
        {
            return new Point3D(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public double X { get; set; }
        
        public double Y { get; set; }
        
        public double Z { get; set; }

        public Point3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public bool Equals(Point3D other)
        {
            return this.X.Equals(other.X) && this.Y.Equals(other.Y) && this.Z.Equals(other.Z);
        }

        public override bool Equals(object obj)
        {
            return obj is Point3D other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.X.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Y.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Z.GetHashCode();
                return hashCode;
            }
        }
    }
}