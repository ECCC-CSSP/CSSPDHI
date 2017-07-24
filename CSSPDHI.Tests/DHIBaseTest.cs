using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using CSSPDHI.Resources;

namespace CSSPDHI.Tests
{
    /// <summary>
    /// Summary description for BaseTest
    /// </summary>
    [TestClass]
    public class DHIBaseTest
    {
        #region Variables
        private string strTest = "Testing123";
        #endregion Variables

        #region Properties
        #endregion Properties

        #region Constructors
        public DHIBaseTest()
        {
        }
        #endregion Constructors

        #region Functions Test
        [TestMethod]
        public void DHIBase_Constructor_OK()
        {
            string fileName = @"C:\CSSP Latest Code Old\CSSPDHIFormApp\CSSPDHIFormApp\Gaspe.m21fm";
            FileInfo fi = new FileInfo(fileName);
            using (DHIBase dhiBase = new DHIBase(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsTrue(fi.Exists);
                Assert.AreEqual(fileName, fi.FullName);
                Assert.AreEqual("", dhiBase.ErrorMessage);
            }
        }

        [TestMethod]
        public void DHIBase_Constructor_CheckFileExist_Error()
        {
            string fileName = @"C:\CSSP Latest Code Old\CSSPDHIFormApp\CSSPDHIFormApp\Gaspe_NotExist.m21fm";
            FileInfo fi = new FileInfo(fileName);
            using (DHIBase dhiBase = new DHIBase(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsFalse(fi.Exists);
                Assert.AreEqual(string.Format(CSSPDHIRes.File_DoesNotExist, fi.FullName), dhiBase.ErrorMessage);
            }
        }
        [TestMethod]
        public void DHIBase_Constructor_OnCSSPDHIChanged_Error()
        {
            string fileName = @"C:\CSSP Latest Code Old\CSSPDHIFormApp\CSSPDHIFormApp\Gaspe_NotExist.m21fm";
            FileInfo fi = new FileInfo(fileName);
            using (DHIBase dhiBase = new DHIBase(fi))
            {
                Assert.IsNotNull(fi);
                Assert.IsFalse(fi.Exists);
                Assert.AreEqual(string.Format(CSSPDHIRes.File_DoesNotExist, fi.FullName), dhiBase.ErrorMessage);

                Assert.AreEqual("Testing123", strTest);

                dhiBase.CSSPDHIChanged += CSSPDHIChanged;

                string status = "This is status";
                float progress = 34.5f;
                bool completed = false;
                string errorMessage = "This is the error message";
                dhiBase.OnCSSPDHIChanged(new DHIBase.CSSPDHIEventArgs(new DHIBase.CSSPDHIMessage(status, progress, completed, errorMessage)));

                Assert.AreEqual("Testing234", strTest);
            }
        }
        #endregion Functions Test

        #region Functions private
        private void CSSPDHIChanged(object sender, DHIBase.CSSPDHIEventArgs e)
        {
            strTest = "Testing234";
        }
        #endregion Functions private

    }
}
