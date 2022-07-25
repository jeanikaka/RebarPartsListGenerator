using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebarPartsListGenerator.Model
{
    public class ModellingService
    {
        Document _doc;
        public ModellingService(Document doc)
        {
            _doc = doc;
        }

        public DetailLine CreatingListOfParts(View view)
        {
            DetailLine curve1 = (DetailLine)_doc.Create.NewDetailCurve(view, Line.CreateBound(new XYZ(), new XYZ(2000/304.8,0,0)));
            return curve1;
        }       

    }
}
