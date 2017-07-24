using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using CSSPDHI;
using CSSPDHI.Resources;

namespace CSSPDHI.Tests
{
    /// <summary>
    /// Summary description for PFSTest
    /// </summary>
    [TestClass]
    public class PFSTest
    {
        #region Variables
        string fileName = @"C:\CSSP Latest Code Old\CSSPDHIFormApp\CSSPDHIFormApp\Gaspe.m21fm";
        #endregion Variables

        #region Properties
        FileInfo fi { get; set; }
        #endregion Properties

        #region Constructors
        public PFSTest()
        {
            fi = new FileInfo(fileName);
        }
        #endregion Constructors

        #region Functions Test
        [TestMethod]
        public void PFS_Constructor_OK()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);
            }
        }
        [TestMethod]
        public void PFS_Constructor_File_DoesNotExist_From_DHIBase_Error()
        {
            FileInfo fiTemp = new FileInfo(fi.FullName + "aa");
            using (PFS pfs = new PFS(fiTemp))
            {
                Assert.AreEqual(string.Format(CSSPDHIRes.File_DoesNotExist, fiTemp.FullName), pfs.ErrorMessage);
            }
        }
        [TestMethod]
        public void PFS_GetVariable_bool_true_OK()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                bool? boolRet = pfs.GetVariable<bool>("FemEngineHD/MUD_TRANSPORT_MODULE/WATER_COLUMN/SAND_FRACTIONS/FRACTION_1", "include_sand", 1);
                Assert.IsNotNull(boolRet);
                Assert.AreEqual(true, boolRet);
                Assert.AreEqual("", pfs.ErrorMessage);
            }
        }
        [TestMethod]
        public void PFS_GetVariable_bool_false_OK()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                bool? boolRet = pfs.GetVariable<bool>("FemEngineHD/MUD_TRANSPORT_MODULE/WATER_COLUMN/DENSITY_AND_VISCOSITY", "include_density_feedback_on_HD", 1);
                Assert.IsNotNull(boolRet);
                Assert.AreEqual(false, boolRet);
                Assert.AreEqual("", pfs.ErrorMessage);
            }
        }
        [TestMethod]
        public void PFS_GetVariable_bool_WrongFileTypeLookingFor__Error()
        {
            FileInfo fi = new FileInfo(@"C:\CSSP Latest Code Old\CSSPDHIFormApp\CSSPDHIFormApp\trans_Mike21.dfsu");
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);

                bool? boolRet = pfs.GetVariable<bool>("FemEngineHD/MUD_TRANSPORT_MODULE/WATER_COLUMN/SAND_FRACTIONS/FRACTION_1", "include_sand", 1);
                string ErrorMessage = string.Format(CSSPDHIRes.WrongFileTypeLookingFor_, "[.m21fm or .m3fm]");
                Assert.AreEqual(ErrorMessage, pfs.ErrorMessage.Substring(0, ErrorMessage.Length));
            }
        }
        [TestMethod]
        public void PFS_GetVariable_bool_PotentiallyWrongFileTypeLookingFor_Error__Error()
        {
            FileInfo fi = new FileInfo(@"C:\CSSP Latest Code Old\CSSPDHIFormApp\CSSPDHIFormApp\wrongFileTypeEvenWithGoodExt.m21fm");
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);

                bool? boolRet = pfs.GetVariable<bool>("FemEngineHD/MUD_TRANSPORT_MODULE/WATER_COLUMN/SAND_FRACTIONS/FRACTION_1", "include_sand", 1);
                string ErrorMessage = string.Format(CSSPDHIRes.PotentiallyWrongFileTypeLookingFor_Error_, "[.m21fm or .m3fm]", "");
                Assert.AreEqual(ErrorMessage, pfs.ErrorMessage.Substring(0, ErrorMessage.Length - 1) + "]");
            }
        }
        [TestMethod]
        public void PFS_GetVariable_bool_File_DoesNotExist_In_GetBool_Error()
        {
            using (PFS pfs = new PFS(fi))
            {
                pfs.fi = new FileInfo(fi.FullName + "aa");
                bool? boolRet = pfs.GetVariable<bool>("FemEngineHD/MUD_TRANSPORT_MODULE/WATER_COLUMN/SAND_FRACTIONS/FRACTION_1", "include_sand", 1);
                Assert.IsNull(boolRet);
                Assert.AreEqual(string.Format(CSSPDHIRes.File_DoesNotExist, pfs.fi.FullName), pfs.ErrorMessage);
            }
        }
        [TestMethod]
        public void PFS_GetVariable_bool_CouldNotFindSectionForPath__Error()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                string path = "FemEngineHD/MUD_TRANSPORT_MODULE/WATER_COLUMN/SAND_FRACTIONS/ThisPathDoesNotExist";
                bool? boolRet = pfs.GetVariable<bool>(path, "include_sand", 1);
                Assert.IsNull(boolRet);
                Assert.AreEqual(string.Format(CSSPDHIRes.CouldNotFindSectionForPath_, path), pfs.ErrorMessage);
            }
        }
        [TestMethod]
        public void PFS_GetVariable_bool_Keyword_DoesNotExistForSectionPath_Error__Error()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                string keyword = "NotAnExistingKeyword";
                string path = "FemEngineHD/MUD_TRANSPORT_MODULE/WATER_COLUMN/SAND_FRACTIONS/FRACTION_1";
                bool? boolRet = pfs.GetVariable<bool>(path, keyword, 1);
                Assert.IsNull(boolRet);
                string ErrorMessage = string.Format(CSSPDHIRes.Keyword_DoesNotExistForSectionPath_Error_, keyword, path, "");
                Assert.AreEqual(ErrorMessage, pfs.ErrorMessage.Substring(0, ErrorMessage.Length - 1) + "]");
            }
        }
        [TestMethod]
        public void PFS_GetVariable_bool_ParameterIndex_DoesNotExistForSectionPath_AndKeyword_Error__Error()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                int ParameterIndex = 2;
                string keyword = "include_sand";
                string path = "FemEngineHD/MUD_TRANSPORT_MODULE/WATER_COLUMN/SAND_FRACTIONS/FRACTION_1";
                bool? boolRet = pfs.GetVariable<bool>(path, keyword, ParameterIndex);
                Assert.IsNull(boolRet);
                string ErrorMessage = string.Format(CSSPDHIRes.ParameterIndex_DoesNotExistForSectionPath_AndKeyword_Error_, ParameterIndex, path, keyword, "");
                Assert.AreEqual(ErrorMessage, pfs.ErrorMessage.Substring(0, ErrorMessage.Length - 1) + "]");
            }
        }
        [TestMethod]
        public void PFS_GetVariable_DateTime_OK()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                DateTime? dateTimeRet = pfs.GetVariableDateTime("FemEngineHD/TIME", "start_time");
                Assert.IsNotNull(dateTimeRet);
                Assert.AreEqual(new DateTime(2011, 1, 1, 0, 0, 0), dateTimeRet);
                Assert.AreEqual("", pfs.ErrorMessage);
            }
        }
        [TestMethod]
        public void PFS_GetVariable_DateTime_CheckAll_Error()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                DateTime? dateTimeRet = pfs.GetVariableDateTime("FemEngineHD/TIME", "start_timeNot");
                Assert.IsNull(dateTimeRet);
            }
        }
        [TestMethod]
        public void PFS_GetVariable_DateTime_Exception_Error()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                DateTime? dateTimeRet = pfs.GetVariableDateTime("FemEngineHD/TIME", "start_time");
                Assert.IsNotNull(dateTimeRet);
                Assert.AreEqual(new DateTime(2011, 1, 1, 0, 0, 0), dateTimeRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                int value = 300;
                int ParameterIndex = 6; // seconds
                int? intRet = pfs.SetVariable<int>("FemEngineHD/TIME", "start_time", ParameterIndex, value);
                Assert.IsNotNull(intRet);

                dateTimeRet = pfs.GetVariableDateTime("FemEngineHD/TIME", "start_time");
                Assert.IsNull(dateTimeRet);
                Assert.IsFalse(string.IsNullOrWhiteSpace(pfs.ErrorMessage));

                pfs.ErrorMessage = "";

                value = 0;
                ParameterIndex = 6; // seconds
                intRet = pfs.SetVariable<int>("FemEngineHD/TIME", "start_time", ParameterIndex, value);
                Assert.IsNotNull(intRet);

                dateTimeRet = pfs.GetVariableDateTime("FemEngineHD/TIME", "start_time");
                Assert.IsNotNull(dateTimeRet);
                Assert.AreEqual(new DateTime(2011, 1, 1, 0, 0, 0), dateTimeRet);
                Assert.AreEqual("", pfs.ErrorMessage);
            }
        }
        [TestMethod]
        public void PFS_GetVariable_double_OK()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                double? doubleRet = pfs.GetVariable<double>("FemEngineHD/DOMAIN", "minimum_depth", 1);
                Assert.IsNotNull(doubleRet);
                Assert.IsTrue(doubleRet.ToString().StartsWith("-0.4999"));
                Assert.AreEqual("", pfs.ErrorMessage);
            }
        }
        [TestMethod]
        public void PFS_GetVariable_FileInfo_OK()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                FileInfo fileInfoRet = pfs.GetVariableFileInfo("FemEngineHD/DOMAIN", "file_name", 1);
                Assert.IsNotNull(fileInfoRet);
                Assert.AreEqual(fi.FullName.Replace("Gaspe.m21fm", "Gaspe.mesh"), fileInfoRet.FullName);
                Assert.AreEqual("", pfs.ErrorMessage);
            }
        }
        [TestMethod]
        public void PFS_GetVariable_FileInfo_Error()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                FileInfo fileInfoRet = pfs.GetVariableFileInfo("FemEngineHD/DOMAIN", "file_nameNot", 1);
                Assert.IsNull(fileInfoRet);
                Assert.IsFalse(string.IsNullOrWhiteSpace(pfs.ErrorMessage));
            }
        }
        [TestMethod]
        public void PFS_GetVariable_FileInfo_Exception_Error()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                FileInfo fileInfoRet = pfs.GetVariableFileInfo("FemEngineHD/DOMAIN", "file_name", 1);
                Assert.IsNotNull(fileInfoRet);
                Assert.AreEqual(fi.FullName.Replace("Gaspe.m21fm", "Gaspe.mesh"), fileInfoRet.FullName);
                Assert.AreEqual("", pfs.ErrorMessage);

                int? intRet = pfs.SetVariable<int>("FemEngineHD/DOMAIN", "file_name", 1, 1);
                Assert.IsNotNull(intRet);
                Assert.AreEqual(1, intRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                FileInfo fileInfoRet2 = pfs.GetVariableFileInfo("FemEngineHD/DOMAIN", "file_name", 1);
                Assert.IsNull(fileInfoRet2);
                Assert.IsFalse(string.IsNullOrWhiteSpace(pfs.ErrorMessage));

                pfs.ErrorMessage = "";

                fileInfoRet2 = pfs.SetVariableFileInfo("FemEngineHD/DOMAIN", "file_name", 1, fileInfoRet);
                Assert.IsNotNull(fileInfoRet);
                Assert.AreEqual(fi.FullName.Replace("Gaspe.m21fm", "Gaspe.mesh"), fileInfoRet.FullName);
                Assert.AreEqual("", pfs.ErrorMessage);

                fileInfoRet = pfs.GetVariableFileInfo("FemEngineHD/DOMAIN", "file_name", 1);
                Assert.IsNotNull(fileInfoRet);
                Assert.AreEqual(fi.FullName.Replace("Gaspe.m21fm", "Gaspe.mesh"), fileInfoRet.FullName);
                Assert.AreEqual("", pfs.ErrorMessage);

            }
        }
        [TestMethod]
        public void PFS_GetVariable_float_OK()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                double? doubleRet = pfs.GetVariable<float>("FemEngineHD/DOMAIN", "minimum_depth", 1);
                Assert.IsNotNull(doubleRet);
                Assert.AreEqual(-0.4999854f, doubleRet);
                Assert.AreEqual("", pfs.ErrorMessage);
            }
        }
        [TestMethod]
        public void PFS_GetVariable_int_OK()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                int? intRet = pfs.GetVariable<int>("FemEngineHD/DOMAIN", "discretization", 1);
                Assert.IsNotNull(intRet);
                Assert.AreEqual(2, intRet);
                Assert.AreEqual("", pfs.ErrorMessage);
            }
        }
        [TestMethod]
        public void PFS_GetVariable_string_OK()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                string strRet = pfs.GetVariableString("FemEngineHD/DOMAIN", "coordinate_type", 1);
                Assert.IsNotNull(strRet);
                Assert.AreEqual("LONG/LAT", strRet);
                Assert.AreEqual("", pfs.ErrorMessage);
            }
        }
        [TestMethod]
        public void PFS_GetVariable_string_Error()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                string strRet = pfs.GetVariableString("FemEngineHD/DOMAIN", "coordinate_typeNot", 1);
                Assert.IsTrue(string.IsNullOrWhiteSpace(strRet));
                Assert.IsFalse(string.IsNullOrWhiteSpace(pfs.ErrorMessage));
            }
        }
        [TestMethod]
        public void PFS_GetVariable_string_Exception_Error()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                string strRet = pfs.GetVariableString("FemEngineHD/DOMAIN", "coordinate_type", 3);
                Assert.IsTrue(string.IsNullOrWhiteSpace(strRet));
                Assert.IsFalse(string.IsNullOrWhiteSpace(pfs.ErrorMessage));
            }
        }
        [TestMethod]
        public void PFS_GetVariable_bit_not_implemented_should_return_null_OK()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                decimal? decimalRet = pfs.GetVariable<decimal>("FemEngineHD/DOMAIN", "minimum_depth", 1);
                Assert.IsNull(decimalRet);
            }
        }
        [TestMethod]
        public void PFS_SetVariable_bool_OK()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                bool? boolRet = pfs.GetVariable<bool>("FemEngineHD/MUD_TRANSPORT_MODULE/WATER_COLUMN/SAND_FRACTIONS/FRACTION_1", "include_sand", 1);
                Assert.IsNotNull(boolRet);
                Assert.AreEqual(true, boolRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                boolRet = pfs.SetVariable<bool>("FemEngineHD/MUD_TRANSPORT_MODULE/WATER_COLUMN/SAND_FRACTIONS/FRACTION_1", "include_sand", 1, false);
                Assert.IsNotNull(boolRet);
                Assert.AreEqual(false, boolRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                boolRet = pfs.GetVariable<bool>("FemEngineHD/MUD_TRANSPORT_MODULE/WATER_COLUMN/SAND_FRACTIONS/FRACTION_1", "include_sand", 1);
                Assert.IsNotNull(boolRet);
                Assert.AreEqual(false, boolRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                boolRet = pfs.SetVariable<bool>("FemEngineHD/MUD_TRANSPORT_MODULE/WATER_COLUMN/SAND_FRACTIONS/FRACTION_1", "include_sand", 1, true);
                Assert.IsNotNull(boolRet);
                Assert.AreEqual(true, boolRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                boolRet = pfs.GetVariable<bool>("FemEngineHD/MUD_TRANSPORT_MODULE/WATER_COLUMN/SAND_FRACTIONS/FRACTION_1", "include_sand", 1);
                Assert.IsNotNull(boolRet);
                Assert.AreEqual(true, boolRet);
                Assert.AreEqual("", pfs.ErrorMessage);
            }
        }
        [TestMethod]
        public void PFS_SetVariable_bool_Error()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                bool? boolRet = pfs.SetVariable<bool>("FemEngineHD/MUD_TRANSPORT_MODULE/WATER_COLUMN/SAND_FRACTIONS/FRACTION_1", "include_sandNot", 1, false);
                Assert.IsNull(boolRet);
                Assert.IsFalse(string.IsNullOrWhiteSpace(pfs.ErrorMessage));
            }
        }
        [TestMethod]
        public void PFS_SetVariable_bool_Exception_Error()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                bool? boolRet = pfs.SetVariable<bool>("FemEngineHD/MUD_TRANSPORT_MODULE/WATER_COLUMN/SAND_FRACTIONS/FRACTION_1", "include_sand", 4, false);
                Assert.IsNull(boolRet);
                Assert.IsFalse(string.IsNullOrWhiteSpace(pfs.ErrorMessage));
            }
        }
        [TestMethod]
        public void PFS_SetVariable_DateTime_OK()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                DateTime? dateTimeRet = pfs.GetVariableDateTime("FemEngineHD/TIME", "start_time");
                Assert.IsNotNull(dateTimeRet);
                Assert.AreEqual(new DateTime(2011, 1, 1, 0, 0, 0), dateTimeRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                dateTimeRet = new DateTime(2011, 1, 1, 0, 0, 5);
                dateTimeRet = pfs.SetVariableDateTime("FemEngineHD/TIME", "start_time", (DateTime)dateTimeRet);
                Assert.IsNotNull(dateTimeRet);
                Assert.AreEqual(new DateTime(2011, 1, 1, 0, 0, 5), dateTimeRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                dateTimeRet = pfs.GetVariableDateTime("FemEngineHD/TIME", "start_time");
                Assert.IsNotNull(dateTimeRet);
                Assert.AreEqual(new DateTime(2011, 1, 1, 0, 0, 5), dateTimeRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                dateTimeRet = new DateTime(2011, 1, 1, 0, 0, 0);
                dateTimeRet = pfs.SetVariableDateTime("FemEngineHD/TIME", "start_time", (DateTime)dateTimeRet);
                Assert.IsNotNull(dateTimeRet);
                Assert.AreEqual(new DateTime(2011, 1, 1, 0, 0, 0), dateTimeRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                dateTimeRet = pfs.GetVariableDateTime("FemEngineHD/TIME", "start_time");
                Assert.IsNotNull(dateTimeRet);
                Assert.AreEqual(new DateTime(2011, 1, 1, 0, 0, 0), dateTimeRet);
                Assert.AreEqual("", pfs.ErrorMessage);
            }
        }
        [TestMethod]
        public void PFS_SetVariable_DateTime_CheckAll_Error()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                DateTime? dateTimeRet = new DateTime(2011, 1, 1, 0, 0, 5);
                dateTimeRet = pfs.SetVariableDateTime("FemEngineHD/TIME", "start_timeNot", (DateTime)dateTimeRet);
                Assert.IsNull(dateTimeRet);
                Assert.IsFalse(string.IsNullOrWhiteSpace(pfs.ErrorMessage));
            }
        }
        [TestMethod]
        public void PFS_SetVariable_DateTime_Exception_Error()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                // ToDo: should try to write test code
            }
        }
        [TestMethod]
        public void PFS_SetVariable_double_OK()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                double? doubleRet = pfs.GetVariable<double>("FemEngineHD/DOMAIN", "minimum_depth", 1);
                Assert.IsNotNull(doubleRet);
                Assert.IsTrue(doubleRet.ToString().StartsWith("-0.4999"));
                Assert.AreEqual("", pfs.ErrorMessage);

                doubleRet = pfs.SetVariable<double>("FemEngineHD/DOMAIN", "minimum_depth", 1, 0.777D);
                Assert.IsNotNull(doubleRet);
                Assert.IsTrue(doubleRet.ToString().StartsWith("0.777"));
                Assert.AreEqual("", pfs.ErrorMessage);

                doubleRet = pfs.GetVariable<double>("FemEngineHD/DOMAIN", "minimum_depth", 1);
                Assert.IsNotNull(doubleRet);
                Assert.AreEqual(0.777D, doubleRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                doubleRet = pfs.SetVariable<double>("FemEngineHD/DOMAIN", "minimum_depth", 1, -0.4999854D);
                Assert.IsNotNull(doubleRet);
                Assert.AreEqual(-0.4999854D, doubleRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                doubleRet = pfs.GetVariable<double>("FemEngineHD/DOMAIN", "minimum_depth", 1);
                Assert.IsNotNull(doubleRet);
                Assert.AreEqual(-0.4999854D, doubleRet);
                Assert.AreEqual("", pfs.ErrorMessage);
            }
        }
        [TestMethod]
        public void PFS_SetVariable_FileInfo_OK()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                FileInfo fileInfoRet = pfs.GetVariableFileInfo("FemEngineHD/DOMAIN", "file_name", 1);
                Assert.IsNotNull(fileInfoRet);
                Assert.AreEqual(fi.FullName.Replace("Gaspe.m21fm", "Gaspe.mesh"), fileInfoRet.FullName);
                Assert.AreEqual("", pfs.ErrorMessage);

                fileInfoRet = new FileInfo(fileInfoRet.FullName.Replace("Gaspe.mesh", "aaa.mesh"));
                fileInfoRet = pfs.SetVariableFileInfo("FemEngineHD/DOMAIN", "file_name", 1, fileInfoRet);
                Assert.IsNotNull(fileInfoRet);
                Assert.AreEqual(fi.FullName.Replace("Gaspe.m21fm", "aaa.mesh"), fileInfoRet.FullName);
                Assert.AreEqual("", pfs.ErrorMessage);

                fileInfoRet = pfs.GetVariableFileInfo("FemEngineHD/DOMAIN", "file_name", 1);
                Assert.IsNotNull(fileInfoRet);
                Assert.AreEqual(fi.FullName.Replace("Gaspe.m21fm", "aaa.mesh"), fileInfoRet.FullName);
                Assert.AreEqual("", pfs.ErrorMessage);

                fileInfoRet = new FileInfo(fileInfoRet.FullName.Replace("aaa.mesh", "Gaspe.mesh"));
                fileInfoRet = pfs.SetVariableFileInfo("FemEngineHD/DOMAIN", "file_name", 1, fileInfoRet);
                Assert.IsNotNull(fileInfoRet);
                Assert.AreEqual(fi.FullName.Replace("Gaspe.m21fm", "Gaspe.mesh"), fileInfoRet.FullName);
                Assert.AreEqual("", pfs.ErrorMessage);

                fileInfoRet = pfs.GetVariableFileInfo("FemEngineHD/DOMAIN", "file_name", 1);
                Assert.IsNotNull(fileInfoRet);
                Assert.AreEqual(fi.FullName.Replace("Gaspe.m21fm", "Gaspe.mesh"), fileInfoRet.FullName);
                Assert.AreEqual("", pfs.ErrorMessage);

            }
        }
        [TestMethod]
        public void PFS_SetVariable_FileInfo_CheckAll_Error()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                FileInfo fileInfoRet = pfs.SetVariableFileInfo("FemEngineHD/DOMAIN", "file_nameNot", 1, new FileInfo("allo"));
                Assert.IsNull(fileInfoRet);
                Assert.IsFalse(string.IsNullOrWhiteSpace(pfs.ErrorMessage));
            }
        }
        [TestMethod]
        public void PFS_SetVariable_FileInfo_Exception_Error()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                FileInfo fileInfoRet = pfs.SetVariableFileInfo("FemEngineHD/DOMAIN", "file_name", 4, new FileInfo("allo"));
                Assert.IsNull(fileInfoRet);
                Assert.IsFalse(string.IsNullOrWhiteSpace(pfs.ErrorMessage));

            }
        }
        [TestMethod]
        public void PFS_SetVariable_float_OK()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                float? floatRet = pfs.GetVariable<float>("FemEngineHD/DOMAIN", "minimum_depth", 1);
                Assert.IsNotNull(floatRet);
                Assert.IsTrue(floatRet.ToString().StartsWith("-0.4999"));
                Assert.AreEqual("", pfs.ErrorMessage);

                floatRet = pfs.SetVariable<float>("FemEngineHD/DOMAIN", "minimum_depth", 1, 0.777f);
                Assert.IsNotNull(floatRet);
                Assert.AreEqual(0.777f, floatRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                floatRet = pfs.GetVariable<float>("FemEngineHD/DOMAIN", "minimum_depth", 1);
                Assert.IsNotNull(floatRet);
                Assert.AreEqual(0.777f, floatRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                floatRet = pfs.SetVariable<float>("FemEngineHD/DOMAIN", "minimum_depth", 1, -0.4999854f);
                Assert.IsNotNull(floatRet);
                Assert.AreEqual(-0.4999854f, floatRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                floatRet = pfs.GetVariable<float>("FemEngineHD/DOMAIN", "minimum_depth", 1);
                Assert.IsNotNull(floatRet);
                Assert.AreEqual(-0.4999854f, floatRet);
                Assert.AreEqual("", pfs.ErrorMessage);
            }
        }
        [TestMethod]
        public void PFS_SetVariable_int_OK()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                int? doubleRet = pfs.GetVariable<int>("FemEngineHD/DOMAIN", "discretization", 1);
                Assert.IsNotNull(doubleRet);
                Assert.AreEqual(2, doubleRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                doubleRet = pfs.SetVariable<int>("FemEngineHD/DOMAIN", "discretization", 1, 3);
                Assert.IsNotNull(doubleRet);
                Assert.AreEqual(3, doubleRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                doubleRet = pfs.GetVariable<int>("FemEngineHD/DOMAIN", "discretization", 1);
                Assert.IsNotNull(doubleRet);
                Assert.AreEqual(3, doubleRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                doubleRet = pfs.SetVariable<int>("FemEngineHD/DOMAIN", "discretization", 1, 2);
                Assert.IsNotNull(doubleRet);
                Assert.AreEqual(2, doubleRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                doubleRet = pfs.GetVariable<int>("FemEngineHD/DOMAIN", "discretization", 1);
                Assert.IsNotNull(doubleRet);
                Assert.AreEqual(2, doubleRet);
                Assert.AreEqual("", pfs.ErrorMessage);
            }
        }
        [TestMethod]
        public void PFS_SetVariable_string_OK()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                string strRet = pfs.GetVariableString("FemEngineHD/DOMAIN", "coordinate_type", 1);
                Assert.IsNotNull(strRet);
                Assert.AreEqual("LONG/LAT", strRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                strRet = "LONG/LATAAA";
                strRet = pfs.SetVariableString("FemEngineHD/DOMAIN", "coordinate_type", 1, strRet);
                Assert.IsNotNull(strRet);
                Assert.AreEqual("LONG/LATAAA", strRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                strRet = pfs.GetVariableString("FemEngineHD/DOMAIN", "coordinate_type", 1);
                Assert.IsNotNull(strRet);
                Assert.AreEqual("LONG/LATAAA", strRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                strRet = "LONG/LAT";
                strRet = pfs.SetVariableString("FemEngineHD/DOMAIN", "coordinate_type", 1, strRet);
                Assert.IsNotNull(strRet);
                Assert.AreEqual("LONG/LAT", strRet);
                Assert.AreEqual("", pfs.ErrorMessage);

                strRet = pfs.GetVariableString("FemEngineHD/DOMAIN", "coordinate_type", 1);
                Assert.IsNotNull(strRet);
                Assert.AreEqual("LONG/LAT", strRet);
                Assert.AreEqual("", pfs.ErrorMessage);

            }
        }
        [TestMethod]
        public void PFS_SetVariable_string_CheckAll_Error()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                string strRet = "LONG/LATAAA";
                strRet = pfs.SetVariableString("FemEngineHD/DOMAIN", "coordinate_typenot", 1, strRet);
                Assert.IsTrue(string.IsNullOrWhiteSpace(strRet));
                Assert.IsFalse(string.IsNullOrWhiteSpace(pfs.ErrorMessage));
            }
        }
        [TestMethod]
        public void PFS_SetVariable_string_Exception_Error()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                string strRet = "LONG/LATAAA";
                strRet = pfs.SetVariableString("FemEngineHD/DOMAIN", "coordinate_type", 4, strRet);
                Assert.IsTrue(string.IsNullOrWhiteSpace(strRet));
                Assert.IsFalse(string.IsNullOrWhiteSpace(pfs.ErrorMessage));
            }
        }
        [TestMethod]
        public void PFS_SetVariable_bit_not_implemented_should_return_null_OK()
        {
            using (PFS pfs = new PFS(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);

                decimal? decimalRet = pfs.SetVariable<decimal>("FemEngineHD/DOMAIN", "minimum_depth", 1, 123.456m);
                Assert.IsNull(decimalRet);
            }
        }

        #endregion Functions Test

        #region Functions private
        #endregion Functions private
    }

}
