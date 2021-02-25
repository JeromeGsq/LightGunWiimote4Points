namespace LightGunWiimote4Points.Models
{
    public class Position
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Position(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
