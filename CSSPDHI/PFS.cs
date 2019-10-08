using CSSPDHI.Resources;
using DHI.PFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSPDHI
{
    public class PFS : DHIBase
    {

        #region Variables
        #endregion Variables

        #region Properties
        #endregion Properties

        #region Constructors
        public PFS(FileInfo fi) : base(fi)
        {
        }
        #endregion Constructors

        #region Overrides
        #endregion Overrides

        #region Events
        #endregion Events

        #region Functions public
        public Nullable<T> GetVariable<T>(string Path, string Keyword, int ParameterIndex) where T : struct
        {
            PFSFile pfsFile = null;
            PFSKeyword pfsKeyword = null;
            if (!CheckAll(Path, Keyword, out pfsFile, out pfsKeyword))
            {
                return null;
            }

            try
            {
                switch (typeof(T).ToString())
                {
                    case "System.Boolean":
                        {
                            object value = pfsKeyword.GetParameter(ParameterIndex).ToBoolean();

                            pfsFile.Close();

                            return (T?)value;
                        }
                    case "System.Double":
                        {
                            object value = pfsKeyword.GetParameter(ParameterIndex).ToDouble();

                            pfsFile.Close();

                            return (T?)value;
                        }
                    case "System.Int32":
                    case "System.Int64":
                        {
                            object value = pfsKeyword.GetParameter(ParameterIndex).ToInt();

                            pfsFile.Close();

                            return (T?)value;
                        }
                    case "System.Single":
                        {
                            object value = pfsKeyword.GetParameter(ParameterIndex).ToSingle();

                            pfsFile.Close();

                            return (T?)value;
                        }
                    default:
                        {
                            return null;
                        }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = string.Format(CSSPDHIRes.ParameterIndex_DoesNotExistForSectionPath_AndKeyword_Error_, ParameterIndex, Path, Keyword, ex.Message + (ex.InnerException != null ? " Inner: " + ex.InnerException.Message : ""));
                OnCSSPDHIChanged(new CSSPDHIEventArgs(new CSSPDHIMessage("Error", -1, false, ErrorMessage)));
                pfsFile.Close();
                return null;
            }
        }
        public DateTime? GetVariableDateTime(string Path, string Keyword)
        {
            PFSFile pfsFile = null;
            PFSKeyword pfsKeyword = null;
            if (!CheckAll(Path, Keyword, out pfsFile, out pfsKeyword))
            {
                return null;
            }
            try
            {

                int Year = pfsKeyword.GetParameter(1).ToInt();
                int Month = pfsKeyword.GetParameter(2).ToInt();
                int Day = pfsKeyword.GetParameter(3).ToInt();
                int Hour = pfsKeyword.GetParameter(4).ToInt();
                int Minute = pfsKeyword.GetParameter(5).ToInt();
                int Second = pfsKeyword.GetParameter(6).ToInt();
                DateTime dateTime = new DateTime(Year, Month, Day, Hour, Minute, Second);

                pfsFile.Close();

                return dateTime;
            }
            catch (Exception ex)
            {
                ErrorMessage = string.Format(CSSPDHIRes.CouldNotReadValueWithPath_AndKeyword_Error_, Path, Keyword, ex.Message + (ex.InnerException != null ? " Inner: " + ex.InnerException.Message : ""));
                OnCSSPDHIChanged(new CSSPDHIEventArgs(new CSSPDHIMessage("Error", -1, false, ErrorMessage)));
                pfsFile.Close();
                return null;
            }
        }
        public FileInfo GetVariableFileInfo(string Path, string Keyword, int ParameterIndex)
        {
            PFSFile pfsFile = null;
            PFSKeyword pfsKeyword = null;
            if (!CheckAll(Path, Keyword, out pfsFile, out pfsKeyword))
            {
                return null;
            }
            try
            {
                FileInfo fileInfo = new FileInfo(pfsKeyword.GetParameter(ParameterIndex).ToFileName());

                pfsFile.Close();

                return fileInfo;
            }
            catch (Exception ex)
            {
                ErrorMessage = string.Format(CSSPDHIRes.ParameterIndex_DoesNotExistForSectionPath_AndKeyword_Error_, ParameterIndex, Path, Keyword, ex.Message + (ex.InnerException != null ? " Inner: " + ex.InnerException.Message : ""));
                OnCSSPDHIChanged(new CSSPDHIEventArgs(new CSSPDHIMessage("Error", -1, false, ErrorMessage)));
                pfsFile.Close();
                return null;
            }
        }
        public string GetVariableString(string Path, string Keyword, int ParameterIndex)
        {
            PFSFile pfsFile = null;
            PFSKeyword pfsKeyword = null;
            if (!CheckAll(Path, Keyword, out pfsFile, out pfsKeyword))
            {
                return "";
            }
            try
            {
                string value = pfsKeyword.GetParameter(ParameterIndex).ToString();

                pfsFile.Close();

                return value;
            }
            catch (Exception ex)
            {
                ErrorMessage = string.Format(CSSPDHIRes.ParameterIndex_DoesNotExistForSectionPath_AndKeyword_Error_, ParameterIndex, Path, Keyword, ex.Message + (ex.InnerException != null ? " Inner: " + ex.InnerException.Message : ""));
                OnCSSPDHIChanged(new CSSPDHIEventArgs(new CSSPDHIMessage("Error", -1, false, ErrorMessage)));
                pfsFile.Close();
                return null;
            }
        }
        public Nullable<T> SetVariable<T>(string Path, string Keyword, int ParameterIndex, T value) where T : struct
        {
            PFSFile pfsFile = null;
            PFSKeyword pfsKeyword = null;
            if (!CheckAll(Path, Keyword, out pfsFile, out pfsKeyword))
            {
                return null;
            }

            try
            {
                switch (typeof(T).ToString())
                {
                    case "System.Boolean":
                        {
                            object obj = value;
                            pfsKeyword.DeleteParameter(ParameterIndex);
                            pfsKeyword.InsertNewParameterBool((bool)obj, ParameterIndex);
                            object boolValue = pfsKeyword.GetParameter(ParameterIndex).ToBoolean();

                            pfsFile.Write(fi.FullName);
                            pfsFile.Close();

                            return (T?)boolValue;
                        }
                    case "System.Double":
                        {
                            object obj = value;
                            pfsKeyword.DeleteParameter(ParameterIndex);
                            pfsKeyword.InsertNewParameterDouble((double)obj, ParameterIndex);
                            object doubleValue = pfsKeyword.GetParameter(ParameterIndex).ToDouble();

                            pfsFile.Write(fi.FullName);
                            pfsFile.Close();

                            return (T?)doubleValue;
                        }
                    case "System.Int32":
                    case "System.Int64":
                        {
                            object obj = value;
                            pfsKeyword.DeleteParameter(ParameterIndex);
                            pfsKeyword.InsertNewParameterInt((int)obj, ParameterIndex);
                            object intValue = pfsKeyword.GetParameter(ParameterIndex).ToInt();

                            pfsFile.Write(fi.FullName);
                            pfsFile.Close();

                            return (T?)intValue;
                        }
                    case "System.Single":
                        {
                            object obj = value;
                            pfsKeyword.DeleteParameter(ParameterIndex);
                            pfsKeyword.InsertNewParameterDouble((float)obj, ParameterIndex);
                            object singleValue = pfsKeyword.GetParameter(ParameterIndex).ToSingle();

                            pfsFile.Write(fi.FullName);
                            pfsFile.Close();

                            return (T?)singleValue;
                        }
                    default:
                        {
                            return null;
                        }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = string.Format(CSSPDHIRes.ParameterIndex_DoesNotExistForSectionPath_AndKeyword_Error_, ParameterIndex, Path, Keyword, ex.Message + (ex.InnerException != null ? " Inner: " + ex.InnerException.Message : ""));
                OnCSSPDHIChanged(new CSSPDHIEventArgs(new CSSPDHIMessage("Error", -1, false, ErrorMessage)));
                pfsFile.Close();
                return null;
            }
        }
        public DateTime? SetVariableDateTime(string Path, string Keyword, DateTime value)
        {
            PFSFile pfsFile = null;
            PFSKeyword pfsKeyword = null;
            if (!CheckAll(Path, Keyword, out pfsFile, out pfsKeyword))
            {
                return null;
            }

            try
            {
                pfsKeyword.DeleteParameter(6);
                pfsKeyword.DeleteParameter(5);
                pfsKeyword.DeleteParameter(4);
                pfsKeyword.DeleteParameter(3);
                pfsKeyword.DeleteParameter(2);
                pfsKeyword.DeleteParameter(1);
                pfsKeyword.InsertNewParameterInt(value.Year, 1);
                pfsKeyword.InsertNewParameterInt(value.Month, 2);
                pfsKeyword.InsertNewParameterInt(value.Day, 3);
                pfsKeyword.InsertNewParameterInt(value.Hour, 4);
                pfsKeyword.InsertNewParameterInt(value.Minute, 5);
                pfsKeyword.InsertNewParameterInt(value.Second, 6);

                int Year = pfsKeyword.GetParameter(1).ToInt();
                int Month = pfsKeyword.GetParameter(2).ToInt();
                int Day = pfsKeyword.GetParameter(3).ToInt();
                int Hour = pfsKeyword.GetParameter(4).ToInt();
                int Minute = pfsKeyword.GetParameter(5).ToInt();
                int Second = pfsKeyword.GetParameter(6).ToInt();
                DateTime dateTime = new DateTime(Year, Month, Day, Hour, Minute, Second);

                pfsFile.Write(fi.FullName);
                pfsFile.Close();

                return dateTime;
            }
            catch (Exception ex)
            {
                // need to write test code for this part
                ErrorMessage = string.Format(CSSPDHIRes.CouldNotSetStartTimeWithPath_Keyword_Error_, Path, Keyword, ex.Message + (ex.InnerException != null ? " Inner: " + ex.InnerException.Message : ""));
                OnCSSPDHIChanged(new CSSPDHIEventArgs(new CSSPDHIMessage("Error", -1, false, ErrorMessage)));
                return null;
            }


        }
        public FileInfo SetVariableFileInfo(string Path, string Keyword, int ParameterIndex, FileInfo fileInfoValue)
        {
            PFSFile pfsFile = null;
            PFSKeyword pfsKeyword = null;
            if (!CheckAll(Path, Keyword, out pfsFile, out pfsKeyword))
            {
                return null;
            }

            try
            {
                pfsKeyword.DeleteParameter(ParameterIndex);
                pfsKeyword.InsertNewParameterFileName(fileInfoValue.FullName, ParameterIndex);
                FileInfo fileInfo = new FileInfo(pfsKeyword.GetParameter(ParameterIndex).ToFileNamePath());

                pfsFile.Write(fi.FullName);
                pfsFile.Close();

                return fileInfo;
            }
            catch (Exception ex)
            {
                ErrorMessage = string.Format(CSSPDHIRes.ParameterIndex_DoesNotExistForSectionPath_AndKeyword_Error_, ParameterIndex, Path, Keyword, ex.Message + (ex.InnerException != null ? " Inner: " + ex.InnerException.Message : ""));
                OnCSSPDHIChanged(new CSSPDHIEventArgs(new CSSPDHIMessage("Error", -1, false, ErrorMessage)));
                return null;
            }
        }
        public string SetVariableString(string Path, string Keyword, int ParameterIndex, string value)
        {
            PFSFile pfsFile = null;
            PFSKeyword pfsKeyword = null;
            if (!CheckAll(Path, Keyword, out pfsFile, out pfsKeyword))
            {
                return null;
            }

            try
            {
                pfsKeyword.DeleteParameter(ParameterIndex);
                pfsKeyword.InsertNewParameterString(value, ParameterIndex);
                string strValue = pfsKeyword.GetParameter(ParameterIndex).ToString();

                pfsFile.Write(fi.FullName);
                pfsFile.Close();

                return strValue;
            }
            catch (Exception ex)
            {
                ErrorMessage = string.Format(CSSPDHIRes.ParameterIndex_DoesNotExistForSectionPath_AndKeyword_Error_, ParameterIndex, Path, Keyword, ex.Message + (ex.InnerException != null ? " Inner: " + ex.InnerException.Message : ""));
                OnCSSPDHIChanged(new CSSPDHIEventArgs(new CSSPDHIMessage("Error", -1, false, ErrorMessage)));
                return null;
            }
        }
        #endregion Functions public

        #region Functions protected
        #endregion Functions protected

        #region Functions private
        private bool CheckAll(string path, string keyword, out PFSFile pfsFile, out PFSKeyword pfsKeyword)
        {
            if (!CheckFileExtension())
            {
                // ErrorMessage set in CheckFileExtension
                pfsFile = null;
                pfsKeyword = null;
                return false;
            }

            if (!TryToOpenPFSFile(out pfsFile))
            {
                // ErrorMessage set in CheckFileExtension
                pfsFile = null;
                pfsKeyword = null;
                return false;
            }

            PFSSection pfsSection = null;
            if (!TryToFindSection(pfsFile, out pfsSection, path))
            {
                // ErrorMessage set in CheckFileExtension
                pfsFile.Close();
                pfsFile = null;
                pfsKeyword = null;
                return false;
            }

            if (!TryToGetKeyword(pfsSection, out pfsKeyword, keyword, path))
            {
                // ErrorMessage set in CheckFileExtension
                pfsFile.Close();
                pfsFile = null;
                pfsKeyword = null;
                return false;
            }

            return true;
        }
        private bool CheckFileExtension()
        {
            List<string> extList = new List<string>() { ".m21fm", ".m3fm" };

            if (!fi.Exists)
            {
                ErrorMessage = string.Format(CSSPDHIRes.File_DoesNotExist, fi.FullName);
                OnCSSPDHIChanged(new CSSPDHIEventArgs(new CSSPDHIMessage("Error", -1, false, ErrorMessage)));
                return false;
            }

            if (!extList.Contains(fi.Extension))
            {
                ErrorMessage = string.Format(CSSPDHIRes.WrongFileTypeLookingFor_, "[.m21fm or .m3fm]");
                OnCSSPDHIChanged(new CSSPDHIEventArgs(new CSSPDHIMessage("Error", -1, false, ErrorMessage)));
                return false;
            }

            return true;
        }
        private bool TryToFindSection(PFSFile pfsFile, out PFSSection pfsSection, string Path)
        {
            pfsSection = pfsFile.GetSectionFromHandle(Path);

            if (pfsSection == null)
            {
                ErrorMessage = string.Format(CSSPDHIRes.CouldNotFindSectionForPath_, Path);
                OnCSSPDHIChanged(new CSSPDHIEventArgs(new CSSPDHIMessage("Error", -1, false, ErrorMessage)));
                pfsFile.Close();
                return false;
            }

            return true;
        }
        private bool TryToGetKeyword(PFSSection pfsSection, out PFSKeyword pfsKeyword, string Keyword, string Path)
        {
            try
            {
                pfsKeyword = pfsSection.GetKeyword(Keyword);
            }
            catch (Exception ex)
            {
                ErrorMessage = string.Format(CSSPDHIRes.Keyword_DoesNotExistForSectionPath_Error_, Keyword, Path, ex.Message + (ex.InnerException != null ? " Inner: " + ex.InnerException.Message : ""));
                OnCSSPDHIChanged(new CSSPDHIEventArgs(new CSSPDHIMessage("Error", -1, false, ErrorMessage)));
                pfsKeyword = null;
                return false;
            }

            return true;
        }
        private bool TryToOpenPFSFile(out PFSFile pfsFile)
        {
            try
            {
                pfsFile = new PFSFile(fi.FullName, true);
            }
            catch (Exception ex)
            {
                ErrorMessage = string.Format(CSSPDHIRes.PotentiallyWrongFileTypeLookingFor_Error_, "[.m21fm or .m3fm]", ex.Message + (ex.InnerException != null ? " Inner: " + ex.InnerException.Message : ""));
                OnCSSPDHIChanged(new CSSPDHIEventArgs(new CSSPDHIMessage("Error", -1, false, ErrorMessage)));
                pfsFile = null;
                return false;
            }

            return true;
        }
        #endregion Functions private

    }
}
