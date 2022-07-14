using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace RebarPartsListGenerator
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class PartsListGeneratorMain : IExternalCommand
    {
        Document _doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                _doc = commandData.Application.ActiveUIDocument.Document;
                FilteredElementCollector collector = new FilteredElementCollector(_doc);
                ViewSchedule rebarSpec = collector.OfCategory(BuiltInCategory.OST_Schedules).WhereElementIsNotElementType().First(it => it.Name == "LC0000_Арматура_Cпецификация на жб конструкцию деталями_Без_IFC") as ViewSchedule;
                if (rebarSpec != null)
                {

                }


                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                return Result.Failed;
            }
            
        }
    }
}
