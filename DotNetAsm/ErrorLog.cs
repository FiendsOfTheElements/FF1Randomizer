//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE_6502net for full license information.
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DotNetAsm
{
    /// <summary>
    /// A simple, flexible error log.
    /// </summary>
    public class ErrorLog
    {
        readonly
        #region Members

        List<Tuple<string, bool>> _errors;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructs an instance of the ErrorLog class.
        /// </summary>
        public ErrorLog() => _errors = new List<Tuple<string, bool>>();
        #endregion

        #region Methods

        /// <summary>
        /// Clear all logged messages.
        /// </summary>
        public void ClearAll() => _errors.Clear();

        /// <summary>
        /// Clear all logged errors.
        /// </summary>
        public void ClearErrors() => _errors.RemoveAll(e => e.Item2);

        /// <summary>
        /// Clears all logged warnings.
        /// </summary>
        public void ClearWarnings() => _errors.RemoveAll(e => e.Item2 == false);

        /// <summary>
        /// Dumps all logged messages to console output.
        /// </summary>
        public void DumpAll() => _errors.ForEach(e => Console.WriteLine(e.Item1));

        /// <summary>
        /// Dumps all logged errors to console output.
        /// </summary>
        public void DumpErrors()
        {
            _errors.Where(e => e.Item2).ToList()
                   .ForEach(error => Console.WriteLine(error.Item1));
        }

        /// <summary>
        /// Dumps all logged warnings to console output.
        /// </summary>
        public void DumpWarnings()
        {
            _errors.Where(e => e.Item2 == false).ToList()
                   .ForEach(warning => Console.WriteLine(warning.Item1));
        }

        /// <summary>
        /// Log a message.
        /// </summary>
        /// <param name="filename">The source file.</param>
        /// <param name="linenumber">The source line number.</param>
        /// <param name="message">The custom string message.</param>
        /// <param name="source">The error source.</param>
        /// <param name="isError">(Optional) indicate if the mesage is an error.</param>
        public void LogEntry(string filename, int linenumber, string message, object source, bool isError = true)
        {
            var sb = new StringBuilder();

            if (string.IsNullOrEmpty(filename))
            {
                if (isError)
                    sb.Append("Syntax error: ");
                else
                    sb.Append("Warning: ");
            }
            else
            {
                if (isError)
                    sb.AppendFormat("Error in file '{0}' at line {1}: ", filename, linenumber);
                else
                    sb.AppendFormat("Warning in file '{0}' at line {1}: ", filename, linenumber);
            }
            if (source == null)
                sb.Append(message.Replace(" '{0}'", string.Empty).Trim());
            else if (message.Contains("'{0}'"))
                sb.AppendFormat(message, source);

            _errors.Add(new Tuple<string, bool>(sb.ToString(), isError));
        }

        /// <summary>
        /// Log a message.
        /// </summary>
        /// <param name="filename">The source file.</param>
        /// <param name="linenumber">The source line number.</param>
        /// <param name="message">The custom string message.</param>
        /// <param name="isError">(Optional) indicate if the mesage is an error.</param>
        public void LogEntry(string filename, int linenumber, string message, bool isError = true)
        {
            LogEntry(filename, linenumber, message, null, isError);
        }

        /// <summary>
        /// Log a message.
        /// </summary>
        /// <param name="filename">The source file.</param>
        /// <param name="linenumber">The source line number.</param>
        /// <param name="isError">(Optional) indicate if the mesage is an error.</param>
        public void LogEntry(string filename, int linenumber, bool isError = true)
        {
            LogEntry(filename, linenumber, string.Empty, isError);
        }

        /// <summary>
        /// Log a message.
        /// </summary>
        /// <param name="line">The The <see cref="T:DotNetAsm.SourceLine"/>.</param>
        /// <param name="message">The custom string message.</param>
        /// <param name="source">The error source.</param>
        /// <param name="isError">(Optional) indicate if the mesage is an error.</param>
        public void LogEntry(SourceLine line, string message, object source, bool isError = true)
        {
            LogEntry(line.Filename, line.LineNumber, message, source, isError);
        }

        /// <summary>
        /// Log a message.
        /// </summary>
        /// <param name="line">The The <see cref="T:DotNetAsm.SourceLine"/>.</param>
        /// <param name="message">The custom string message.</param>
        /// <param name="isError">(Optional) indicate if the mesage is an error.</param>
        public void LogEntry(SourceLine line, string message, bool isError = true)
        {
            LogEntry(line.Filename, line.LineNumber, message, null, isError);
        }

        /// <summary>
        /// Log a message.
        /// </summary>
        /// <param name="line">The <see cref="T:DotNetAsm.SourceLine"/>.</param>
        /// <param name="isError">(Optional) indicate if the mesage is an error.</param>
        public void LogEntry(SourceLine line, bool isError = true) => LogEntry(line.Filename, line.LineNumber, isError);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the log entries
        /// </summary>
        public ReadOnlyCollection<string> Entries => _errors.Select(e => e.Item1).ToList().AsReadOnly();

        /// <summary>
        /// Gets if the log has errors.
        /// </summary>
        public bool HasErrors => _errors.Any(e => e.Item2);

        /// <summary>
        /// Gets if the log has warnings.
        /// </summary>
        public bool HasWarnings => _errors.Any(e => e.Item2 == false);

        /// <summary>
        /// Gets the error count in the log.
        /// </summary>
        public int ErrorCount => _errors.Count(e => e.Item2 == true);

        /// <summary>
        /// Gets the warning count in the log.
        /// </summary>
        public int WarningCount => _errors.Count(w => w.Item2 == false);

        #endregion
    }
}
