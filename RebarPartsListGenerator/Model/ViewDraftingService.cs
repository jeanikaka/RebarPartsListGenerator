using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebarPartsListGenerator.Model
{
    /// <summary>
    /// Class for work with Drafting view
    /// </summary>
    public class ViewDraftingService
    {
        private Document _doc;
        private string _viewNameV;
        private string nPPtranslateV = "LIST OF PARTS";
        private string headOnSheetV = "ВЕДОМОСТЬ ДЕТАЛЕЙ";

        /// <summary>
        /// Use this constructor if working with Rebars
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="filteredRebars">Filtered rebars from model</param>
        public ViewDraftingService(Document doc, List<Rebar> filteredRebars)
        {
            _doc = doc;
            _viewNameV = filteredRebars.First().LookupParameter(P.nPPcomplectNumber).AsValueString() + "_" +
                                filteredRebars.First().LookupParameter(P.nPPmodelFilter).AsValueString() + "_" + "ВЕДОМОСТЬ ДЕТАЛЕЙ";
        }
        /// <summary>
        /// Method for creating a list of parts in drafting view 
        /// </summary>
        /// <returns>The newly created list of parts represented in drafting view</returns>
        public ViewDrafting ViewDraftingCreate()
        {
            FilteredElementCollector filteredElems = new FilteredElementCollector(_doc).OfClass(typeof(ViewFamilyType));
            ViewFamilyType viewFamilyType = filteredElems.Cast<ViewFamilyType>().FirstOrDefault(vft => vft.ViewFamily == ViewFamily.Drafting);
            ViewDrafting viewDrafting = ViewDrafting.Create(_doc, viewFamilyType.Id);
            //Для смены типа семейства
            //ElementId typeId = viewDrafting.GetValidTypes().First(it => _doc.GetElement(it).LookupParameter("Имя типа").AsValueString() == "NPP_Имя вида с переводом");
            //viewDrafting.ChangeTypeId(typeId);
            viewDrafting.LookupParameter(P.viewName).Set(_viewNameV);
            viewDrafting.LookupParameter(P.headOnSheet).Set(headOnSheetV);
            viewDrafting.LookupParameter(P.nPPtranslate).Set(nPPtranslateV);
            return viewDrafting;
        }   
    }
}
