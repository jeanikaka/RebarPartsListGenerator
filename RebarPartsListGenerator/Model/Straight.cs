using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebarPartsListGenerator.Model
{
    public class Straight
    {
        public XYZ GuideVector { get; private set; }
        public double X0 { get; private set; }
        public double Y0 { get; private set; }
        public double Z0 { get; private set; }
        public XYZ Origin { get; private set; }
        /// <summary>
        /// Create a new instance of straight. If you don't have a guiding vector
        /// but have 2 points (point0, point1), guiding vector coords are the difference of 2 points
        /// </summary>
        /// <param name="point0"></param>
        /// <param name="point1"></param>
        public Straight(XYZ point0, XYZ point1)
        {
            Origin = new XYZ(point0.X, point0.Y, point0.Z);
            GuideVector = new XYZ(point1.X - point0.X, point1.Y - point0.Y, point1.Z - point0.Z);
        }
    }
}
