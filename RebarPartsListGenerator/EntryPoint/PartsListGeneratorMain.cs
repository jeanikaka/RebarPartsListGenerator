
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
            
                _sel = commandData.Application.ActiveUIDocument.Selection;
                _doc = commandData.Application.ActiveUIDocument.Document;

                ViewSchedule activeView = _doc.ActiveView as ViewSchedule;
                int selectedElemsCount = _sel.GetElementIds().Count;
                if (activeView == null || selectedElemsCount < 1)
                {
                    TaskDialog.Show("Ошибка", "Откройте спецификацию и выделите столбец перед запуском!", TaskDialogCommonButtons.Ok);
                    return Result.Failed;
                }
                RebarService rebarService = new RebarService(_sel, _doc);
                ModellingService modellingService = new ModellingService(_doc, _sel);
                List<Rebar> rebars = rebarService.RebarsFromCurSelected();
                List<Rebar> filteredRebars = rebarService.FilterRebars(rebars);
                ModellingService listModelling = new ModellingService(_doc, _sel);
                ViewDraftingService viewDraftingService = new ViewDraftingService(_doc, filteredRebars);

                using (Transaction transaction = new Transaction(_doc, "Создание ведомости деталей"))
                {
    
                    transaction.Start();                    
                    ViewDrafting viewDrafting = viewDraftingService.ViewDraftingCreate();                    
                    DetailCurveArray curve = listModelling.CreateTableHead(viewDrafting as View);
                    listModelling.CreatingMultipleTableBody(viewDrafting as View, filteredRebars);
                    listModelling.CreateScetchesFromRebars(viewDrafting, filteredRebars);
                    transaction.Commit();
                }

                return Result.Succeeded;
           
        }


    }

}
