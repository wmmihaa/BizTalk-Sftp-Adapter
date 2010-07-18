using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Blogical.Shared.Adapters.Common
{
    public class XmlSerializerSectionHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="configContext">configContext</param>
        /// <param name="section">section</param>
        /// <returns>object</returns>
        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            return section;
        }
    }
}
