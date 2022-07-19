using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RebarPartsListGenerator.Model;

namespace RebarPartsListGenerator
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class PartsListGeneratorMain : IExternalCommand
    {
        Document _doc;
        Selection _sel;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {            
            try
            {
                _sel = commandData.Application.ActiveUIDocument.Selection;
                _doc = commandData.Application.ActiveUIDocument.Document;
                ScheduleService scheduleService = new ScheduleService(_doc, _sel);
                ViewSchedule rebarSchedule =  scheduleService.ViewScheduleFromSelection();

                RebarHolder rebarHolder = new RebarHolder(rebarSchedule, _doc);
                List<Rebar> rebars = rebarHolder.GetElements();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                return Result.Failed;
            }
        }
        public static void Main()
        {

        }

    }

}
