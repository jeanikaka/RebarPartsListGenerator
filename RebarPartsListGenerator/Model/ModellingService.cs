using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;



namespace RebarPartsListGenerator.Model
{
    public class ModellingService
    {
        static Document _doc = null;
        static Selection _sel = null;
        public ModellingService(Document doc, Selection sel)
        {
            _doc = doc;
            _sel = sel;
        }
        RebarService _rebarService = new RebarService(_sel, _doc);
        static GeometryService _gs = new GeometryService();

        int lineStyle4BlackContId = 2255547; //Стиль линии "NPP_4_Сплошная_Черная"
        int lineStyle5BlackContId = 3062478; //Стиль линии "NPP_5_Сплошная_Черная"
        int textTypeArial_3_08Id = 1443588; //Стиль текста "NPP_Arial_3_0.8"
        int textTypeArial_25_08Id = 1443584; //Стиль текста "NPP_Arial_2.5_0.8"
        
        public TextNoteType CreateTransparentTextNoteType(Document doc, int existingElementTypeId)
        {
            TextNoteType existingTextNoteType = doc.GetElement(new ElementId(existingElementTypeId)) as TextNoteType;
            string existingTypeName = existingTextNoteType.get_Parameter(BuiltInParameter.SYMBOL_NAME_PARAM).AsValueString();

            bool transparentTextTypeExist = new FilteredElementCollector(doc).OfClass(typeof(TextNoteType))
                .Any(it => it.get_Parameter(BuiltInParameter.SYMBOL_NAME_PARAM).AsValueString() == existingTypeName + "_Прозрачный");
            if (transparentTextTypeExist)
            {
                return new FilteredElementCollector(doc).OfClass(typeof(TextNoteType))
                .First(it => it.get_Parameter(BuiltInParameter.SYMBOL_NAME_PARAM).AsValueString() == existingTypeName + "_Прозрачный") as TextNoteType;
            }
            else 
            {
                TextNoteType newTextNoteType = existingTextNoteType.Duplicate("NPP_Arial_2.5_0.8_Прозрачный") as TextNoteType;
                newTextNoteType.get_Parameter(BuiltInParameter.TEXT_BACKGROUND).Set(1);//1- прозрачный, 0- непрозрачный
                return newTextNoteType;
            }
        }
         
        public DetailCurveArray CreateTableHead(View view)
        {
            double[,,] xYZs = {
            //Horizontal lines
             { {  0, 0, 0 }, { 9000, 0, 0 } } ,
             { {  0, -1000, 0 }, { 9000, -1000, 0 } },
             //Vertical lines
             { {  0, 0, 0 }, { 0, -1000, 0 } },
             { {  2000, 0, 0 }, { 2000, -1000, 0 } },
             { {  9000, 0, 0 }, { 9000, -1000, 0 } }
            };
            DetailCurveArray detailCurveArray = _gs.CurveArrayFromCoords(_doc, view, xYZs);     
            
            foreach(DetailLine detailLine in detailCurveArray)
            {
                detailLine.LineStyle = _doc.GetElement(new ElementId(lineStyle4BlackContId)); ;//Id стиля линии "NPP_4_Сплошная_Черная"
            }

            XYZ itemTextNotePos = new XYZ(_gs.ToFeet(1000), _gs.ToFeet(-500), _gs.ToFeet(0));
            TextNoteOptions textNoteOptions = new TextNoteOptions();
            textNoteOptions.HorizontalAlignment = HorizontalTextAlignment.Center;
            textNoteOptions.VerticalAlignment = VerticalTextAlignment.Middle;
            textNoteOptions.TypeId = new ElementId(textTypeArial_3_08Id);

            TextNote itemTextNote = TextNote.Create(_doc, view.Id, itemTextNotePos, "Поз. Item", textNoteOptions);
            itemTextNote.Width = 0.0298288568924218;

            XYZ scetchTextNotePos = new XYZ(_gs.ToFeet(5500), _gs.ToFeet(-500), _gs.ToFeet(0));
            TextNote scetchTextNote = TextNote.Create(_doc, view.Id, scetchTextNotePos, "Эскиз Scetch", textNoteOptions);
            scetchTextNote.Width = 0.0374925504375577;

            return detailCurveArray;
        }

        public double[] _p0 = { 0, -1000, 0 };//The coords of last horizontal line startpoint of table head
        public double[] _pC = { 5500, -2500, 0 };//The coords of first tablebody

        public List<XYZ> _tableBodysCenters = new List<XYZ>();
        public void CreatingMultipleTableBody(View view, List<Rebar> rebars)
        {
            double[] _p0 = this._p0;
            double[] _pC = this._pC;
            _tableBodysCenters.Add(new XYZ(_gs.ToFeet(_pC[0]), _gs.ToFeet(_pC[1]), _gs.ToFeet(_pC[2])));

            int quantity = rebars.Count;
            for (int i = 0;  i < quantity; i++)
            {
                string indexPos = rebars[i].LookupParameter("NPP_Позиция").AsValueString() ?? " ";
                this.CreatingTableBody(view, _p0, indexPos);
                _p0[1] -= 3000;
                _pC[1] -= 3000;
                _tableBodysCenters.Add(new XYZ(_gs.ToFeet(_pC[0]), _gs.ToFeet(_pC[1]), _gs.ToFeet(_pC[2])));
            }
        }
        double scetchCellHeightY = _gs.ToFeet(3000-1298);
        double scetchCellLengthX = _gs.ToFeet(7000-2000);
        public void CreatingTableBody(View view, double[] startPoint, string indexPos)
        {
            double x0 = _p0[0];
            double y0 = _p0[1];
            double z0 = _p0[2];
            double[,,] xYZs = {
            //Horizontal lines
             { {x0, y0 - 3000, z0}, { x0 + 9000, y0 - 3000, z0 } },
             //Vertical lines
             { {  x0, y0, z0 }, { x0 , y0 - 3000, z0 } },
             { {  x0 + 2000, y0, z0 }, { x0 + 2000, y0 - 3000, z0 } },
             { {  x0 + 9000, y0, z0 }, { x0 + 9000, y0 - 3000, z0 } }            };
            DetailCurveArray detailCurveArray = _gs.CurveArrayFromCoords(_doc, view, xYZs);
            foreach (DetailLine detailLine in detailCurveArray)
            {
                detailLine.LineStyle = _doc.GetElement(new ElementId(lineStyle4BlackContId));//стиль "NPP_4_Сплошная_Черная"
            }
            XYZ posTextNotePos = new XYZ(_gs.ToFeet(x0 + 1000), _gs.ToFeet(y0 -1500), _gs.ToFeet(0));
            TextNoteOptions textNoteOptions = new TextNoteOptions();
            textNoteOptions.HorizontalAlignment = HorizontalTextAlignment.Center;
            textNoteOptions.VerticalAlignment = VerticalTextAlignment.Middle;
            textNoteOptions.TypeId = new ElementId(textTypeArial_3_08Id);            
            TextNote itemTextNote = TextNote.Create(_doc, view.Id, posTextNotePos, indexPos, textNoteOptions);
            itemTextNote.Width = 0.0298288568924218;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="viewId"></param>
        /// <param name="offset">Offset in mm</param>
        public void CreateTextNearCurve(Curve curve, ElementId viewId, Transform translation, XYZ adjacentArcCenter)
        {
            double offset = 150;
            TextNoteOptions textNoteOptions = new TextNoteOptions();
            textNoteOptions.HorizontalAlignment = HorizontalTextAlignment.Center;
            textNoteOptions.VerticalAlignment = VerticalTextAlignment.Middle;
            textNoteOptions.TypeId = CreateTransparentTextNoteType(_doc, textTypeArial_25_08Id).Id;
            string noteText = "";            
            XYZ baseTextNotePos = new XYZ(curve.Evaluate(0.5, true).X, curve.Evaluate(0.5, true).Y, 0);
            double X = curve.Evaluate(0.5, true).X, Y = curve.Evaluate(0.5, true).Y, Z = 0;
            XYZ textNotePos = new XYZ();
            //Задаем значение и расположение текста около прямых линий
            if (curve.GetType().Name == "Line")
            {
                double lengthInFoots = curve.ApproximateLength;
                double lengthInMilimeters = _gs.FromFeet(lengthInFoots);
                double roundedLenghtInMilimeters = Math.Round(lengthInMilimeters, 0);
                noteText = roundedLenghtInMilimeters.ToString();
                double startX = Math.Round(_gs.FromFeet(curve.GetEndPoint(0).X));
                double startY = Math.Round(_gs.FromFeet(curve.GetEndPoint(0).Y));
                double endX = Math.Round(_gs.FromFeet(curve.GetEndPoint(1).X));
                double endY = Math.Round(_gs.FromFeet(curve.GetEndPoint(1).Y));

                double X0 = baseTextNotePos.X + _gs.ToFeet(offset);
                double Y0 = baseTextNotePos.Y + _gs.ToFeet(offset);
                double X1 = baseTextNotePos.X - _gs.ToFeet(offset);
                double Y1 = baseTextNotePos.Y - _gs.ToFeet(offset);

                double Xc = adjacentArcCenter.X;
                double Yc = adjacentArcCenter.Y;
                if (startX == endX)
                {                    
                    X = Math.Abs(X0 - Xc) > Math.Abs(X1- Xc) ? X0 : X1;
                    textNoteOptions.Rotation = AnaliticGeometry.ToRadians(90);
                }
                else if (startY == endY)
                {
                    Y = Math.Abs(Y0 - Yc) > Math.Abs(Y1 - Yc) ? Y0 : Y1;                    
                }
                else
                {
                    double deltaX = endX - startX;
                    double deltaY = endY - startY;

                    double tangens = deltaY / deltaX;
                    
                    double angleInRadians = Math.Atan(tangens);
                    textNoteOptions.Rotation = angleInRadians;
                    //if(angleInRadians > 0 && angleInRadians < Math.PI / 2)
                    //{

                    //}
                    SortedDictionary<double,List<double>> deltas = new SortedDictionary<double, List<double>>()
                    {
                        {Math.Pow(Math.Pow(X0 - Xc,2) + Math.Pow(Y0 - Yc, 2), 0.5),new List<double>{X0, Y0 }},
                        {Math.Pow(Math.Pow(X1 - Xc,2) + Math.Pow(Y1 - Yc, 2), 0.5),new List<double>{X1, Y1 }},
                        {Math.Pow(Math.Pow(X0 - Xc,2) + Math.Pow(Y1 - Yc, 2), 0.5),new List<double>{X0, Y1 }},
                        {Math.Pow(Math.Pow(X1 - Xc,2) + Math.Pow(Y0 - Yc, 2), 0.5),new List<double>{X1, Y0 }},
                    };
                    X = deltas.Values.ToList()[2][0];
                    Y = deltas.Values.ToList()[2][1];

                    //X = X0 - _gs.ToFeet(offset) * Math.Cos(angleInRadians);
                    //Y = Y0 + _gs.ToFeet(offset) * Math.Sin(angleInRadians);
                }
                textNotePos = new XYZ(X, Y, Z);
            }
            //Задаем значение и расположение текста около дуг сопряжения
            else if (curve.GetType().Name == "Arc")
            {
                Arc arc = (Arc)curve;
                noteText = "R" + Math.Round(_gs.FromFeet(arc.Radius),0).ToString();
                X = baseTextNotePos.X;
                Y = baseTextNotePos.Y;
                textNotePos = translation.OfPoint(new XYZ(X, Y, Z));
            }
            
            TextNote itemTextNote = TextNote.Create(_doc, viewId, textNotePos, noteText, textNoteOptions);
            itemTextNote.Width = 0.0298288568924218;
        }
        public void CreateScetchFromRebarCurves(View view, List<Curve> rebarCurves,Rebar rebar)
        {
            CurveArray curveArray = new CurveArray();
            for (int i = 0; i < rebarCurves.Count; i++)
            {
                Curve prevCurve = Line.CreateBound(new XYZ(), new XYZ(1, 1, 0));                
                Curve curve = rebarCurves[i];
                Curve nextCurve = Line.CreateBound(new XYZ(), new XYZ(1,1,0));
                try
                {
                    nextCurve = rebarCurves[i + 1];
                }                
                catch (ArgumentOutOfRangeException ex)
                {
                    nextCurve = prevCurve;
                }
                try
                {
                    prevCurve = rebarCurves[i - 1];
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    prevCurve = nextCurve;  
                }

                string prevCurveTypeName = prevCurve.GetType().Name;
                string nextCurveTypeName = nextCurve.GetType().Name;
                string CurveTypeName = curve.GetType().Name;

                Transform translation = Transform.CreateTranslation(new XYZ());
                double offset = _gs.ToFeet(150);
                XYZ adjacentArcCenter = new XYZ();
                if (CurveTypeName == "Arc")
                {
                    Arc arc = (Arc)curve;
                    XYZ arcCenter = arc.Center;
                    XYZ arcCurveCenter = arc.Evaluate(0.5, true);
                    double tX = 0; double tY = 0;
                    
                    if (arcCenter.X <= arcCurveCenter.X)
                    {
                        tX = -offset;
                        if (arcCenter.Y > arcCurveCenter.Y)
                        {
                            tY =  offset;
                        }
                        else if (arcCenter.Y <= arcCurveCenter.Y)
                        {
                            tY = -offset;
                        }
                    }
                    else if(arcCenter.X >= arcCurveCenter.X)
                    {
                        tX =  offset;
                        if (arcCenter.Y > arcCurveCenter.Y)
                        {
                            tY = offset;
                        }
                        else if (arcCenter.Y <= arcCurveCenter.Y)
                        {
                            tY = -offset;
                        }
                    }
                    translation = Transform.CreateTranslation(new XYZ(tX,tY, 0));

                }             

                else if(CurveTypeName == "Line")
                {                    

                    if(prevCurveTypeName == "Arc" || nextCurveTypeName == "Arc")
                    {
                        Arc adjacentArc = (nextCurveTypeName == "Arc" ? nextCurve  : prevCurve) as Arc;
                        adjacentArcCenter = adjacentArc.Center;
                    }
                }
                curveArray.Append(curve);
                CreateTextNearCurve(curve, view.Id, translation, adjacentArcCenter);
            }
            DetailCurveArray detailCurveArray = new DetailCurveArray();
            try
            {
                detailCurveArray = _doc.Create.NewDetailCurveArray(view, curveArray);
            }
            catch(Exception ex)
            {
                string rebarFormName = rebar.get_Parameter(BuiltInParameter.REBAR_SHAPE).AsValueString();
                string rebarPosition = rebar.LookupParameter(P.nPPPosition)?.AsValueString();
                string rebarLenght = rebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LENGTH)?.AsValueString();
                string rebarDiameter = _doc.GetElement(rebar.GetTypeId()).get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsValueString();
                TaskDialog.Show("Ошибка", $"Ошибка при создании эскиза для стержня NPP_Позиция: {rebarPosition}, Форма: {rebarFormName}, Диаметр: {rebarDiameter}, Длина: {rebarLenght}");
                throw ex;
            }            
            foreach (DetailCurve detailLine in detailCurveArray)
            {
                detailLine.LineStyle = _doc.GetElement(new ElementId(lineStyle5BlackContId));//стиль "NPP_5_Сплошная_Черная"
            }
        }
        
        public void CreateScetchesFromRebars(View view, List<Rebar> rebars)
        {
            for(int i = 0; i < rebars.Count; i++)
            {
                Rebar rebar = rebars[i];
                List<Curve> rebarCurves = _rebarService.GetRebarCurves(rebar);

                XYZ bbCenterOfCurves = _gs.GetBbExtremFromCurves(rebarCurves, "Center");
                XYZ centerOfTableBody = _tableBodysCenters[i];
                List<Curve> rotatedCurves = _gs.RotateCurves(rebarCurves);
                List<Curve> movedRebarCurves = _gs.MoveCurves(rotatedCurves, bbCenterOfCurves, centerOfTableBody);
                XYZ pMax = _gs.GetBbExtremFromCurves(movedRebarCurves, "Max");
                XYZ pMin = _gs.GetBbExtremFromCurves(movedRebarCurves, "Min");
                double scatchLengthX = pMax.X - pMin.X;
                double scatchLengthY = pMax.Y - pMin.Y;
                double scaleKoefX = scetchCellLengthX / scatchLengthX;
                double scaleKoefY = scetchCellHeightY / scatchLengthY;
                //Внутри ф-ии scaledCurves в 3 аргументе идет определение мининмально приближенного к еденице значения отношения стороны ячейки к длине между экстремумами формы арматуры. 
                //Таким образом, получаем направление координат, вдоль которого наиболле приближена форма арматурного стержня к границам ячейки.
                double scaleKoef = Math.Abs(1 - scaleKoefX) < Math.Abs(1 - scaleKoefY) ? scaleKoefX : scaleKoefY;
                List<Curve> scaledRebarCurves = _gs.ScaleCurves(movedRebarCurves, centerOfTableBody, scaleKoef);
                CreateScetchFromRebarCurves(view, scaledRebarCurves, rebar);
                CreateScetchesFromRebarCouplers(rebar, view, scaledRebarCurves);
            }
        }
        public void CreateScetchesFromRebarCouplers(Rebar rebar, View view,List<Curve> rebarCurves)
        {
            Curve startCurveOfRebar = rebarCurves.First();
            Curve endCurveOfRebar = rebarCurves.Last();
            string startPointCoupler = rebar.get_Parameter(BuiltInParameter.REBAR_ELEM_ENDTREATMENT_START).AsValueString();
            string endPointCoupler = rebar.get_Parameter(BuiltInParameter.REBAR_ELEM_ENDTREATMENT_END).AsValueString();
            Dictionary<Curve, string> pointsAndCouplers = new Dictionary<Curve, string> {
                {startCurveOfRebar, startPointCoupler },
                {endCurveOfRebar, endPointCoupler }};
            foreach (KeyValuePair<Curve, string> pair in pointsAndCouplers)
            {
                Curve curve = pair.Key;
                XYZ point = new XYZ();
                if (curve == startCurveOfRebar)
                {
                    point = startCurveOfRebar.GetEndPoint(0);
                }
                else if (curve == endCurveOfRebar)
                {
                    point = endCurveOfRebar.GetEndPoint(1);
                }

                string couplerName = pair.Value;
                double X = _gs.FromFeet(point.X);
                double Y = _gs.FromFeet(point.Y);
                double Z = _gs.FromFeet(point.Z);
                List<Curve> curveList = new List<Curve>();
                if (couplerName.Contains("Муфта"))
                {
                    double[,,] xYZs = {
                            //Horizontal lines
                             { {  X - 200, Y + 150, 0 }, { X + 200, Y + 150, 0 } },
                             { {  X - 200, Y - 150, 0 }, { X + 200, Y - 150, 0 } },
                             //Vertical lines
                             { {  X - 200, Y + 150, 0 }, { X - 200, Y - 150, 0 } },
                             { {  X + 200, Y + 150, 0 }, { X + 200, Y - 150, 0 } }
                            };
                    curveList = _gs.GetCurvesFromCoords(xYZs);
                }
                else if (couplerName.Contains("Резьба"))
                {
                    double[,,] xYZs = {
                             //Vertical lines
                             { {  X - 220, Y + 125, 0 }, { X - 220, Y -125, 0 } },
                             { {  X - 110, Y + 125, 0 }, { X - 110, Y - 125, 0 } },
                             { {  X, Y + 125, 0 }, { X, Y - 125, 0 } }
                            };
                    curveList = _gs.GetCurvesFromCoords(xYZs);
                }
                else
                {
                    return;
                }
                double startX = Math.Round(_gs.FromFeet(curve.GetEndPoint(0).X));
                double startY = Math.Round(_gs.FromFeet(curve.GetEndPoint(0).Y));
                double endX = Math.Round(_gs.FromFeet(curve.GetEndPoint(1).X));
                double endY = Math.Round(_gs.FromFeet(curve.GetEndPoint(1).Y));
                double angleOfCurveRotation = 0;
                double k = 0;
                if ((endX < startX && endY > startY) | (endX < startX | endY < startY))
                {
                    k = Math.PI;
                }
                if (startX == endX)
                {
                    angleOfCurveRotation = AnaliticGeometry.ToRadians(90);
                }
                else if (startY != endY && startX != endX)
                {
                    double deltaX = endX - startX;
                    double deltaY = endY - startY;
                    double tangens = deltaY / deltaX;
                    angleOfCurveRotation = Math.Atan(tangens);
                }
                Transform rotation = Transform.CreateRotationAtPoint(new XYZ(0, 0, 1), angleOfCurveRotation + k, new XYZ(_gs.ToFeet(X), _gs.ToFeet(Y), _gs.ToFeet(Z)));
                List<Curve> rotatedCurves = curveList.Select(it => it.CreateTransformed(rotation)).ToList();

                CurveArray curveArray = new CurveArray();
                foreach (Curve curve1 in rotatedCurves)
                {
                    curveArray.Append(curve1);
                }
                DetailCurveArray detailCurveArray = _doc.Create.NewDetailCurveArray(view, curveArray);
                foreach (DetailCurve detailLine in detailCurveArray)
                {
                    detailLine.LineStyle = _doc.GetElement(new ElementId(lineStyle5BlackContId));//стиль "NPP_5_Сплошная_Черная"
                }
                
            }
        }

    }
}
