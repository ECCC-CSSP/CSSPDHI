using CSSPDHI.Resources;
using CSSPModelsDLL.Models;
using DHI.Generic.MikeZero.DFS.dfsu;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSSPDHI
{
    public class DFSU : DHIBase
    {
        #region Variables
        #endregion Variables

        #region Properties
        #endregion Properties

        #region Constructors
        public DFSU(FileInfo fi) : base(fi)
        {
        }
        #endregion Constructors

        #region Overrides
        #endregion Overrides

        #region Events
        #endregion Events

        #region Functions public
        public bool GetStudyAreaContourPolygonList(List<ContourPolygon> contourPolygonList)
        {
            //contourPolygonList = new List<ContourPolygon>();

            using (DFSU dfsu = new DFSU(fi))
            {
                List<List<Node>> StudyAreaList = new List<List<Node>>();

                DfsuFile dfsuFile;
                List<int> PolygonIndexes = new List<int>();

                try
                {
                    dfsuFile = DfsuFile.Open(fi.FullName);
                }
                catch (Exception ex)
                {
                    ErrorMessage = string.Format(CSSPDHIRes.CouldNotOpen_Error_, fi.FullName, ex.Message + (ex.InnerException != null ? "Inner: " + ex.InnerException.Message : ""));
                    OnCSSPDHIChanged(new CSSPDHIEventArgs(new CSSPDHIMessage("Error", -1, false, ErrorMessage)));
                    contourPolygonList.Add(new ContourPolygon() { Error = ErrorMessage });
                    return false;
                }

                int TotalNumberWithCodeBigger0 = dfsuFile.Code.Where(c => c > 0).Count();

                List<Element> ElementList = new List<Element>();
                List<Node> NodeList = new List<Node>();

                // ----------------------------------------
                // filling the ElementList and The NodeList
                // ----------------------------------------

                FillElementListAndNodeList(ElementList, NodeList);

                List<Node> AllNodeList = new List<Node>();
                //List<ContourPolygon> ContourPolygonList = new List<ContourPolygon>();

                List<Node> AboveNodeList = (from n in NodeList
                                            where n.Code != 0
                                            select n).ToList<Node>();

                foreach (Node sn in AboveNodeList)
                {
                    List<Node> EndNodeList = (from n in sn.ConnectNodeList
                                              where n.Code != 0
                                              select n).ToList<Node>();

                    foreach (Node en in EndNodeList)
                    {
                        AllNodeList.Add(en);
                    }

                    if (sn.Code != 0)
                    {
                        AllNodeList.Add(sn);
                    }

                }

                List<Element> UniqueElementList = new List<Element>();

                List<Element> TempUniqueElementList = new List<Element>();

                // filling UniqueElementList
                TempUniqueElementList = (from el in ElementList
                                         where (el.Type == 21 || el.Type == 32)
                                         select el).Distinct().ToList<Element>();

                foreach (Element el in TempUniqueElementList)
                {
                    if ((el.NodeList[0].Code != 0 && el.NodeList[1].Code != 0)
                        || (el.NodeList[0].Code != 0 && el.NodeList[2].Code != 0)
                        || (el.NodeList[1].Code != 0 && el.NodeList[2].Code != 0))
                    {
                        UniqueElementList.Add(el);
                    }
                }

                TempUniqueElementList = (from el in ElementList
                                         where (el.Type == 25 || el.Type == 33)
                                         select el).Distinct().ToList<Element>();

                foreach (Element el in TempUniqueElementList)
                {
                    if ((el.NodeList[0].Code != 0 && el.NodeList[1].Code != 0)
                        || (el.NodeList[1].Code != 0 && el.NodeList[2].Code != 0)
                        || (el.NodeList[2].Code != 0 && el.NodeList[3].Code != 0)
                        || (el.NodeList[3].Code != 0 && el.NodeList[0].Code != 0))
                    {
                        UniqueElementList.Add(el);
                    }
                }

                List<Node> UniqueNodeList = (from n in AllNodeList orderby n.ID select n).Distinct().ToList<Node>();


                Dictionary<String, Vector> ForwardVector = new Dictionary<String, Vector>();
                Dictionary<String, Vector> BackwardVector = new Dictionary<String, Vector>();

                //_TaskRunnerBaseService.SendPercentToDB(_TaskRunnerBaseService._BWObj.appTaskModel.AppTaskID, 30);

                foreach (Element el in UniqueElementList)
                {
                    Node Node0 = new Node();
                    Node Node1 = new Node();
                    Node Node2 = new Node();
                    Node Node3 = new Node();


                    if (el.Type == 21 || el.Type == 32)
                    {
                        Node0 = el.NodeList[0];
                        Node1 = el.NodeList[1];
                        Node2 = el.NodeList[2];

                        int ElemCount01 = (from el1 in UniqueElementList
                                           from el2 in Node0.ElementList
                                           from el3 in Node1.ElementList
                                           where el1 == el2 && el1 == el3
                                           select el1).Count();

                        int ElemCount02 = (from el1 in UniqueElementList
                                           from el2 in Node0.ElementList
                                           from el3 in Node2.ElementList
                                           where el1 == el2 && el1 == el3
                                           select el1).Count();

                        int ElemCount12 = (from el1 in UniqueElementList
                                           from el2 in Node1.ElementList
                                           from el3 in Node2.ElementList
                                           where el1 == el2 && el1 == el3
                                           select el1).Count();


                        if (Node0.Code != 0 && Node1.Code != 0)
                        {
                            if (ElemCount01 == 1)
                            {
                                ForwardVector.Add(Node0.ID.ToString() + "," + Node1.ID.ToString(), new Vector() { StartNode = Node0, EndNode = Node1 });
                                BackwardVector.Add(Node1.ID.ToString() + "," + Node0.ID.ToString(), new Vector() { StartNode = Node1, EndNode = Node0 });
                            }
                        }
                        if (Node0.Code != 0 && Node2.Code != 0)
                        {
                            if (ElemCount02 == 1)
                            {
                                ForwardVector.Add(Node0.ID.ToString() + "," + Node2.ID.ToString(), new Vector() { StartNode = Node0, EndNode = Node2 });
                                BackwardVector.Add(Node2.ID.ToString() + "," + Node0.ID.ToString(), new Vector() { StartNode = Node2, EndNode = Node0 });
                            }
                        }
                        if (Node1.Code != 0 && Node2.Code != 0)
                        {
                            if (ElemCount12 == 1)
                            {
                                ForwardVector.Add(Node1.ID.ToString() + "," + Node2.ID.ToString(), new Vector() { StartNode = Node1, EndNode = Node2 });
                                BackwardVector.Add(Node2.ID.ToString() + "," + Node1.ID.ToString(), new Vector() { StartNode = Node2, EndNode = Node1 });
                            }
                        }
                    }
                    else if (el.Type == 25 || el.Type == 33)
                    {
                        Node0 = el.NodeList[0];
                        Node1 = el.NodeList[1];
                        Node2 = el.NodeList[2];
                        Node3 = el.NodeList[3];

                        int ElemCount01 = (from el1 in UniqueElementList
                                           from el2 in Node0.ElementList
                                           from el3 in Node1.ElementList
                                           where el1 == el2 && el1 == el3
                                           select el1).Count();

                        int ElemCount03 = (from el1 in UniqueElementList
                                           from el2 in Node0.ElementList
                                           from el3 in Node3.ElementList
                                           where el1 == el2 && el1 == el3
                                           select el1).Count();

                        int ElemCount12 = (from el1 in UniqueElementList
                                           from el2 in Node1.ElementList
                                           from el3 in Node2.ElementList
                                           where el1 == el2 && el1 == el3
                                           select el1).Count();

                        int ElemCount23 = (from el1 in UniqueElementList
                                           from el2 in Node2.ElementList
                                           from el3 in Node3.ElementList
                                           where el1 == el2 && el1 == el3
                                           select el1).Count();


                        if (Node0.Code != 0 && Node1.Code != 0)
                        {
                            if (ElemCount01 == 1)
                            {
                                ForwardVector.Add(Node0.ID.ToString() + "," + Node1.ID.ToString(), new Vector() { StartNode = Node0, EndNode = Node1 });
                                BackwardVector.Add(Node1.ID.ToString() + "," + Node0.ID.ToString(), new Vector() { StartNode = Node1, EndNode = Node0 });
                            }
                        }
                        if (Node0.Code != 0 && Node3.Code != 0)
                        {
                            if (ElemCount03 == 1)
                            {
                                ForwardVector.Add(Node0.ID.ToString() + "," + Node3.ID.ToString(), new Vector() { StartNode = Node0, EndNode = Node3 });
                                BackwardVector.Add(Node3.ID.ToString() + "," + Node0.ID.ToString(), new Vector() { StartNode = Node3, EndNode = Node0 });
                            }
                        }
                        if (Node1.Code != 0 && Node2.Code != 0)
                        {
                            if (ElemCount12 == 1)
                            {
                                ForwardVector.Add(Node1.ID.ToString() + "," + Node2.ID.ToString(), new Vector() { StartNode = Node1, EndNode = Node2 });
                                BackwardVector.Add(Node2.ID.ToString() + "," + Node1.ID.ToString(), new Vector() { StartNode = Node2, EndNode = Node1 });
                            }
                        }
                        if (Node2.Code != 0 && Node3.Code != 0)
                        {
                            if (ElemCount23 == 1)
                            {
                                ForwardVector.Add(Node2.ID.ToString() + "," + Node3.ID.ToString(), new Vector() { StartNode = Node2, EndNode = Node3 });
                                BackwardVector.Add(Node3.ID.ToString() + "," + Node2.ID.ToString(), new Vector() { StartNode = Node3, EndNode = Node2 });
                            }
                        }
                    }
                    else
                    {
                        ErrorMessage = string.Format(CSSPDHIRes.ElementTypeIsOtherThan_Its_, "21, 25, 32, 33", el.Type.ToString());
                        OnCSSPDHIChanged(new CSSPDHIEventArgs(new CSSPDHIMessage("Error", -1, false, ErrorMessage)));
                        contourPolygonList.Add(new ContourPolygon() { Error = ErrorMessage });
                        return false;
                    }

                }

                //_TaskRunnerBaseService.SendPercentToDB(_TaskRunnerBaseService._BWObj.appTaskModel.AppTaskID, 40);

                bool MoreStudyAreaLine = true;
                int PolygonCount = 0;
                //MapInfoService mapInfoService = new MapInfoService(_TaskRunnerBaseService._BWObj.appTaskModel.Language, _TaskRunnerBaseService._User);
                while (MoreStudyAreaLine)
                {
                    PolygonCount += 1;

                    List<Node> FinalContourNodeList = new List<Node>();
                    Vector LastVector = new Vector();
                    LastVector = ForwardVector.First().Value;
                    FinalContourNodeList.Add(LastVector.StartNode);
                    FinalContourNodeList.Add(LastVector.EndNode);
                    ForwardVector.Remove(LastVector.StartNode.ID.ToString() + "," + LastVector.EndNode.ID.ToString());
                    BackwardVector.Remove(LastVector.EndNode.ID.ToString() + "," + LastVector.StartNode.ID.ToString());
                    bool PolygonCompleted = false;
                    while (!PolygonCompleted)
                    {
                        List<string> KeyStrList = (from k in ForwardVector.Keys
                                                   where k.StartsWith(LastVector.EndNode.ID.ToString() + ",")
                                                   && !k.EndsWith("," + LastVector.StartNode.ID.ToString())
                                                   select k).ToList<string>();

                        if (KeyStrList.Count != 1)
                        {
                            KeyStrList = (from k in BackwardVector.Keys
                                          where k.StartsWith(LastVector.EndNode.ID.ToString() + ",")
                                          && !k.EndsWith("," + LastVector.StartNode.ID.ToString())
                                          select k).ToList<string>();

                            if (KeyStrList.Count != 1)
                            {
                                PolygonCompleted = true;
                                break;
                            }
                            else
                            {
                                LastVector = BackwardVector[KeyStrList[0]];
                                BackwardVector.Remove(LastVector.StartNode.ID.ToString() + "," + LastVector.EndNode.ID.ToString());
                                ForwardVector.Remove(LastVector.EndNode.ID.ToString() + "," + LastVector.StartNode.ID.ToString());
                            }
                        }
                        else
                        {
                            LastVector = ForwardVector[KeyStrList[0]];
                            ForwardVector.Remove(LastVector.StartNode.ID.ToString() + "," + LastVector.EndNode.ID.ToString());
                            BackwardVector.Remove(LastVector.EndNode.ID.ToString() + "," + LastVector.StartNode.ID.ToString());
                        }
                        FinalContourNodeList.Add(LastVector.EndNode);
                        if (FinalContourNodeList[FinalContourNodeList.Count - 1] == FinalContourNodeList[0])
                        {
                            PolygonCompleted = true;
                        }
                    }

                    double Area = CalculateAreaOfPolygon(FinalContourNodeList);
                    if (Area < 0)
                    {
                        FinalContourNodeList.Reverse();
                        Area = CalculateAreaOfPolygon(FinalContourNodeList);
                    }

                    FinalContourNodeList.Add(FinalContourNodeList[0]);

                    ContourPolygon contourLine = new ContourPolygon() { };
                    contourLine.ContourNodeList = FinalContourNodeList;
                    contourLine.ContourValue = 0;
                    contourPolygonList.Add(contourLine);

                    if (ForwardVector.Count == 0)
                    {
                        MoreStudyAreaLine = false;
                    }

                }
            }

            return true;
        }
        #endregion Functions public

        #region Functions protected
        #endregion Functions protected

        #region Functions private
        public double CalculateDistance(double lat1, double long1, double lat2, double long2, double EarthRadius)
        {
            return Math.Abs(EarthRadius * Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(long2 - long1)));
        }
        public double CalculateAreaOfPolygon(List<Node> NodeList)
        {
            double TotalArea = 0;

            double MinLat = (double)NodeList.Min(e => e.Y);
            double MinLong = (double)NodeList.Min(e => e.X);
            double DistOneLat = CalculateDistance(MinLat * d2r, MinLong * d2r, (MinLat + 1) * d2r, MinLong * d2r, R);
            double DistOneLong = CalculateDistance(MinLat * d2r, MinLong * d2r, MinLat * d2r, (MinLong + 1) * d2r, R);

            double SumPositive = 0;
            double SumNegative = 0;
            for (int i = 0; i < NodeList.Count; i++)
            {
                if (i == NodeList.Count - 1)
                {
                    SumPositive += ((double)NodeList[i].X * DistOneLong) * ((double)NodeList[0].Y * DistOneLat);
                    SumNegative += ((double)NodeList[i].Y * DistOneLat) * ((double)NodeList[0].X * DistOneLong);
                }
                else
                {
                    SumPositive += ((double)NodeList[i].X * DistOneLong) * ((double)NodeList[i + 1].Y * DistOneLat);
                    SumNegative += ((double)NodeList[i].Y * DistOneLat) * ((double)NodeList[i + 1].X * DistOneLong);
                }

                TotalArea = (SumPositive - SumNegative) / 2;

            }

            return TotalArea;
        }
        public void FillElementListAndNodeList(List<Element> ElementList, List<Node> NodeList)
        {
            DfsuFile dfsuFile = DfsuFile.Open(fi.FullName);

            try
            {
                for (int i = 0; i < dfsuFile.NumberOfNodes; i++)
                {
                    Node n = new Node()
                    {
                        Code = dfsuFile.Code[i],
                        ID = dfsuFile.NodeIds[i],
                        X = (float)dfsuFile.X[i],
                        Y = (float)dfsuFile.Y[i],
                        Z = dfsuFile.Z[i],
                        Value = 0,
                        ConnectNodeList = new List<Node>(),
                        ElementList = new List<Element>()
                    };
                    NodeList.Add(n);
                }

                for (int i = 0; i < dfsuFile.NumberOfElements; i++)
                {
                    Element el = new Element()
                    {
                        ID = dfsuFile.ElementIds[i],
                        Type = dfsuFile.ElementType[i],
                        Value = 0,
                        NodeList = new List<Node>(),
                        NumbOfNodes = 0
                    };
                    ElementList.Add(el);
                }

                for (int i = 0; i < dfsuFile.NumberOfElements; i++)
                {
                    int CountNode = 0;
                    for (int j = 0; j < dfsuFile.ElementTable[i].Count(); j++)
                    {
                        CountNode += 1;
                        ElementList[i].NodeList.Add(NodeList[dfsuFile.ElementTable[i][j] - 1]);
                        if (!NodeList[dfsuFile.ElementTable[i][j] - 1].ElementList.Contains(ElementList[i]))
                        {
                            NodeList[dfsuFile.ElementTable[i][j] - 1].ElementList.Add(ElementList[i]);
                        }
                        for (int k = 0; k < dfsuFile.ElementTable[i].Count(); k++)
                        {
                            if (k != j)
                            {
                                if (!NodeList[dfsuFile.ElementTable[i][j] - 1].ConnectNodeList.Contains(NodeList[dfsuFile.ElementTable[i][k] - 1]))
                                {
                                    NodeList[dfsuFile.ElementTable[i][j] - 1].ConnectNodeList.Add(NodeList[dfsuFile.ElementTable[i][k] - 1]);
                                }
                            }
                        }
                    }
                    ElementList[i].NumbOfNodes = CountNode;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                dfsuFile.Close();
            }
        }

        #endregion Functions private

    }
}
