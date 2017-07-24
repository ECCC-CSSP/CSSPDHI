using CSSPDHI.Resources;
using CSSPModelsDLL.Models;
using DHI.Generic.MikeZero.DFS.dfsu;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSPDHI
{
    public class DHIBase : IDisposable
    {
        #region Variables
        public double R = 6378137.0;
        public double d2r = Math.PI / 180;
        #endregion Variables

        #region Properties
        public string ErrorMessage { get; set; }
        public FileInfo fi { get; set; }
        private bool _disposed { get; set; }
        #endregion Properties

        #region Constructors
        public DHIBase(FileInfo fi)
        {
            this.fi = fi;
            this.ErrorMessage = "";
            _disposed = false;

            if (!CheckFileExist(fi))
            {
                return;
            }
        }

        #endregion Constructors

        #region Overrides
        #endregion Overrides

        #region Events
        public event EventHandler<CSSPDHIEventArgs> CSSPDHIChanged;
        #endregion Events

        #region Functions public
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion Functions public

        #region Functions protected
        public void OnCSSPDHIChanged(CSSPDHIEventArgs e)
        {
            EventHandler<CSSPDHIEventArgs> handler = CSSPDHIChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion Functions protected

        #region Functions private
        private bool CheckFileExist(FileInfo fi)
        {
            if (!fi.Exists)
            {
                ErrorMessage = string.Format(CSSPDHIRes.File_DoesNotExist, fi.FullName);
                return false;
            }

            return true;
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    fi = null;
                }

                _disposed = true;
            }
        }
        #endregion Functions private

        #region SubClass
        public class CSSPDHIEventArgs : EventArgs
        {
            #region Properties
            public CSSPDHIMessage CSSPDHIMessage { get; private set; }
            #endregion Properties

            #region Constructors
            public CSSPDHIEventArgs(CSSPDHIMessage csspDHIMessage)
            {
                CSSPDHIMessage = csspDHIMessage;
            }
            #endregion Constructors
        }
        public class CSSPDHIMessage
        {
            #region Properties
            public string Status { get; set; }
            public float Progress { get; set; }
            public bool Completed { get; set; }
            public string ErrorMessage { get; set; }
            #endregion Properties

            #region Constructors
            public CSSPDHIMessage(string Status, float Progress, bool Completed, string ErrorMessage)
            {
                this.Status = Status;
                this.Progress = Progress;
                this.Completed = Completed;
                this.ErrorMessage = ErrorMessage;
            }
            #endregion Constructors
        }

        #endregion SubClass
    }
}
