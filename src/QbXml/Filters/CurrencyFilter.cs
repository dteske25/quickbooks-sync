﻿using QbSync.QbXml.Extensions;
using QbSync.QbXml.Type;
using System;
using System.Collections.Generic;
using System.Xml;

namespace QbSync.QbXml.Filters
{
    public class CurrencyFilter : IXmlConvertible
    {
        public IEnumerable<IdType> ListId
        {
            get;
            set;
        }

        public IEnumerable<StrType> FullName
        {
            get;
            set;
        }

        public virtual void AppendXml(XmlElement parent)
        {
            CheckFilters();

            if (ListId != null)
            {
                parent.AppendTags("ListId", ListId);
            }

            if (FullName != null)
            {
                parent.AppendTags("FullName", FullName);
            }
        }

        private void CheckFilters()
        {
            if (ListId != null && FullName != null)
            {
                throw new ArgumentException("You cannot set ListId or FullName at the same time.");
            }
        }
    }
}
