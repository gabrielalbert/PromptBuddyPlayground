using System;
using Npgsql;
using PromptEngineering.Models;

namespace PromptEngineering.Utils
{
    /// <summary>  
    /// A utility library containing various helper methods and extensions.  
    /// </summary>  
    public class UtilityLibrary
    {
    }

    #region StringExtensions
    /// <summary>  
    /// Provides extension methods for the <see cref="string"/> class.  
    /// </summary>  
    public static class StringExtensions
    {
        /// <summary>  
        /// Replaces escape characters in the string with custom placeholders.  
        /// </summary>  
        /// <param name="str">The input string.</param>  
        /// <returns>A string with escape characters replaced by custom placeholders.</returns>  
        public static string ReplaceEscapeChars(this string str)
        {            
            return str.Replace("\n", "###linebreake###").Replace("\t", "###wordbreake###").Replace($"\"", "###doublequote###");           
        }

        public static string ReplaceNewLineChars(this string str)
        {
            return str?.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        public static string ReplaceNewLineToSpaceChars(this string str)
        {
            return str?.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
        }

        public static string CleanCliOutput(this string str)
        {
            string prefixResultContent = (str.IndexOf(Configurations.COPILOT_CLI_RESPONSE_SANITIZE1) > 0 ? str.Substring(0, str.IndexOf(Configurations.COPILOT_CLI_RESPONSE_SANITIZE1) + 43) : string.Empty);
            string postfixResultContent = (str.LastIndexOf(Configurations.COPILOT_CLI_RESPONSE_SANITIZE2) > 0 ? str.Substring(str.LastIndexOf(Configurations.COPILOT_CLI_RESPONSE_SANITIZE2)) : string.Empty);
            str = (string.IsNullOrEmpty(prefixResultContent) ? str : str.Replace(prefixResultContent, string.Empty));
            str = (string.IsNullOrEmpty(postfixResultContent) ? str : str.Replace(postfixResultContent, string.Empty));

            return str;
        }
    }
    #endregion

    #region NpgsqlDataReaderExtensions
    /// <summary>  
    /// Provides extension methods for the <see cref="NpgsqlDataReader"/> class.  
    /// </summary>  
    public static class NpgsqlDataReaderExtensions
    {
        /// <summary>  
        /// Checks if the specified column is DBNull.  
        /// </summary>  
        /// <param name="dataReader">The data reader instance.</param>  
        /// <param name="columnName">The name of the column to check.</param>  
        /// <returns><c>true</c> if the column is DBNull; otherwise, <c>false</c>.</returns>  
        public static bool IsDBNull( this NpgsqlDataReader dataReader, string columnName )
        {
            return dataReader[columnName] == DBNull.Value;
        }

        /// <summary>  
        /// Checks if the specified column is DBNull.  
        /// </summary>  
        /// <param name="dataReader">The data reader instance.</param>  
        /// <param name="columnIndex">The index of the column to check.</param>  
        /// <returns><c>true</c> if the column is DBNull; otherwise, <c>false</c>.</returns>  
        public static bool IsDBNull( this NpgsqlDataReader dataReader, int columnIndex )
        {
            return dataReader[columnIndex] == DBNull.Value;
        }
    }
    #endregion
}
