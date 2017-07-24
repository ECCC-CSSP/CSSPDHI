using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using CSSPDHI;
using CSSPDHI.Resources;
using CSSPModelsDLL.Models;
using DHI.Generic.MikeZero.DFS.dfsu;

namespace CSSPDHI.Tests
{
    /// <summary>
    /// Summary description for PFSTest
    /// </summary>
    [TestClass]
    public class DFSUTest
    {
        #region Variables
        string fileName = @"C:\CSSP Latest Code Old\CSSPDHIFormApp\CSSPDHIFormApp\hydro_Mike21.dfsu";
        #endregion Variables

        #region Properties
        FileInfo fi { get; set; }
        #endregion Properties

        #region Constructors
        public DFSUTest()
        {
            fi = new FileInfo(fileName);
        }
        #endregion Constructors

        #region Functions Test
        [TestMethod]
        public void DFSU_Constructor_OK()
        {
            using (DFSU dfsu = new DFSU(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);
            }
        }
        [TestMethod]
        public void DFSU_Constructor_File_DoesNotExist_From_DHIBase_Error()
        {
            FileInfo fiTemp = new FileInfo(fi.FullName + "aa");
            using (DFSU dfsu = new DFSU(fiTemp))
            {
                Assert.AreEqual(string.Format(CSSPDHIRes.File_DoesNotExist, fiTemp.FullName), dfsu.ErrorMessage);
            }
        }
        [TestMethod]
        public void DFSU_GetStudyArea_Error()
        {
            using (DFSU dfsu = new DFSU(fi))
            {
                List<List<Node>> StudyAreaPolygons = new List<List<Node>>();

                dfsu.GetStudyArea(StudyAreaPolygons);

                Assert.AreEqual(true, true);
            }
        }

        #endregion Functions Test

        #region Functions private
        #endregion Functions private
    }

}
