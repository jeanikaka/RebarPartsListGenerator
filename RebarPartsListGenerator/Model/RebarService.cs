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
        //public List<Curve> GetRebarCurves(Rebar rebar)
        //{
        //    List<Curve> rebarCurves = new List<Curve>();
        //    int n = rebar.NumberOfBarPositions;
        //    List<Curve> centerlineCurves = rebar.GetCenterlineCurves(adjustForSelfIntersection: true, suppressHooks: false, suppressBendRadius: false, MultiplanarOption.IncludeAllMultiplanarCurves, n).ToList();
        //    foreach (var c in centerlineCurves)
        //    {
        //        var accessor
        //                    = rebar.GetShapeDrivenAccessor();
        //        var trf = accessor
        //            .GetBarPositionTransform(n);

        //        if (rebar.get_Parameter(BuiltInParameter.REBAR_SHAPE).AsValueString().Contains("Форма арматурного стержня"))
        //        {

        //        }
        //        rebarCurves.Add(c.CreateTransformed(trf));
        //    }
        //    return rebarCurves;
        //}
        public List<Curve> GetRebarCurves(Rebar rebar)
        {
            RebarShape rebarShape = this._doc.GetElement(rebar.GetShapeId()) as RebarShape;
            List<Curve> shapeCurves = rebarShape.GetCurvesForBrowser().ToList();            
            List<Curve> centerlineCurves = rebar.GetCenterlineCurves(adjustForSelfIntersection: true, suppressHooks: false, suppressBendRadius: false, MultiplanarOption.IncludeOnlyPlanarCurves, 0).ToList();
            return centerlineCurves;
        }
        

        /// <summary>
        /// Use this method for filter rebar list by selected parameters values
        /// </summary>
        /// <param name="rebarList">List to filter</param>
        /// <returns>Filtered list of rebars</returns>
        public List<Rebar> FilterRebars(List<Rebar> rebarList)
        {
            List<Rebar> filteredRebars = new List<Rebar>();
            List<string> addedForms = new List<string>();
            return rebarList.Where(element =>
            {
                bool isNPP_R_Length_in_Running_Meters = _doc.GetElement(element.GetTypeId()).LookupParameter("NPP_R_Length_in_Running_Meters").AsInteger() == 1;
                bool isNPP_R__B = element.LookupParameter("NPP_R__B").AsDouble() > 0;
                bool isEndProcessing = element.LookupParameter("NPP_Обработка_Концов_0_1").AsInteger() == 1;
                string rebarForm = element.get_Parameter(BuiltInParameter.REBAR_SHAPE).AsValueString();                
                bool allow = (isNPP_R_Length_in_Running_Meters || isNPP_R__B || isEndProcessing) && !addedForms.Contains(rebarForm);
                if (!addedForms.Contains(rebarForm))
                {
                    addedForms.Add(rebarForm);
                }
                return allow;
            }).ToList();
        }
    }
}
