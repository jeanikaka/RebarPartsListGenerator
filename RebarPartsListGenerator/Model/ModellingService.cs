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
            List<ElementId> lineIds = curve1.GetLineStyleIds().ToList();
            ElementId blackLine5Id = lineIds.Find(it => it.IntegerValue == 3062478);//Id стиля линии "NPP_5_Сплошная_Черная"
            curve1.LineStyle = _doc.GetElement(blackLine5Id);
            return curve1;
        }
        public DetailCurveArray TableHead(View view)
        {
            DetailCurveArray detailCurveArray = new DetailCurveArray();
            CurveArray curveArray = new CurveArray();
            double[,,] xYZs = {
            //Horizontal lines
             { {  0, 0, 0 }, { 9000, 0, 0 } } ,
             { {  0, -1000, 0 }, { 9000, -1000, 0 } },
             //Vertical lines
             { {  0, 0, 0 }, { 0, -1000, 0 } },
             { {  2000, 0, 0 }, { 2000, -1000, 0 } },
             { {  9000, 0, 0 }, { 9000, -1000, 0 } }
            };

            for (int i = 0; i < xYZs.GetLength(0); i++)//Each line
            {
                double x0 = UnitUtils.ConvertToInternalUnits(xYZs[i, 0, 0], UnitTypeId.Millimeters);
                double y0 = UnitUtils.ConvertToInternalUnits(xYZs[i, 0, 1], UnitTypeId.Millimeters);
                double z0 = UnitUtils.ConvertToInternalUnits(xYZs[i, 0, 2], UnitTypeId.Millimeters);

                double x1 = UnitUtils.ConvertToInternalUnits(xYZs[i, 1, 0], UnitTypeId.Millimeters);
                double y1 = UnitUtils.ConvertToInternalUnits(xYZs[i, 1, 1], UnitTypeId.Millimeters);
                double z1 = UnitUtils.ConvertToInternalUnits(xYZs[i, 1, 2], UnitTypeId.Millimeters);

                XYZ p0 = new XYZ(x0, y0, z0);
                XYZ p1 = new XYZ(x1, y1, z1);
                curveArray.Append(Line.CreateBound(p0, p1));
            }
            detailCurveArray = _doc.Create.NewDetailCurveArray(view, curveArray);
            
            foreach(DetailLine detailLine in detailCurveArray)
            {
                detailLine.LineStyle = _doc.GetElement(new ElementId(3062478));//Id стиля линии "NPP_5_Сплошная_Черная"
            }



            return detailCurveArray;
        }


    }
}
