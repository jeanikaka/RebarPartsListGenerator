using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebarPartsListGenerator.Model
{
    /// <summary>
    /// Class for working with Rebar
    /// </summary>
    public class RebarService
    {
        Selection _sel;
        Document _doc;
        /// <summary>
        /// Class for working with Rebar
        /// </summary>
        public RebarService(Selection sel, Document doc)
        {
            _sel = sel;
            _doc = doc;
        }
        /// <summary>
        /// Returns the Rebars from currently selected elements
        /// </summary>
        /// <returns>List of currently selected elements</returns>

        public List<Rebar> RebarsFromCurSelected()
        {
            return new FilteredElementCollector(_doc, _sel.GetElementIds())
            .WhereElementIsNotElementType()
            .OfCategory(BuiltInCategory.OST_Rebar)
            .OfClass(typeof(Rebar))
            .Cast<Rebar>()
            .ToList();
        }

        public List<Curve> GetRebarCurves(Rebar rebar)
        {
            List<Curve> rebarCurves = new List<Curve>();
            int n = rebar.NumberOfBarPositions;
            RebarShape rebarShape = this._doc.GetElement(rebar.GetShapeId()) as RebarShape;
            List<Curve> shapeCurves = rebarShape.GetCurvesForBrowser().ToList();
            for (int i = 0; i < n; i++)
            {
                //List<Curve> curves = rebar.GetTransformedCenterlineCurves(adjustForSelfIntersection: true, suppressHooks: false, suppressBendRadius: false, MultiplanarOption.IncludeOnlyPlanarCurves, i).ToList();
                List<Curve> centerlineCurves = rebar.GetCenterlineCurves(adjustForSelfIntersection: true, suppressHooks: false, suppressBendRadius: false, MultiplanarOption.IncludeAllMultiplanarCurves, i).ToList();
                Transform transform = rebar.GetMovedBarTransform(i);
                if (rebar.IsRebarShapeDriven()) //Если арматура по форме
                {

                    var accessor
                        = rebar.GetShapeDrivenAccessor();

                    var trf = accessor
                        .GetBarPositionTransform(i);


                    foreach (var c in shapeCurves)
                    {
                        rebarCurves.Add(c);
                    }
                }
                else// Произвольная форма
                {
                    foreach (var c in centerlineCurves)
                    {
                        rebarCurves.Add(c);
                    }
                }

            }
            return rebarCurves;
        }
        

    }
}
