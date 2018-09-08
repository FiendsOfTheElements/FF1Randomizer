//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to 
// deal in the Software without restriction, including without limitation the 
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

using System;

namespace DotNetAsm
{
    /// <summary>
    /// Defines an interface for an expression evaluator that can evaluate mathematical 
    /// expressions from strings.
    /// </summary>
    public interface IEvaluator
    {
        /// <summary>
        /// Evaluates a text string as a mathematical expression.
        /// </summary>
        /// <param name="expression">The string representation of the mathematical expression.</param>
        /// <returns>The result of the expression evaluation.</returns>
        long Eval(string expression);

        /// <summary>
        /// Evaluates a text string as a mathematical expression.
        /// </summary>
        /// <param name="expression">The string representation of the mathematical expression.</param>
        /// <param name="minval">The minimum value of expression. If the evaluated value is
        /// lower, then an exception will occur.</param>
        /// <param name="maxval">The maximum value of the expression. If the evaluated value 
        /// is higher, then an exception will occur.</param>
        /// <returns>The result of the expression evaluation.</returns>
        long Eval(string expression, long minval, long maxval);

        /// <summary>
        /// Evaluates a text string as a conditional (boolean) evaluation.
        /// </summary>
        /// <param name="expression">The string representation of the conditional expression.</param>
        /// <returns><c>True</c> if the expression is true, otherwise <c>false</c>.</returns>
        bool EvalCondition(string expression);

        /// <summary>
        /// Defines a symbol lookup for the evaluator to translate symbols (such as 
        /// variables) in expressions.
        /// </summary>
        /// <param name="lookupFunc">The lookup function to define the symbol.</param>
        void DefineSymbolLookup(Func<string, string> lookupFunc);

        /// <summary>
        /// Defines a symbol lookup for the evaluator to translate symbols (such as 
        /// variables) in expressions.
        /// </summary>
        /// <param name="regex">A regex pattern for the symbol.</param>
        /// <param name="lookupfunc">The lookup function to define the symbol.</param>
        void DefineSymbolLookup(string regex, Func<string, string> lookupfunc);
    }
}
