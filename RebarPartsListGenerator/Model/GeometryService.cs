using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// This method gets center of bounding box which describes the geometry centers of curves.
        /// </summary>
        /// <param name="curves">Curves described by bounding box</param>
        /// <returns>The center of bounding box which is going through geometry centers of curves</returns>
        public XYZ GetBbCenterFromCurveList(List<Curve> curves)
        {
            List<XYZ> allPoints = curves.Select(it => {
                if (it is Arc)
                {
                    Arc arc = (Arc)it;
                    return arc.Center;
                }
                else if (it is Line line)
                {
                    return it.Evaluate(0.5, true);
                }
                else
                {
                    TaskDialog taskDialog = new TaskDialog("Ошибка типа кривой в методе GetBoundingBoxCenter!");
                    return null;
                }
            }).ToList();
            XYZ pMin = new XYZ(allPoints.Min(it => it.X), allPoints.Min(it => it.Y), allPoints.Min(it => it.Z));
            XYZ pMax = new XYZ(allPoints.Max(it => it.X), allPoints.Max(it => it.Y), allPoints.Max(it => it.Z));
            XYZ bBCenter = new XYZ((pMin.X + pMax.X) / 2, (pMin.Y + pMax.Y) / 2, (pMin.Z + pMax.Z) / 2);
            return bBCenter;
        }

        public List<Curve> moveCurves(List<Curve> curves, XYZ startpoint, XYZ endpoint)
        {
            Transform moveTransform = Transform.CreateTranslation(new XYZ(endpoint.X - startpoint.X, endpoint.Y - startpoint.Y, endpoint.Z - endpoint.Z));
            return curves.Select(it => it.CreateTransformed(moveTransform)).ToList();
        } 
        public double ToFeet(double millimeters)
        {
            return UnitUtils.ConvertToInternalUnits(millimeters, UnitTypeId.Millimeters);
        }
    }
}
