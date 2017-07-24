using CSSPDHI.Resources;
using CSSPModelsDLL.Models;
using DHI.Generic.MikeZero;
using DHI.Generic.MikeZero.DFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSPDHI
{
    public class Tide : DHIBase
    {
        #region Variables
        #endregion Variables

        #region Properties
        #endregion Properties

        #region Constructors
        public Tide(FileInfo fi) : base(fi)
        {
        }
        #endregion Constructors

        #region Overrides
        #endregion Overrides

        #region Events
        #endregion Events

        #region Functions public
        public bool GenerateWebTideNode(TVFileModel TVFileModelBC, int WebTideNodeNumb, List<Coord> CoordList, List<TVFileModel> tvFileModelList, int BoundaryConditionCodeNumber, List<List<WaterLevelResult>> AllWLResults, List<IEnumerable<CurrentResult>> AllCurrentResults)
        {
            List<eumItem> eumItemList = new List<eumItem>();

            DfsFactory factory = new DfsFactory();

            IDfsFile dfsOldFile = DfsFileFactory.DfsGenericOpen(TVFileModelBC.ServerFilePath + TVFileModelBC.ServerFileName);

            DfsBuilder dfsNewFile = DfsBuilder.Create(dfsOldFile.FileInfo.FileTitle, dfsOldFile.FileInfo.ApplicationTitle, dfsOldFile.FileInfo.ApplicationVersion);

            double WebTideStepsInMinutes = ((double)((IDfsEqCalendarAxis)((dfsOldFile.FileInfo).TimeAxis)).TimeStep / 60);

            DateTime? dateTimeTemp = null;
            int? NumberOfTimeSteps = null;
            int? TimeStepInterval = null;
            using (PFS pfs = new PFS(base.fi))
            {
                dateTimeTemp = pfs.GetVariableDateTime("FemEngineHD/TIME", "start_time");
                if (dateTimeTemp == null)
                {
                    dfsOldFile.Close();
                    return false;
                }

                NumberOfTimeSteps = pfs.GetVariable<int>("FemEngineHD/TIME", "number_of_time_steps", 1);
                if (NumberOfTimeSteps == null)
                {
                    dfsOldFile.Close();
                    return false;
                }

                TimeStepInterval = pfs.GetVariable<int>("FemEngineHD/TIME", "time_step_interval", 1);
                if (TimeStepInterval == null)
                {
                    dfsOldFile.Close();
                    return false;
                }
            }

            DateTime StartDate = ((DateTime)dateTimeTemp).AddHours(-1);
            DateTime EndDate = ((DateTime)dateTimeTemp).AddSeconds((int)NumberOfTimeSteps * (int)TimeStepInterval).AddHours(1);

            dfsNewFile.SetDataType(dfsOldFile.FileInfo.DataType);
            dfsNewFile.SetGeographicalProjection(dfsOldFile.FileInfo.Projection);
            dfsNewFile.SetTemporalAxis(factory.CreateTemporalEqCalendarAxis(eumUnit.eumUsec, StartDate, 0, WebTideStepsInMinutes * 60));
            dfsNewFile.SetItemStatisticsType(StatType.RegularStat);

            foreach (IDfsDynamicItemInfo di in dfsOldFile.ItemInfo)
            {
                DfsDynamicItemBuilder ddib = dfsNewFile.CreateDynamicItemBuilder();
                ddib.Set(di.Name, eumQuantity.Create(di.Quantity.Item, di.Quantity.Unit), di.DataType);
                ddib.SetValueType(di.ValueType);
                ddib.SetAxis(factory.CreateAxisEqD1(eumUnit.eumUsec, CoordList.Count, 0, 1));
                ddib.SetReferenceCoordinates(di.ReferenceCoordinateX, di.ReferenceCoordinateY, di.ReferenceCoordinateZ);
                dfsNewFile.AddDynamicItem(ddib.GetDynamicItemInfo());
                eumItemList.Add(di.Quantity.Item);
            }

            dfsOldFile.Close();

            string[] NewFileErrors = dfsNewFile.Validate();
            StringBuilder sbErr = new StringBuilder();
            foreach (string s in NewFileErrors)
            {
                sbErr.AppendLine(s);
            }

            if (NewFileErrors.Count() > 0)
            {
                ErrorMessage = string.Format(CSSPDHIRes.CouldNotCreate_, TVFileModelBC.ServerFileName.Replace(".dfs0", "dfs1"));
                OnCSSPDHIChanged(new CSSPDHIEventArgs(new CSSPDHIMessage("Error", -1, false, ErrorMessage)));
                return false;
            }

            string NewFileNameBC = TVFileModelBC.ServerFileName;

            if (CoordList.Count == 0)
            {
                ErrorMessage = CSSPDHIRes.NumberOfWebTideNodesIsZero;
                OnCSSPDHIChanged(new CSSPDHIEventArgs(new CSSPDHIMessage("Error", -1, false, ErrorMessage)));
                return false;
            }

            if (eumItemList.Count == 1)
            {
                if (eumItemList[0] == eumItem.eumIWaterLevel || eumItemList[0] == eumItem.eumIWaterDepth)
                {
                    List<WaterLevelResult> WLResults = null;

                    dfsNewFile.CreateFile(TVFileModelBC.ServerFilePath + NewFileNameBC);
                    IDfsFile file = dfsNewFile.GetFile();
                    for (int i = 0; i < WLResults.ToList().Count; i++)
                    {
                        float[] floatArray = new float[AllWLResults.Count];

                        for (int j = 0; j < AllWLResults.Count; j++)
                        {
                            floatArray[j] = ((float)((List<WaterLevelResult>)AllWLResults[j].ToList())[i].WaterLevel);
                        }



                        file.WriteItemTimeStepNext(0, floatArray);  // water level array
                    }
                    file.Close();
                }
                else
                {
                    ErrorMessage = string.Format(CSSPDHIRes.FileContainsOneParamButItsNotOfTypeWLOrWDItIs_, eumItemList[0].ToString());
                    OnCSSPDHIChanged(new CSSPDHIEventArgs(new CSSPDHIMessage("Error", -1, false, ErrorMessage)));
                    return false;
                }
            }
            else if (eumItemList.Count == 2)
            {
                if (eumItemList[0] == eumItem.eumIuVelocity && eumItemList[1] == eumItem.eumIvVelocity)
                {
                    // read web tide for the required time
                    List<CurrentResult> CurrentResults = null;

                    dfsNewFile.CreateFile(TVFileModelBC.ServerFilePath + NewFileNameBC);
                    IDfsFile file = dfsNewFile.GetFile();
                    for (int i = 0; i < CurrentResults.ToList().Count; i++)
                    {
                        float[] floatArrayX = new float[AllCurrentResults.Count];
                        float[] floatArrayY = new float[AllCurrentResults.Count];

                        for (int j = 0; j < AllCurrentResults.Count; j++)
                        {
                            floatArrayX[j] = ((float)((List<CurrentResult>)AllCurrentResults[j].ToList())[i].x_velocity);
                            floatArrayY[j] = ((float)((List<CurrentResult>)AllCurrentResults[j].ToList())[i].y_velocity);
                        }

                        file.WriteItemTimeStepNext(0, floatArrayX);  // Current xVelocity
                        file.WriteItemTimeStepNext(0, floatArrayY);  // Current yVelocity
                    }
                    file.Close();
                }
                else
                {
                    ErrorMessage = string.Format(CSSPDHIRes.FileContains2ParamButItsNotOfUVAndVVItIs_And_, eumItemList[0].ToString(), eumItemList[1].ToString());
                    OnCSSPDHIChanged(new CSSPDHIEventArgs(new CSSPDHIMessage("Error", -1, false, ErrorMessage)));
                    return false;
                }
            }
            else
            {
                // this is not a file that is used for Water Level or Currents
            }

            return false;
        }
        public bool SetupWebTide()
        {
            return false;
        }
        public void ReadFile()
        {
            OnCSSPDHIChanged(new CSSPDHIEventArgs(new CSSPDHIMessage("Started", 2.2f, false, "")));
        }
        #endregion Functions public

        #region Functions protected
        #endregion Functions protected

        #region Functions private
        #endregion Functions private

    }
}
