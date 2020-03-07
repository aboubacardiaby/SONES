using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Sones.Library
{
    [Serializable]

    public class SalesOrderSplitter

    {

        private int _totalSplits = -1;

        private XDocument _message = null;

        public void Initialize(XmlDocument message, int totalSplits)

        {

            _totalSplits = totalSplits;

            _message = XDocument.Parse(message.OuterXml);



        }

        public XmlDocument GetMessage(int currentSplit)

        {



            var childNodes = _message.Descendants("Line");

            int cnt = childNodes.Count();

            var take = cnt / 2;

            var c1 = childNodes

                            .Skip(currentSplit)

                            .Take(take);

            var xdoc = XElement.Parse(_message.ToString());

            XElement lineschild = xdoc.Element("Lines");

            lineschild.ReplaceWith(

                new XElement("Lines", c1));

            var xd = new XmlDocument();

            xd.LoadXml(xdoc.ToString());



            return xd;

        }
    }
}
