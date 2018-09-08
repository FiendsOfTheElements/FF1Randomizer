//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE_6502net for full license information.
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DotNetAsm
{
    /// <summary>
    /// Represents an in-memory load of a binary file.
    /// </summary>
    public sealed class BinaryFile : IEquatable<BinaryFile>
    {
        #region Constructor

        /// <summary>
        /// Constructs a new instance of a binary file load.
        /// </summary>
        /// <param name="filename">The filename of the binary file.</param>
        public BinaryFile(string filename)
        {
            Filename = filename;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Opens the underlying file specified in the binary file's filenae.
        /// </summary>
        /// <returns><c>True</c> if the file was opened successfully, otherwise <c>false</c>.</returns>
        public bool Open()
        {
            try
            {
                var filename = Filename.TrimOnce('"');
                Data = File.ReadAllBytes(filename).ToList();
                Filename = filename;
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region IEquatable

        /// <summary>
        /// Determines whether this binary file is equal to the other,
        /// based on filename only.
        /// </summary>
        /// <param name="other">The other file.</param>
        /// <returns><c>True</c> if the files (filenames) are equal, otherwise <c>false</c>.</returns>
        public bool Equals(BinaryFile other) => this.Filename == other.Filename;

        #endregion

        #region Override Methods

        /// <summary>
        /// Determines whether this binary file is equal to the other,
        /// based on filename only.
        /// </summary>
        /// <param name="obj">The other file.</param>
        /// <returns><c>True</c> if the files (filenames) are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as BinaryFile;
            return this.Filename == other.Filename;
        }

        /// <summary>
        /// Gets the binary file's unique hash.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => Filename.GetHashCode();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the filename of the binary file.
        /// </summary>
        /// <value>The filename.</value>
        public string Filename { get; private set; }

        /// <summary>
        /// Gets the binary file data.
        /// </summary>
        /// <value>The data.</value>
        public List<byte> Data { get; private set; }

        #endregion
    }
}
