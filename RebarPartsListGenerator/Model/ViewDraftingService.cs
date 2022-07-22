using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebarPartsListGenerator.Model
{
    public class ViewDraftingService
    {
        private Document _doc;
        private string nameP = "Имя вида";
        private string namePV = "ВЕДОМОСТЬ ДЕТАЛЕЙ";
        private string nameTranslatedP = "NPP_Перевод";
        private string nameTranslatedPV = "LIST OF PARTS";
        private string setNumberP = "NPP_Номер_Комплекта";
        private string setNumberPV;
        private string modelFilterP = "NPP_Фильтрация_Модели";
        private string modelFilterPV;
        public ViewDraftingService(Document doc)
        {
            _doc = doc;            
        }
        public ViewDrafting ViewDraftingCreate(string viewFamilyTypeName)
        {
            FilteredElementCollector filteredElems = new FilteredElementCollector(_doc).OfClass(typeof(ViewFamilyType));
            ViewFamilyType viewFamilyType = filteredElems.Cast<ViewFamilyType>().FirstOrDefault(vft => vft.ViewFamily == ViewFamily.Drafting);
            ViewDrafting viewDrafting = ViewDrafting.Create(_doc, viewFamilyType.Id);
            viewDrafting.LookupParameter(nameP).Set(namePV);
            viewDrafting.LookupParameter(nameTranslatedP).Set(nameTranslatedPV);
            return viewDrafting;
        }   
    }
}
