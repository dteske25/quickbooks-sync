﻿using QbSync.QbXml.Extensions;
using QbSync.QbXml.Type;
using System.Xml;

namespace QbSync.QbXml.Messages.Requests
{
    public class DataExtAddRequest : DataExtRequest
    {
        public DataExtAddRequest()
            : base("DataExtAddRq")
        {
        }

        public StrType DataExtValue { get; set; }

        protected override void BuildRequest(XmlElement parent)
        {
            var doc = parent.OwnerDocument;
            var dataExtAdd = doc.CreateElement("DataExtAdd");
            parent.AppendChild(dataExtAdd);
            base.BuildRequest(dataExtAdd);

            // Quickbooks, are you serious? this HAS TO BE at the end.
            dataExtAdd.AppendTag("DataExtValue", DataExtValue);
        }
    }
}