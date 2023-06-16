﻿using System;
using System.Reflection;

namespace JsonSubTypes
{
    //  MIT License
    //  
    //  Copyright (c) 2017 Emmanuel Counasse
    //  
    //  Permission is hereby granted, free of charge, to any person obtaining a copy
    //  of this software and associated documentation files (the "Software"), to deal
    //  in the Software without restriction, including without limitation the rights
    //  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    //  copies of the Software, and to permit persons to whom the Software is
    //  furnished to do so, subject to the following conditions:
    //  
    //  The above copyright notice and this permission notice shall be included in all
    //  copies or substantial portions of the Software.
    //  
    //  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    //  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    //  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    //  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    //  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    //  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    //  SOFTWARE.
    internal class JsonSubtypesConverter : JsonSubtypes
    {
        private readonly Type _baseType;
        private readonly Type _fallbackType;

        public JsonSubtypesConverter(Type baseType, Type fallbackType) : base()
        {
            _baseType = baseType;
            _fallbackType = fallbackType;
        }

        public JsonSubtypesConverter(Type baseType, string jsonDiscriminatorPropertyName, Type fallbackType) : base(jsonDiscriminatorPropertyName)
        {
            _baseType = baseType;
            _fallbackType = fallbackType;
        }

        internal override Type GetFallbackSubType(Type type)
        {
            return _fallbackType;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == _baseType || ToTypeInfo(_baseType).IsAssignableFrom(ToTypeInfo(objectType));
        }
    }
}
