﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSPDHI
{
    public class MDF : DHIBase
    {
        #region Variables
        #endregion Variables

        #region Properties
        #endregion Properties

        #region Constructors
        public MDF(FileInfo fi) : base(fi)
        {
        }
        #endregion Constructors

        #region Overrides
        #endregion Overrides

        #region Events
        #endregion Events

        #region Functions public
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
