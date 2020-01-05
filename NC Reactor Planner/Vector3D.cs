namespace NC_Reactor_Planner
{
    public struct Vector3D
    {
        public static Vector3D operator *(Vector3D left, int right)
        {
            return new Vector3D(left.X * right, left.Y * right, left.Z * right);
        }
        
        public double X { get; set; }
        
        public double Y { get; set; }
        
        public double Z { get; set; }

        public Vector3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}