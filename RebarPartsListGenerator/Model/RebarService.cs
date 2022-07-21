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
    }
}
