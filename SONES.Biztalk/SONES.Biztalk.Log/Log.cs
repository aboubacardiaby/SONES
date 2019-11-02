using Microsoft.XLANGs.BaseTypes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SONES.Biztalk.Log
{
    [Serializable]
    public class Log
    {
        public void Logs(XLANGMessage message)
        {

            var xdoc = (XmlDocument)message[0].RetrieveAs(typeof(XmlDocument));
            var names = new XmlNamespaceManager(xdoc.NameTable);
            string correlationId = null;
            var connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionInfo"]);
            var sql = "[Usp_InsertLog]";

            var command = new SqlCommand(sql, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@CorrelationId", SqlDbType.VarChar)).Value = correlationId;
            connection.Open();
            int ID = command.ExecuteNonQuery();
            connection.Close();
        }
    }
}
