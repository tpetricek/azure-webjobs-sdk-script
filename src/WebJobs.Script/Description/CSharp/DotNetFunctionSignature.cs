// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    /// <summary>
    /// Provides function identity validation and identification.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class DotNetFunctionSignature : IEquatable<DotNetFunctionSignature>
    {
        private readonly ImmutableArray<string> _parameterNames;

        private readonly ImmutableArray<IParameterSymbol> _parameters;

        internal DotNetFunctionSignature(ImmutableArray<string> parameterNames, ImmutableArray<IParameterSymbol> parameters)
        {
            _parameterNames = parameterNames;
            _parameters = parameters;
        }

        /// <summary>
        /// Returns true if the function uses locally defined types (i.e. types defined in the function assembly) in its parameters;
        /// otherwise, false.
        /// </summary>
        public bool HasLocalTypeReference { get; set; }

        public ImmutableArray<string> ParameterNames
        {
            get
            {
                return _parameterNames;
            }
        }

        public ImmutableArray<IParameterSymbol> Parameters
        {
            get
            {
                return _parameters;
            }
        }

        internal static DotNetFunctionSignature FromCompilation(ICompilation compilation, IFunctionEntryPointResolver entryPointResolver)
        {
            if (compilation == null)
            {
                throw new ArgumentNullException("compilation");
            }
            if (entryPointResolver == null)
            {
                throw new ArgumentNullException("entryPointResolver");
            }

            var signature = compilation.FindEntryPoint(entryPointResolver);

            return signature;
        }

        private static bool AreParametersEquivalent(IParameterSymbol param1, IParameterSymbol param2)
        {
            if (ReferenceEquals(param1, param2))
            {
                return true;
            }

            if (param1 == null || param2 == null)
            {
                return false;
            }

            return param1.RefKind == param2.RefKind &&
                string.Compare(param1.Name, param2.Name, StringComparison.Ordinal) == 0 &&
                string.Compare(GetFullTypeName(param1.Type), GetFullTypeName(param2.Type), StringComparison.Ordinal) == 0 &&
                param1.IsOptional == param2.IsOptional;
        }

        private static string GetFullTypeName(ITypeSymbol type)
        {
            if (type == null)
            {
                return string.Empty;
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}, {2}", type.ContainingNamespace.MetadataName, type.MetadataName, type.ContainingAssembly.ToDisplayString());
        }

        public bool Equals(DotNetFunctionSignature other)
        {
            if (other == null)
            {
                return false;
            }

            if (_parameters.Count() != other._parameters.Count())
            {
                return false;
            }

            if (_parameters.Count() != _parameterNames.Count())
            {
                return false;
            }

            if (other._parameters.Count() != other._parameterNames.Count())
            {
                return false;
            }

            return _parameters.Zip(other._parameters, (a, b) => AreParametersEquivalent(a, b)).All(r => r);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DotNetFunctionSignature);
        }

        public override int GetHashCode()
        {
            return string.Join("<>", _parameters.Select(p => GetParameterIdentityString(p)))
                .GetHashCode();
        }

        private static string GetParameterIdentityString(IParameterSymbol parameterSymbol)
        {
            return string.Join("::", parameterSymbol.RefKind, parameterSymbol.Name,
                GetFullTypeName(parameterSymbol.Type), parameterSymbol.IsOptional);
        }
    }
}
