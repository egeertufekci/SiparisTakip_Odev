using System.Data;
using System.Data.SqlClient;
using System;
using System.Xml;

namespace SiparisTakip_Odev.Data
{
    public static class Db
    {
        private static string ConnectionString
        {
            get
            {
                // Try to get System.Configuration.ConfigurationManager via reflection to avoid needing project reference
                try
                {
                    var t = Type.GetType("System.Configuration.ConfigurationManager, System.Configuration");
                    if (t != null)
                    {
                        var csProperty = t.GetProperty("ConnectionStrings");
                        var cs = csProperty.GetValue(null, null);
                        var itemProp = cs.GetType().GetProperty("Item", new Type[] { typeof(string) });
                        var entry = itemProp.GetValue(cs, new object[] { "Baglanti" });
                        if (entry != null)
                        {
                            var connProp = entry.GetType().GetProperty("ConnectionString");
                            var conn = connProp.GetValue(entry, null) as string;
                            if (!string.IsNullOrEmpty(conn)) return conn;
                        }
                    }
                }
                catch { }

                // Fallback: try to read App.config directly
                try
                {
                    var doc = new XmlDocument();
                    doc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                    var node = doc.SelectSingleNode("//connectionStrings/add[@name='Baglanti']");
                    if (node?.Attributes != null)
                    {
                        var att = node.Attributes["connectionString"];
                        if (att != null) return att.Value;
                    }
                }
                catch { }

                // Last resort: default local DB
                return "Server=localhost;Database=SiparisTakip_OdevDB;Trusted_Connection=True;TrustServerCertificate=True;";
            }
        }

        public static DataTable GetTable(string sql, params SqlParameter[] parameters)
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                da.Fill(dt);
            }
            return dt;
        }

        public static int Execute(string sql, params SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        // Helper to check whether a column exists in a table
        public static bool ColumnExists(string tableName, string columnName)
        {
            var dt = GetTable("SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=@t AND COLUMN_NAME=@c",
                new SqlParameter("@t", tableName),
                new SqlParameter("@c", columnName));
            return dt.Rows.Count > 0;
        }
    }
}
