using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSTD;

namespace SqlOrm
{
    public class DBExpressionBuilder
    {

        #region MemberPath

        public bool IsSameObjectAs(MemberPath path1, MemberPath path2)
        {
            return path1.CommonMembersWith(path2) == path1.Count -1;
        }

        // path1 a.b.c.val
        // path2 a.b.val
        public bool IsHigherLevelThan(MemberPath path1, MemberPath path2)
        {
            int _commonObjects = path1.CommonMembersWith(path2);
            return (_commonObjects == path2.Count - 1 && path1.Count >= path2.Count) ;
        }

        // Path  a.b.c.val
        // other a.b.val
        public bool IsLowerLevelThan(MemberPath path1, MemberPath path2)
        {
            return path1.CommonMembersWith(path2) == path2.Count - 1 && path1.Count > path2.Count;
        }

        #endregion MemberPath

        #region symbols

        public static bool IsSymbol(ref string s)
        {
            return IsArithmeticOperator(s)
                || IsOperatorComparison(ref s)
                || IsOperatorLogic(ref s)
                || IsPunctuation(s);
        }

        public static bool IsOperatorComparison(ref string s)
        {
            if(s == "==")
            {
                s = "=";
                return true;
            }
            return Array.IndexOf(__operatorsComparison, s) > -1;
        }

        public static bool IsOperatorLogic(ref string s)
        {
            
            if(s == "&&")
            {
                s = "and";
                return true;
            }
            if(s == "||")
            {
                s = "or";
                return true;
            }
            return Array.IndexOf(__operatorsLogic, s) > -1;
        }

        public static bool IsArithmeticOperator(string s)
        {
            return Array.IndexOf(__operatorsArithmetic, s) > -1;
        }

        public static bool IsPunctuation(string s)
        {
            return s == "(" || s == ")" || s == "{" || s == "}";
        } 
        
        private static string[] __operatorsLogic = new string[]{"and", "or"};
        private static string[] __operatorsArithmetic = new string[]{"+", "-", "*", "/", "%"};
        private static string[] __operatorsComparison = new string[]{"=", "!=", "<>", "<", "<=", ">", ">=", "!<", "!>"};

        #endregion symbols
    }
}
