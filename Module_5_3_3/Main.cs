using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module_5_3_3
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            IList<Reference> selectdElement = uidoc.Selection.PickObjects(ObjectType.Element, new PipeFilter(), "Выберите элементы");

            using (Transaction ts = new Transaction(doc, "Set"))
            {
                ts.Start();
                foreach (var selectedElem in selectdElement)
                {
                    Element element = doc.GetElement(selectedElem);

                    double length = UnitUtils.ConvertFromInternalUnits(element.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble(), UnitTypeId.Meters)*1.1;
                    element.LookupParameter("Длина с запасом").Set(UnitUtils.ConvertToInternalUnits(length, UnitTypeId.Millimeters));
                }
                ts.Commit();
            }
            

            return Result.Succeeded;
        }
        
        public class PipeFilter : ISelectionFilter
        {
            public bool AllowElement(Element elem)
            {
                //Если элемент в фильтре является стеной тогда возращает значение Истина
                return elem is Pipe;
            }

            public bool AllowReference(Reference reference, XYZ position)
            {
                return false;
            }
        }
    }
}
