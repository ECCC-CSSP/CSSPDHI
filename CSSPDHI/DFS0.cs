using DHI.Generic.MikeZero.DFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSPDHI
{
    public class DFS0 : DHIBase
    {
        #region Variables
        #endregion Variables

        #region Properties
        #endregion Properties

        #region Constructors
        public DFS0(FileInfo fi) : base(fi)
        {
        }
        #endregion Constructors

        #region Overrides
        #endregion Overrides

        #region Events
        #endregion Events

        #region Functions public
        public double GetWebTideStepsInMinutes()
        {
            IDfsFile dfs0File = DfsFileFactory.DfsGenericOpen(fi.FullName);

            DfsBuilder dfsNewFile = DfsBuilder.Create(dfs0File.FileInfo.FileTitle, dfs0File.FileInfo.ApplicationTitle, dfs0File.FileInfo.ApplicationVersion);

            double WebTideStepsInMinutes = ((double)((IDfsEqCalendarAxis)((dfs0File.FileInfo).TimeAxis)).TimeStep / 60);

            return WebTideStepsInMinutes;
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
