// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Markdig.Helpers
{
    /// <summary>
    /// Inspired by CoreLib, taken from https://github.com/MihaZupan/SharpCollections, cc @MihaZupan
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class ThrowHelper
    {
        private const MethodImplOptions NoInlining = MethodImplOptions.NoInlining;

        [DoesNotReturn]
        [MethodImpl(NoInlining)]
        public static void ArgumentNullException(string paramName) => throw new ArgumentNullException(paramName);

        [DoesNotReturn]
        [MethodImpl(NoInlining)]
        public static void ArgumentNullException_item() => throw new ArgumentNullException("item");

        [DoesNotReturn]
        [MethodImpl(NoInlining)]
        public static void ArgumentNullException_text() => throw new ArgumentNullException("text");

        [DoesNotReturn]
        [MethodImpl(NoInlining)]
        public static void ArgumentNullException_label() => throw new ArgumentNullException("label");

        [DoesNotReturn]
        [MethodImpl(NoInlining)]
        public static void ArgumentNullException_key() => throw new ArgumentNullException("key");

        [DoesNotReturn]
        [MethodImpl(NoInlining)]
        public static void ArgumentNullException_name() => throw new ArgumentNullException("name");

        [DoesNotReturn]
        [MethodImpl(NoInlining)]
        public static void ArgumentNullException_markdown() => throw new ArgumentNullException("markdown");

        [DoesNotReturn]
        [MethodImpl(NoInlining)]
        public static void ArgumentNullException_writer() => throw new ArgumentNullException("writer");

        [DoesNotReturn]
        [MethodImpl(NoInlining)]
        public static void ArgumentNullException_leafBlock() => throw new ArgumentNullException("leafBlock");

        [DoesNotReturn]
        [MethodImpl(NoInlining)]
        public static void ArgumentNullException_markdownObject() => throw new ArgumentNullException("markdownObject");


        [DoesNotReturn]
        [MethodImpl(NoInlining)]
        public static void ArgumentException(string message) => throw new ArgumentException(message);

        [DoesNotReturn]
        [MethodImpl(NoInlining)]
        public static void ArgumentException(string message, string paramName) => throw new ArgumentException(message, paramName);


        [DoesNotReturn]
        [MethodImpl(NoInlining)]
        public static void ArgumentOutOfRangeException(string paramName) => throw new ArgumentOutOfRangeException(paramName);

        [DoesNotReturn]
        [MethodImpl(NoInlining)]
        public static void ArgumentOutOfRangeException(string message, string paramName) => throw new ArgumentOutOfRangeException(message, paramName);

        [DoesNotReturn]
        [MethodImpl(NoInlining)]
        public static void ArgumentOutOfRangeException_index() => throw new ArgumentOutOfRangeException("index");


        [DoesNotReturn]
        [MethodImpl(NoInlining)]
        public static void InvalidOperationException(string message) => throw new InvalidOperationException(message);


        [DoesNotReturn]
        [MethodImpl(NoInlining)]
        public static void ThrowArgumentNullException(ExceptionArgument argument)
        {
            throw new ArgumentNullException(argument.ToString());
        }

        [DoesNotReturn]
        [MethodImpl(NoInlining)]
        public static void ThrowArgumentException(ExceptionArgument argument, ExceptionReason reason)
        {
            throw new ArgumentException(argument.ToString(), GetExceptionReason(reason));
        }

        [DoesNotReturn]
        [MethodImpl(NoInlining)]
        public static void ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionReason reason)
        {
            throw new ArgumentOutOfRangeException(argument.ToString(), GetExceptionReason(reason));
        }

        [DoesNotReturn]
        [MethodImpl(NoInlining)]
        public static void ThrowIndexOutOfRangeException()
        {
            throw new IndexOutOfRangeException();
        }

        private static string GetExceptionReason(ExceptionReason reason)
        {
            switch (reason)
            {
                case ExceptionReason.String_Empty:
                    return "String must not be empty.";

                case ExceptionReason.SmallCapacity:
                    return "Capacity was less than the current size.";

                case ExceptionReason.InvalidOffsetLength:
                    return "Offset and length must refer to a position in the string.";

                case ExceptionReason.DuplicateKey:
                    return "The given key is already present in the dictionary.";

                default:
                    Debug.Assert(false, "The enum value is not defined, please check the ExceptionReason Enum.");
                    return "";
            }
        }
    }

    internal enum ExceptionArgument
    {
        key,
        input,
        value,
        length,
        offsetLength,
        text
    }

    internal enum ExceptionReason
    {
        String_Empty,
        SmallCapacity,
        InvalidOffsetLength,
        DuplicateKey,
    }
}
