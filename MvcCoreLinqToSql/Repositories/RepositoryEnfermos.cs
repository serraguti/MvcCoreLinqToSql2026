using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using MvcCoreLinqToSql.Models;
using System.Data;

namespace MvcCoreLinqToSql.Repositories
{
    public class RepositoryEnfermos
    {
        private DataTable tablaEnfermos;
        private SqlConnection cn;
        private SqlCommand com;

        public RepositoryEnfermos()
        {
            string connectionString = @"Data Source=LOCALHOST\DEVELOPER;Initial Catalog=HOSPITAL;Persist Security Info=True;User ID=SA;Encrypt=True;Trust Server Certificate=True";
            this.cn = new SqlConnection(connectionString);
            this.com = new SqlCommand();
            this.com.Connection = this.cn;
            string sql = "select * from ENFERMO";
            SqlDataAdapter ad =
                new SqlDataAdapter(sql, this.cn);
            this.tablaEnfermos = new DataTable();
            ad.Fill(this.tablaEnfermos);
        }

        public List<Enfermo> GetEnfermos()
        {
            var consulta = from datos in
                               this.tablaEnfermos.AsEnumerable()
                           select datos;
            List<Enfermo> enfermos = new List<Enfermo>();
            foreach (var row in consulta)
            {
                Enfermo enfermo = new Enfermo
                {
                    Inscripcion = row.Field<string>("INSCRIPCION"),
                    Apellido = row.Field<string>("APELLIDO"),
                    Direccion = row.Field<string>("DIRECCION"),
                    FechaNacimiento = row.Field<DateTime>("FECHA_NAC"),
                    Genero = row.Field<string>("S"),
                    Nss = row.Field<string>("NSS")
                };
                enfermos.Add(enfermo);
            }
            return enfermos;
        }

        public Enfermo FindEnfermo(string inscripcion)
        {
            var consulta = from datos in
                               this.tablaEnfermos.AsEnumerable()
                           where datos.Field<string>("INSCRIPCION")
                           == inscripcion
                           select datos;
            //COMPROBAMOS SI TENEMOS DATOS
            if (consulta.Count() == 0)
            {
                return null;
            }
            else
            {
                var row = consulta.First();
                Enfermo enfermo = new Enfermo
                {
                    Inscripcion = row.Field<string>("INSCRIPCION"),
                    Apellido = row.Field<string>("APELLIDO"),
                    Direccion = row.Field<string>("DIRECCION"),
                    FechaNacimiento = row.Field<DateTime>("FECHA_NAC"),
                    Genero = row.Field<string>("S"),
                    Nss = row.Field<string>("NSS")
                };
                return enfermo;
            }
        }

        //CONSULTAS DE ACCION CON ADO NET
        public void DeleteEnfermo(string inscripcion)
        {
            string sql = 
                "delete from ENFERMO where INSCRIPCION=@inscripcion";
            this.com.Parameters.AddWithValue("@inscripcion", inscripcion);
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;
            this.cn.Open();
            this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }
    }
}
