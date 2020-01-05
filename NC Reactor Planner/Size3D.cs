namespace NC_Reactor_Planner
{
    public struct Size3D
    {
        public double X { get; set; }
        
        public double Y { get; set; }
        
        public double Z { get; set; }

        public Size3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}