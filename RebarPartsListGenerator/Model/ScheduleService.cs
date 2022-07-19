using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace RebarPartsListGenerator.Model
{
    public class ScheduleService
    {
        private Document _doc;
        private Selection _sel;
        public ScheduleService(Document doc, Selection sel)
        {
            _doc = doc;
            _sel = sel;
        }

        public ViewSchedule ViewScheduleFromSelection()
        {            
            SelectionFilter schedulesFilter = new SelectionFilter(BuiltInCategory.OST_ScheduleGraphics);
            Reference pickedRef = _sel.PickObject(ObjectType.Element, schedulesFilter, "Select rebar schedule");
            ScheduleSheetInstance rebarScheduleInstance = _doc.GetElement(pickedRef) as ScheduleSheetInstance;
            ViewSchedule rebarSchedule = _doc.GetElement(rebarScheduleInstance.ScheduleId) as ViewSchedule;

            if (rebarSchedule != null)
            {
                return rebarSchedule;
            }
            else
            {
                return null;
            }

        }
    }
}
