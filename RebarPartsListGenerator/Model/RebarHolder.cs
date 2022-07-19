using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebarPartsListGenerator.Model
{
    public class RebarHolder
    {

        ViewSchedule _rebarViewSchedule;
        Document _doc;
        public RebarHolder(ViewSchedule rebarViewSchedule, Document doc)
        {
            _rebarViewSchedule = rebarViewSchedule;
            _doc = doc;
        }
        public List<Rebar> GetElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(_doc, _rebarViewSchedule.Id);
            var rebarList = collector.OfCategory(BuiltInCategory.OST_Rebar).WhereElementIsNotElementType().Cast<Rebar>().Where(element =>
            {
                return _doc.GetElement(element.GetTypeId()).LookupParameter("NPP_R_Length_in_Running_Meters").AsInteger() == 1 ||
              element.LookupParameter("NPP_R__B ").AsDouble() > 0 ||
              element.LookupParameter("NPP_Обработка_Концов_0_1").AsInteger() == 1;
            }).ToList();
            return rebarList;
        }
    }
}
