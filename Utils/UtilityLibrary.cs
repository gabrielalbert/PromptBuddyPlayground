using System;
using Npgsql;

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
