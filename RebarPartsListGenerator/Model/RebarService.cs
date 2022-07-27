using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
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
        public IList<Curve> GetRebarCurves(List<Rebar> rebarList)
        {

            IList<Curve> rebarCurves = new List<Curve>();
            Rebar rebar = rebarList[0];
            int n = rebar.NumberOfBarPositions;
            for (int i = 0; i < n; i++)
            {
                IList<Curve> centerlineCurves = rebar.GetCenterlineCurves(adjustForSelfIntersection: true, suppressHooks: false, suppressBendRadius: false, MultiplanarOption.IncludeOnlyPlanarCurves, i);

                if (rebar.IsRebarShapeDriven()) //Если арматура по форме
                {
                    var accessor
                        = rebar.GetShapeDrivenAccessor();

                    var trf = accessor
                        .GetBarPositionTransform(i);


                    foreach (var c in centerlineCurves)
                    {
                        rebarCurves.Add(c.CreateTransformed(trf));
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
