//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE_6502net for full license information.
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.CommandLine;

namespace DotNetAsm
{
    /// <summary>
    /// A helper class to parse and present strongly-typed options from the command-line.
    /// </summary>
    public class AsmCommandLineOptions
    {
        #region Members

        IReadOnlyList<string> _source;
        IReadOnlyList<string> _defines;
        string _arch;
        string _cpu;
        string _listingFile;
        string _labelFile;
        string _outputFile;
        bool _bigEndian;
        bool _quiet;
        bool _verboseDasm;
        bool _werror;
        bool _noWarn;
        bool _caseSensitive;
        bool _noAssembly;
        bool _noSource;
        bool _printVersion;
        bool _noDisassembly;
        bool _warnLeft;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new instance of the AsmCommandLineOptions.
        /// </summary>
        public AsmCommandLineOptions()
        {
            _source = new List<string>();
            _defines = new List<string>();
            _arch =
            _cpu =
            _listingFile =
            _labelFile =
            _outputFile = string.Empty;
            _bigEndian =
            _verboseDasm =
            _werror =
            _noWarn =
            _warnLeft =
            _noDisassembly =
            _noSource =
            _noAssembly =
            _quiet =
            _printVersion =
            _caseSensitive = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process the command-line arguments passed by the end-user.
        /// </summary>
        /// <param name="args">The argument string.</param>
        public void ProcessArgs(string[] args)
        {
            if (args.Length == 0)
            {
                throw new Exception("One or more arguments expected. Try '-?|-h|help' for usage.");
            }
            Arguments = new string[args.Length];

            args.CopyTo(Arguments, 0);
            var result = ArgumentSyntax.Parse(args, syntax =>
            {
                syntax.DefineOption("o|output",         ref _outputFile,    "Output assembly to <arg>");
                syntax.DefineOption("b|big-endian",     ref _bigEndian,     "Set byte order of output to big-endian");
                syntax.DefineOption("arch",             ref _arch,          "Specify architecture-specific options");
                syntax.DefineOption("cpu",              ref _cpu,           "Specify the target CPU and instruction set");
                syntax.DefineOptionList("D|define",     ref _defines,       "Assign value to a global symbol/label in <arg>");
                syntax.DefineOption("q|quiet",          ref _quiet,         "Assemble in quiet mode (no console messages)");
                syntax.DefineOption("w|no-warn",        ref _noWarn,        "Suppress all warnings");
                syntax.DefineOption("werror",           ref _werror,        "Treat all warnings as errors");
                syntax.DefineOption("wleft",            ref _warnLeft,      "Issue warnings about whitespaces before labels");
                syntax.DefineOption("l|labels",         ref _labelFile,     "Output label definitions to <arg>");
                syntax.DefineOption("L|list",           ref _listingFile,   "Output listing to <arg>");
                syntax.DefineOption("a|no-assembly",    ref _noAssembly,    "Suppress assembled bytes from assembly listing");
                syntax.DefineOption("d|no-disassembly", ref _noDisassembly, "Suppress disassembly from assembly listing");
                syntax.DefineOption("s|no-source",      ref _noSource,      "Suppress original source from assembly listing");
                syntax.DefineOption("verbose-asm",      ref _verboseDasm,   "Expand listing to include all directives and comments");
                syntax.DefineOption("C|case-sensitive", ref _caseSensitive, "Treat all symbols as case sensitive");
                syntax.DefineOption("V|version",        ref _printVersion,  "Print current version");
                syntax.DefineParameterList("source",    ref _source,        "The source files to assemble");
            });
            ArgsPassed = args.Length;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the argument string array passed.
        /// </summary>
        public string[] Arguments { get; set; }

        /// <summary>
        /// Gets or sets the target architecture information.
        /// </summary>
        public string Architecture { get { return _arch; } set { _arch = value; } }

        /// <summary>
        /// Gets the selected CPU.
        /// </summary>
        /// <value>The cpu.</value>
        public string CPU { get { return _cpu; } }

        /// <summary>
        /// Gets the value determining whether output file should be generated,
        /// based on the criterion that input files were specified and either
        /// 1) an output file was also, or that 2) no listing nor label file was.
        /// </summary>
        public bool GenerateOutput
        {
            get
            {
                return _source.Count > 0 &&
                     (
                      !string.IsNullOrEmpty(_outputFile) ||
                      (string.IsNullOrEmpty(_labelFile) && string.IsNullOrEmpty(_listingFile))
                     );
            }
        }

        /// <summary>
        /// Gets the read-only list of input filenames.
        /// </summary>
        public IReadOnlyList<string> InputFiles { get { return _source; } }

        /// <summary>
        /// Gets the read-only list of label defines.
        /// </summary>
        public IReadOnlyList<string> LabelDefines { get { return _defines; } }

        /// <summary>
        /// Gets the output filename.
        /// </summary>
        public string OutputFile { get { return _outputFile; } }

        /// <summary>
        /// The assembly listing filename.
        /// </summary>
        public string ListingFile { get { return _listingFile; } }

        /// <summary>
        /// Gets the label listing filename.
        /// </summary>
        public string LabelFile { get { return _labelFile; } }

        /// <summary>
        /// Gets the flag that indicates assembly should be quiet.
        /// </summary>
        public bool Quiet { get { return _quiet; } }

        /// <summary>
        /// Gets the flag that indicates warnings should be suppressed.
        /// </summary>
        public bool NoWarnings { get { return _noWarn; } }

        /// <summary>
        /// Gets a value indicating whether to suppress warnings for whitespaces before labels.
        /// </summary>
        /// <value>If <c>true</c> warn left; otherwise, suppress the warning.</value>
        public bool WarnLeft { get { return _warnLeft; } }

        /// <summary>
        /// Gets a flag that treats warnings as errors.
        /// </summary>
        public bool WarningsAsErrors
        {
            get
            {
                if (!_noWarn)
                    return _werror;
                return false;
            }
        }

        /// <summary>
        /// Gets the number of arguments passed after the call to
        /// <see cref="T:DotNetAsm.AsmCommandLineOptions.ProcessArgs"/>.
        /// </summary>
        /// <value>The arguments passed.</value>
        public int ArgsPassed { get; private set; }

        /// <summary>
        /// Gets a flag indicating that assembly listing should be
        /// verbose.
        /// </summary>
        public bool VerboseList { get { return _verboseDasm; } }

        /// <summary>
        /// Gets a flag that indicates the source should be processed as
        /// case-sensitive.
        /// </summary>
        public bool CaseSensitive { get { return _caseSensitive; } }

        /// <summary>
        /// Gets the System.StringComparison, which is based on the case-sensitive flag.
        /// </summary>
        public StringComparison StringComparison
        {
            get
            {
                return _caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            }
        }

        /// <summary>
        /// Gets the System.StringComparer, which is based on the case-sensitive flag.
        /// </summary>
        public StringComparer StringComparar
        {
            get
            {
                return _caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
            }
        }

        /// <summary>
        /// Gets the RegexOption flag indicating case-sensitivity based on the case-sensitive flag.
        /// </summary>
        public System.Text.RegularExpressions.RegexOptions RegexOption
        {
            get
            {
                return _caseSensitive ? System.Text.RegularExpressions.RegexOptions.None : System.Text.RegularExpressions.RegexOptions.IgnoreCase;
            }
        }

        /// <summary>
        /// Gets a flag that indicates that the output should be in big-endian byte order.
        /// </summary>
        public bool BigEndian { get { return _bigEndian; } }

        /// <summary>
        /// Gets a flag indicating if assembly listing should suppress original source.
        /// </summary>
        public bool NoSource { get { return _noSource; } }

        /// <summary>
        /// Gets a flag indicating if assembly listing should suppress 6502 disassembly.
        /// </summary>
        public bool NoDissasembly { get { return _noDisassembly; } }

        /// <summary>
        /// Gets a flag indicating if assembly listing should suppress assembly bytes.
        /// </summary>
        public bool NoAssembly { get { return _noAssembly; } }

        /// <summary>
        /// Gets a flag indicating the full version of the assembler should be printed.
        /// </summary>
        public bool PrintVersion { get { return _printVersion; } }

        #endregion
    }
}
