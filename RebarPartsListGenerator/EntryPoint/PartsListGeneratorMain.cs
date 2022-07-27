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

                ViewSchedule activeView = _doc.ActiveView as ViewSchedule;
                int selectedElemsCount = _sel.GetElementIds().Count;
                if (activeView == null || selectedElemsCount < 1)
                {
                    TaskDialog.Show("Ошибка", "Откройте спецификацию и выделите столбец перед запуском!", TaskDialogCommonButtons.Ok);
                    return Result.Failed;
                }

                RebarService rebarService = new RebarService(_sel, _doc);
                List<Rebar> rebars = rebarService.RebarsFromCurSelected();
                List<Rebar> filteredRebars = this.FilterRebars(rebars);

                using (Transaction transaction = new Transaction(_doc, "Создание ведомости деталей"))
                {
                    transaction.Start();
                    ViewDraftingService viewDraftingService = new ViewDraftingService(_doc, filteredRebars);
                    ViewDrafting viewDrafting = viewDraftingService.ViewDraftingCreate();
                    ModellingService listModelling = new ModellingService(_doc);
                    DetailCurveArray curve = listModelling.CreateTableHead(viewDrafting as View);
                    listModelling.CreatingMultipleTableBody(viewDrafting as View);

                    transaction.Commit();
                }
                IList<Curve> rebarCurves = rebarService.GetRebarCurves(filteredRebars);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                return Result.Failed;
            }
        }
        /// <summary>
        /// Use this method for filter rebar list by selected parameters values
        /// </summary>
        /// <param name="rebarList">List to filter</param>
        /// <returns>Filtered list of rebars</returns>
        private List<Rebar> FilterRebars(List<Rebar> rebarList)
        {
            return rebarList.Where(element =>
            {
                bool isNPP_R_Length_in_Running_Meters = _doc.GetElement(element.GetTypeId()).LookupParameter("NPP_R_Length_in_Running_Meters").AsInteger() == 1;
                bool isNPP_R__B = element.LookupParameter("NPP_R__B").AsDouble() > 0;
                bool isEndProcessing = element.LookupParameter("NPP_Обработка_Концов_0_1").AsInteger() == 1;
                bool allow = (isNPP_R_Length_in_Running_Meters || isNPP_R__B || isEndProcessing);
                return allow;
            }).ToList();
        }

    }

}
