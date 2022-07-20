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
        public List<Element> GetElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(_doc, _rebarViewSchedule.Id);
            var rebarList = collector.OfCategory(BuiltInCategory.OST_Rebar).OfClass(typeof(Rebar)).WhereElementIsNotElementType().ToList();
            
            List<Element> resultList = rebarList.Where(element =>
            {
                bool isNPP_R_Length_in_Running_Meters = _doc.GetElement(element.GetTypeId()).LookupParameter("NPP_R_Length_in_Running_Meters").AsInteger() == 1;
                bool isNPP_R__B = element.LookupParameter("NPP_R__B").AsDouble() > 0;
                bool isEndProcessing = element.LookupParameter("NPP_Обработка_Концов_0_1").AsInteger() == 1;
                bool allow = (isNPP_R_Length_in_Running_Meters || isNPP_R__B || isEndProcessing);
                return allow;
            }).ToList();


            return resultList;
        }
    }
}
