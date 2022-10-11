using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RebarPartsListGenerator.Model.AnaliticGeometry;

namespace RebarPartsListGenerator.Model
{
    /// <summary>
    /// Class for work with abstract geometry
    /// </summary>
    public class GeometryService
    {
        public GeometryService()
        {

        }
        /// <summary>
        /// Creates a new DetailCurveArray from given coords of start and end points
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="view"></param>
        /// <param name="coords"></param>
        /// <returns></returns>
        public DetailCurveArray CurveArrayFromCoords(Document doc, View view, double[,,] coords, Transform transform = null)
        {
            CurveArray curveArray = new CurveArray();
            for (int i = 0; i < coords.GetLength(0); i++)//Each line
            {
                double x0 = ToFeet(coords[i, 0, 0]);
                double y0 = ToFeet(coords[i, 0, 1]);
                double z0 = ToFeet(coords[i, 0, 2]);

                double x1 = ToFeet(coords[i, 1, 0]);
                double y1 = ToFeet(coords[i, 1, 1]);
                double z1 = ToFeet(coords[i, 1, 2]);

                XYZ p0 = new XYZ(x0, y0, z0);
                XYZ p1 = new XYZ(x1, y1, z1);

                if (transform != null)
                {
                    curveArray.Append(Line.CreateBound(p0, p1).CreateTransformed(transform));
                }
                else
                {
                    curveArray.Append(Line.CreateBound(p0, p1));
                }
            }
            DetailCurveArray detailCurveArray = doc.Create.NewDetailCurveArray(view, curveArray);
            return detailCurveArray;
        }
        public List<Curve> GetCurvesFromCoords(double[,,] coords, Transform transform = null)
        {
            List<Curve> curveArray = new List<Curve>();
            for (int i = 0; i < coords.GetLength(0); i++)//Each line
            {
                double x0 = ToFeet(coords[i, 0, 0]);
                double y0 = ToFeet(coords[i, 0, 1]);
                double z0 = ToFeet(coords[i, 0, 2]);

                double x1 = ToFeet(coords[i, 1, 0]);
                double y1 = ToFeet(coords[i, 1, 1]);
                double z1 = ToFeet(coords[i, 1, 2]);

                XYZ p0 = new XYZ(x0, y0, z0);
                XYZ p1 = new XYZ(x1, y1, z1);

                if (transform != null)
                {
                    Line line = Line.CreateBound(p0, p1);
                    curveArray.Add(line.CreateTransformed(transform));

                }
                else
                {
                    Line line = Line.CreateBound(p0, p1);
                    curveArray.Add(line);
                }                
            }
            return curveArray;
        }
        ///// <summary>
        ///// This method gets center of bounding box which describes the geometry centers of curves.
        ///// </summary>
        ///// <param name="curves">Curves described by bounding box</param>
        ///// <returns>The center of bounding box which is going through geometry centers of curves</returns>
        //public XYZ GetBbCenterFromCurveList(List<Curve> curves)
        //{
        //    List<XYZ> allPoints = curves.Select(it =>
        //    {
        //        switch (it.GetType().Name)
        //        {
        //            case "Arc":
        //                Arc arc = (Arc)it;
        //                return arc.Center;

        //            case "Line":
        //                return it.Evaluate(0.5, true);
        //            case "HermiteSpline":
        //                HermiteSpline hermiteSpline = (HermiteSpline)it;
        //                List<XYZ> controlPoint = hermiteSpline.ControlPoints.ToList();
        //                return GetBbCenterFromXYZList(controlPoint);
        //            default:
        //                TaskDialog.Show("Ошибка", "Такой тип кривой не существует, добавьте описание для метода GetBbCenterFromCurveList типа " + it.GetType().Name, TaskDialogCommonButtons.Ok);
        //                throw new Exception();
        //        }
        //    }).ToList();
        //    XYZ bBCenter = GetBbCenterFromXYZList(allPoints);
        //    return bBCenter;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curves"></param>
        /// <param name="extrem">"Min", "Max" or "Center"</param>
        /// <returns></returns>
        public XYZ GetBbExtremFromCurves(List<Curve> curves, string extrem)
        {
            
            List<XYZ> allPoints = new List<XYZ>();
            foreach(Curve curve in curves)
            {
                allPoints.Add(curve.GetEndPoint(0));
                allPoints.Add(curve.GetEndPoint(1));
            }
            return GetBbCenterFromXYZList(allPoints)[extrem];
        }
        public List<Curve> MoveCurves(List<Curve> curves, XYZ startpoint, XYZ endpoint)
        {
            Transform moveTransform = Transform.CreateTranslation(new XYZ(endpoint.X - startpoint.X, endpoint.Y - startpoint.Y, -startpoint.Z));            
            return curves.Select(it => it.CreateTransformed(moveTransform)).ToList();
        } 
        public List<Curve> RotateCurves(List<Curve> curves)
        {
            XYZ axis = new XYZ();
            XYZ origin = new XYZ();
            double angleInRadians = 0;
            //Получение 3 точек плоскости, в которой лежит форма стержня
            XYZ point1 = curves[0].GetEndPoint(0);
            XYZ point2 = curves[0].GetEndPoint(1);
            XYZ point3 = curves[1].GetEndPoint(1);
            //Получение нормали к плоскости формы
            Plane XOY = new Plane(new XYZ(0,0,0), new XYZ(0, 1, 0), new XYZ(1, 0, 0));
            Plane curvesPlane = new Plane(point1, point2, point3);
            double angleInDegrees = AngleBetweenPlanesInDegrees(XOY, curvesPlane);
            angleInRadians = ToRadians(angleInDegrees);
            
            if (angleInRadians != 0)
            {
                Straight intersectStraigth = IntersectXOYandPlane(curvesPlane);
                axis = intersectStraigth.GuideVector;
                origin = intersectStraigth.Origin;
                Transform angleTransform = Transform.CreateRotationAtPoint(axis, -angleInRadians, origin);
                return curves.Select(it => it.CreateTransformed(angleTransform)).ToList();
            }
            return curves;
        }
        public double ToFeet(double millimeters)
        {
            return UnitUtils.ConvertToInternalUnits(millimeters, UnitTypeId.Millimeters);
        }
        public double FromFeet(double foots)
        {
            return UnitUtils.ConvertFromInternalUnits(foots, UnitTypeId.Millimeters);
        }
        public List<Curve> ScaleCurves(List<Curve> curves, XYZ scalePoint, double scaleKoeff)
        {
            Transform moveForScale = Transform.CreateTranslation(XYZ.Zero.Subtract(scalePoint));
            Transform scaleTrans = Transform.Identity.ScaleBasis(scaleKoeff);
            List<Curve> scaledCurves = curves.Select(it => it
            .CreateTransformed(moveForScale)
            .CreateTransformed(scaleTrans)
            .CreateTransformed(moveForScale.Inverse)).ToList();
            return scaledCurves;
        }
        public XYZ GetMinPointFromCurves(List<Curve> curves)
        {
            return new XYZ(
                    curves.Min(it => Math.Min(it.GetEndPoint(0).X, it.GetEndPoint(1).X)),
                    curves.Min(it => Math.Min(it.GetEndPoint(0).Y, it.GetEndPoint(1).Y)),
                    curves.Min(it => Math.Min(it.GetEndPoint(0).Z, it.GetEndPoint(1).Z)));
        }
        //public XYZ GetMaxPointFromCurves(List<Curve> curves)
        //{
        //    return new XYZ(
        //        curves.Max(it => Math.Max(it.GetEndPoint(0).X, it.GetEndPoint(1).X)),
        //        curves.Max(it => Math.Max(it.GetEndPoint(0).Y, it.GetEndPoint(1).Y)),
        //        curves.Max(it => Math.Max(it.GetEndPoint(0).Z, it.GetEndPoint(1).Z)));
        //}
        public Dictionary<string, XYZ> GetBbCenterFromXYZList(List<XYZ> points)
        {
            Dictionary<string, XYZ> curvesExtrems = new Dictionary<string, XYZ>();
            
            XYZ pMin = new XYZ(points.Min(it => it.X), points.Min(it => it.Y), points.Min(it => it.Z));
            XYZ pMax = new XYZ(points.Max(it => it.X), points.Max(it => it.Y), points.Max(it => it.Z));
            XYZ bBCenter = new XYZ((pMin.X + pMax.X) / 2, (pMin.Y + pMax.Y) / 2, (pMin.Z + pMax.Z) / 2);
            curvesExtrems.Add("Max", pMax);
            curvesExtrems.Add("Min", pMin);
            curvesExtrems.Add("Center", bBCenter);
            return curvesExtrems;
        }            
    }
}
