using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebarPartsListGenerator.Model
{
    /// <summary>
    /// Фильтр, ограничивающий выбор заданными элементами модели. 
    /// Только они выделяются и могут быть выбраны при наведении курсора.
    /// </summary>
    public class SelectionFilter : ISelectionFilter
    {
        BuiltInCategory _category;
        public SelectionFilter(BuiltInCategory category)
        {
            _category = category;
        }
        public bool AllowElement(Element elem)
        {
            return (elem.Category.Id.IntegerValue == (int)_category);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
