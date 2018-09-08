//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE_6502net for full license information.
//
//-----------------------------------------------------------------------------

namespace DotNetAsm
{
    /// <summary>
    ///The base assembler class. Must be inherited.
    /// </summary>
    public abstract class AssemblerBase
    {
        #region Members

        IAssemblyController _controller;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs an instance of the class implementing the base class.
        /// </summary>
        /// <param name="controller">The <see cref="T:DotNetAsm.IAssemblyController"/>.</param>
        protected AssemblerBase(IAssemblyController controller)
        {
            Controller = controller;

            if (controller == null)
                Reserved = new ReservedWords();
            else
                Reserved = new ReservedWords(Controller.Options.StringComparar);
        }

        /// <summary>
        /// Constructs an instance of the class implementing the base class.
        /// </summary>
        protected AssemblerBase() :
            this(null)
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the token is a reserved word to the assembler object.
        /// </summary>
        /// <param name="token">The token to check if reserved</param>
        /// <returns><c>True</c> if reserved, otherwise <c>false</c>.</returns>
        public virtual bool IsReserved(string token) { return false; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the reserved keywords of the object.
        /// </summary>
        protected ReservedWords Reserved { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="T:DotNetAsm.IAssemblyController"/>.
        /// </summary>
        protected IAssemblyController Controller
        {
            get { return _controller; }
            set {_controller = value; }
        }

        #endregion
    }
}
