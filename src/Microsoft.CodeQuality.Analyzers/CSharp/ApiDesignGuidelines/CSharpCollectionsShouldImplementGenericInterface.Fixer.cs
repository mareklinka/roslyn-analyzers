// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using Microsoft.CodeQuality.Analyzers.ApiDesignGuidelines;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Microsoft.CodeQuality.CSharp.Analyzers.ApiDesignGuidelines
{
    /// <summary>
    /// CA1010: Collections should implement generic interface
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp), Shared]
    public sealed class CSharpCollectionsShouldImplementGenericInterfaceFixer : CollectionsShouldImplementGenericInterfaceFixer
    {
    }
}