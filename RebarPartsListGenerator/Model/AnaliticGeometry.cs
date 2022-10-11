using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace RebarPartsListGenerator.Model
{
    public static class AnaliticGeometry
    {
        public static double ToRadians(double degrees)
        {
            return degrees * PI / 180;
        }
        public static double ToDegrees(double radians)
        {
            return radians * 180 / PI;
        }
        /// <summary>
        /// Calculates a scalar product between 2 vectors
        /// </summary>
        /// <returns>Negative number if angle between vectors is obtuse, 0 if angle is 90 degrees, positive if angle is sharp</returns>
        public static double ScalarProduct(XYZ vector0, XYZ vector1)
        {
            return vector0.X * vector1.X + vector0.Y * vector1.Y + vector0.Z * vector1.Z;
        }
        public static XYZ VectorProduct(XYZ vector0, XYZ vector1)
        {
            double A0, B0, C0;
            A0 = vector0.X; B0 = vector0.Y; C0 = vector0.Z;
            double A1, B1, C1;
            A1 = vector1.X; B1 = vector1.Y; C1 = vector1.Z;
            //Результат векторного произведения: нормаль к плоскости, в которой лежат 2 вектора - вектор, получаемый через нахождение определителя матрицы следующего вида:
            //                               |i   j  k|
            //result = [vector0 x vector1] = |A0 B0 C0|
            //                               |A1 B1 C1|
            XYZ result = new XYZ(B0 * C1 - B1 * C0, -(A0 * C1 - A1 * C0), A0 * B1 - B0 * A1);
            return result;
        }
        public static Straight IntersectXOYandPlane(Plane plane0)
        {
            //Plane XOY = new Plane(new XYZ(0, 0, 0), new XYZ(0, 1, 0), new XYZ(1, 0, 0));
            double A0, B0, C0, D0;
            A0 = plane0.A; B0 = plane0.B; C0 = plane0.C; D0= plane0.D;
            //double A1, B1, C1, D1;
            //A1 = XOY.A; B1 = XOY.B; C1 = XOY.C; D1 = XOY.D;
            //Составляем системы уравнений: 1- уравнение плоскости
            //2 - уравнение плоскости XOY : Z = 0
            //Обнуляем X или Y для поиска точки, которая пренадлежит прямой, по которой пересекаются плоскости. 
            //При обнулении одной из координат получим соответственно координаты точки, в которой прямая пересекает другую ось
            //Общий вид уравнений
            //A0*x + B0*y + C0*z + D0 = 0
            //A1*x + B1*y + C1*z + D1 = 0
            //в данном частном случае получаем следующий вид
            //A0*x + B0*y + C0*z + D0 = 0
            //A1*0 + B1*0 + z + 0 = 0
            //double z = 0;
            //Проверяем к-ты при y и x и обнуляем соотв. координату при выполнении условия
            //double y;
            //double x;
            //if (A0 != 0)
            //{
            //    y = 0;
            //    x = -D0 / A0;
            //}
            //else if (B0 != 0)
            //{
            //    x = 0;
            //    y = -D0 / B0;
            //}
            //else
            //{
            //    throw new Exception("Class AnaliticGeometry, method IntersectPlanes: division by zero");
            //}
            //XYZ point0 = new XYZ(x, y, z);
            //XYZ guidingVector = new XYZ(B0 * C1 - C0 * B1, C0 * A1 - A0 * C1, A0 * B1 - B0 * A1);
            //Straight intersectStraight = new Straight(point0, guidingVector);

            XYZ startPoint = new XYZ();
            XYZ endPoint = new XYZ();
            if (A0 == 0)
            {
                Straight OY = new Straight(new XYZ(0, 0, 0), new XYZ(0, 1, 0));
                startPoint = IntersectPlaneStraight(plane0, OY);
                endPoint = new XYZ(startPoint.X + 1, startPoint.Y, startPoint.Z);
            }
            else if(B0 == 0)
            {
                Straight OX = new Straight(new XYZ(0, 0, 0), new XYZ(1, 0, 0));
                startPoint = IntersectPlaneStraight(plane0, OX);
                endPoint = new XYZ(startPoint.X, startPoint.Y + 1, startPoint.Z);
            }
            else
            {
                Straight OX = new Straight(new XYZ(0, 0, 0), new XYZ(1, 0, 0));
                startPoint = IntersectPlaneStraight(plane0, OX);
                Straight OY = new Straight(new XYZ(0, 0, 0), new XYZ(0, 1, 0));
                endPoint = IntersectPlaneStraight(plane0, OY);
            }
            return new Straight(startPoint, endPoint);
        }
        public static XYZ IntersectPlaneStraight (Plane plane, Straight straight)
        {
            XYZ normalToPlane = plane.Normal;
            XYZ guideVector = straight.GuideVector;
            if (ScalarProduct(normalToPlane, guideVector) == 0)
            {
                throw new Exception("Прямая параллельна плоскости");
            }
            double A,B,C,D,P0,P1,P2,X0,Y0,Z0;
            A = plane.A; B = plane.B; C = plane.C; D = plane.D;
            P0 = guideVector.X;
            P1 = guideVector.Y;
            P2 = guideVector.Z;
            X0 = straight.X0;
            Y0 = straight.Y0;
            Z0 = straight.Z0;
            double t = -(A * X0 - B * Y0 - C * Z0 - D) / (A * P0 + B * P1 + C * P2);
            double Xin = P0 * t + X0;
            double Yin = P1 * t + Y0;
            double Zin = P2 * t + Z0;
            return new XYZ(Xin, Yin, Zin);
        }
        public static double AngleBetweenPlanesInDegrees(Plane plane0, Plane plane1)
        {
            double angleInDegrees = 0;
            XYZ normalToPlane0 = plane0.Normal;
            XYZ normalToPlane1 = plane1.Normal;
            if (ScalarProduct(normalToPlane0, normalToPlane1) == 0)
            {

                return 90;
            }
            double X0,Y0,Z0,X1,Y1,Z1;
            X0 = normalToPlane0.X; Y0 = normalToPlane0.Y; Z0 = normalToPlane0.Z;
            X1 = normalToPlane1.X; Y1 = normalToPlane1.Y; Z1 = normalToPlane1.Z;
            double angleInRadians = Math.Acos((X0 * X1 + Y0 * Y1 + Z0 * Z1) / (Sqrt(Pow(X0, 2) + Pow(Y0, 2) + Pow(Z0, 2)) * Sqrt(Pow(X1, 2) + Pow(Y1, 2) + Pow(Z1, 2))));
            angleInDegrees = ToDegrees(angleInRadians);
            if (angleInDegrees > 90)
            {
                return ToDegrees(PI - angleInRadians);
            }
            else
            {
                return angleInDegrees;
            }
        }
        /// <summary>
        /// This method is for calculating number of minors in the matrix
        /// </summary>
        /// <param name="rowNumber">Number of matrix rows</param>
        /// <param name="columnNumber">Number of matrix columns</param>
        /// <param name="orderNumber">Number of order that is needed to be found in the matrix</param>
        /// <param name=""></param>
        /// <returns></returns>
        public static double minorsNumber(int rowNumber, int columnNumber, int orderNumber)
        {
            return Combos(rowNumber, orderNumber) * Combos(columnNumber, orderNumber);
        }
        public static double Combos(int ofNumber, int orderNumber)
        {
            return Factorial(ofNumber) / (Factorial(orderNumber) * Factorial(ofNumber - orderNumber));
        }
        public static double Factorial(int number) 
        {
            double result = 1;
            if (number > 1)
            {
                for(int i = 2; i <= number; i++)
                {
                    result *= i;
                }
            }
            return result;

        }

    }
}
