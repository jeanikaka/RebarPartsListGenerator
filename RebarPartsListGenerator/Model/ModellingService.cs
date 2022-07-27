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
        static Document _doc = null;
        public ModellingService(Document doc)
        {
            _doc = doc;
        }

        int lineStyle4BlackContId = 2255547; //Стиль линии "NPP_5_Сплошная_Черная"
        int lineStyle5BlackContId = 3062478; //Стиль линии "NPP_5_Сплошная_Черная"

        public DetailCurveArray CreateTableHead(View view)
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
                detailLine.LineStyle = _doc.GetElement(new ElementId(lineStyle4BlackContId)); ;//Id стиля линии "NPP_4_Сплошная_Черная"
            }

            XYZ itemTextNotePos = new XYZ(
                UnitUtils.ConvertToInternalUnits(1000, UnitTypeId.Millimeters),
                UnitUtils.ConvertToInternalUnits(-500, UnitTypeId.Millimeters),
                UnitUtils.ConvertToInternalUnits(0, UnitTypeId.Millimeters));
            TextNoteOptions textNoteOptions = new TextNoteOptions();
            textNoteOptions.HorizontalAlignment = HorizontalTextAlignment.Center;
            textNoteOptions.VerticalAlignment = VerticalTextAlignment.Middle;
            textNoteOptions.TypeId = new ElementId(1443588);
            TextNote itemTextNote = TextNote.Create(_doc, view.Id, itemTextNotePos, "Поз. Item", textNoteOptions);
            itemTextNote.Width = 0.0298288568924218;

            XYZ scetchTextNotePos = new XYZ(
                UnitUtils.ConvertToInternalUnits(5500, UnitTypeId.Millimeters),
                UnitUtils.ConvertToInternalUnits(-500, UnitTypeId.Millimeters),
                UnitUtils.ConvertToInternalUnits(0, UnitTypeId.Millimeters));
            TextNote scetchTextNote = TextNote.Create(_doc, view.Id, scetchTextNotePos, "Эскиз Scetch", textNoteOptions);
            scetchTextNote.Width = 0.0374925504375577;

            return detailCurveArray;
        }

        double[] _p0 = { 0, -1000, 0 };//The coors of last horizontal line startpoint of table head
        
        public void CreatingMultipleTableBody(View view)
        {
            double[] _p0 = this._p0;
            int startIndexPos = 1;
            for (int i = 0;  i < 4; i++)
            {
                this.CreatingTableBody(view, _p0, startIndexPos);
                _p0[1] -= 3000;
                startIndexPos++;
            }
        }

        public void CreatingTableBody(View view, double[] startPoint, int startIndexPos = 1)
        {
            DetailCurveArray detailCurveArray = new DetailCurveArray();
            CurveArray curveArray = new CurveArray();
            double x0 = _p0[0];
            double y0 = _p0[1];
            double z0 = _p0[2];

            double[,,] xYZs = {
            //Horizontal lines
             { {x0, y0 - 3000, z0}, { x0 + 9000, y0 - 3000, z0 } },
             //Vertical lines
             { {  x0, y0, z0 }, { x0 , y0 - 3000, z0 } },
             { {  x0 + 2000, y0, z0 }, { x0 + 2000, y0 - 3000, z0 } },
             { {  x0 + 9000, y0, z0 }, { x0 + 9000, y0 - 3000, z0 } }
            };

            for (int i = 0; i < xYZs.GetLength(0); i++)//Each line
            {
                double _x0 = UnitUtils.ConvertToInternalUnits(xYZs[i, 0, 0], UnitTypeId.Millimeters);
                double _y0 = UnitUtils.ConvertToInternalUnits(xYZs[i, 0, 1], UnitTypeId.Millimeters);
                double _z0 = UnitUtils.ConvertToInternalUnits(xYZs[i, 0, 2], UnitTypeId.Millimeters);

                double _x1 = UnitUtils.ConvertToInternalUnits(xYZs[i, 1, 0], UnitTypeId.Millimeters);
                double _y1 = UnitUtils.ConvertToInternalUnits(xYZs[i, 1, 1], UnitTypeId.Millimeters);
                double _z1 = UnitUtils.ConvertToInternalUnits(xYZs[i, 1, 2], UnitTypeId.Millimeters);

                XYZ p0 = new XYZ(_x0, _y0, _z0);
                XYZ p1 = new XYZ(_x1, _y1, _z1);
                curveArray.Append(Line.CreateBound(p0, p1));
            }
            detailCurveArray = _doc.Create.NewDetailCurveArray(view, curveArray);

            foreach (DetailLine detailLine in detailCurveArray)
            {
                detailLine.LineStyle = _doc.GetElement(new ElementId(lineStyle4BlackContId));//стиль "NPP_4_Сплошная_Черная"
            }

            string posText = startIndexPos.ToString();
            XYZ posTextNotePos = new XYZ(
                UnitUtils.ConvertToInternalUnits(x0 + 1000, UnitTypeId.Millimeters),
                UnitUtils.ConvertToInternalUnits(y0 -1500, UnitTypeId.Millimeters),
                UnitUtils.ConvertToInternalUnits(0, UnitTypeId.Millimeters));
            TextNoteOptions textNoteOptions = new TextNoteOptions();
            textNoteOptions.HorizontalAlignment = HorizontalTextAlignment.Center;
            textNoteOptions.VerticalAlignment = VerticalTextAlignment.Middle;
            textNoteOptions.TypeId = new ElementId(1443588);
            TextNote itemTextNote = TextNote.Create(_doc, view.Id, posTextNotePos, posText, textNoteOptions);
            itemTextNote.Width = 0.0298288568924218;


        }


    }
}
