using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RebarPartsListGenerator.Model
{
    public class Plane
    {
        private double X0 { get; set; }
        private double Y0 { get; set; }
        private double Z0 { get; set; }
        private double X1 { get; set; }
        private double Y1 { get; set; }
        private double Z1 { get; set; }
        private double X2 { get; set; }
        private double Y2 { get; set; }
        private double Z2 { get; set; }
        public double A { get; set; }
        public double B { get; private set; }
        public double C { get; private set; }
        public double D { get; private set; }
        /// <summary>
        /// Creating new plane instance by 3 points
        /// </summary>
        /// <param name="point0"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <exception cref="Exception">One of points is not in plane</exception>
        public Plane(XYZ point0, XYZ point1, XYZ point2)
        {
            X0 = point0.X; Y0 = point0.Y; Z0 = point0.Z;
            X1 = point1.X; Y1 = point1.Y; Z1 = point1.Z;
            X2 = point2.X; Y2 = point2.Y; Z2 = point2.Z;
            bool isIn2, isIn3;
            //Матрица имеет вид
            // |x-x0 x1-x0 x2-x0|
            // |y-y0 y1-y0 y2-y0| = 0
            // |z-z0 z1-z0 z2-z0|
            // Проверяем принадлежность 2 и 3 точки плоскости раскрывая определитель по 1 столбцу и подставляя соотв. значения x,y,z. 1 точку не проверяем, тождество всегда выполнится
            A = Math.Round((Y1 - Y0) * (Z2 - Z0) - (Y2 - Y0) * (Z1 - Z0), 4);
            B = Math.Round((X1 - X0) * (Z2 - Z0) - (X2 - X0) * (Z1 - Z0), 4);
            C = Math.Round((X1 - X0) * (Y2 - Y0) - (X2 - X0) * (Y1 - Y0), 4);
            D = Math.Round(-X0 * A + Y0 * B - Z0 * C, 4);
            isIn2 = Math.Round(A * X1 - B * Y1 + C * Z1 + D, 3) == 0;
            isIn3 = Math.Round(A * X2 - B * Y2 + C * Z2 + D, 3) == 0;

            if (!isIn3 || !isIn2)
            {
                throw new Exception("Убедитесь, что точки лежат в 1 плоскости. Какая- то из точек не удовлетворяет уравнению");
            }
        }
        public XYZ Normal { get => new XYZ(A, B, C); }        
    }
}
