using ActiveSense.Tempsense.model.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveSense.Tempsense.model.Modelo
{
    public class Medida
    {
        [Key]
        [DisplayName("Medida")]
        public int MedidaID { get; set; }

        public decimal Valor { get; set; }
        //public int Humedad { get; set; }

        public DateTime? FechaHora { get; set; }

        [DisplayName("Dispositivo")]
        public int DispositivoID { get; set; }
        public virtual Dispositivos Dispositivo { get; set; }
        
        public const int CANTIDAD_DISPOSITIVOS = 10;

        private const string PERFIL_ADMINISTRADOR = "Administrador";

        private const int FILTRO_DIAS = 1440;

        public string getIdDispositivo(string idUser)
        {
            string chainConexion = ConfigurationManager.ConnectionStrings["TempsenseConnection"].ConnectionString;
            SqlDataReader reader;
            using (SqlConnection sqlConnection1 = new SqlConnection(chainConexion))
            {
                using (SqlCommand cmdTotal = new SqlCommand())
                {
                    sqlConnection1.Open();
                    cmdTotal.CommandType = CommandType.Text;
                    cmdTotal.Connection = sqlConnection1;
                    cmdTotal.CommandText = " SELECT * FROM AspNetUsers Where Id=" + idUser;

                    try
                    {
                        reader = cmdTotal.ExecuteReader();
                        while (reader.Read())
                        {
                            var user = (int)reader[0];
                            Debug.WriteLine(user);
                        }
                    }
                    catch (Exception ex) { }

                }

            }

            return "";
        }

        /*
         * Función que permite listas las medidas para una tabla paginada o gráfica
         * dados unos filtros como fechas, identificador de dispositivos
         **/
        public List<Medida> Listar(int pageIndex, int pageSize, out int pageCount, 
            int dispositivo, string fechaInicio, string fechaFin, string idUser = "", 
            string perfil = "", int  filtroTiempo = 0)
        {
            string chainConexion = ConfigurationManager.ConnectionStrings["TempsenseConnection"].ConnectionString;
            using (SqlConnection sqlConnection1 = new SqlConnection(chainConexion))
            {
                SqlDataReader reader;
                List<Medida> orders = new List<Medida>();

                string whereTotal = "";
                string consultaFiltroTotal = "";
                int consultaTotalMedidasEncontradas = 0;

                if (fechaInicio != "" && fechaFin != "")
                {
                    whereTotal = " FechaHora BETWEEN ('" + fechaInicio + " 00:00" + "') AND ('" + fechaFin + " 23:59" + "') ";
                }

                if (dispositivo != 0)
                {
                    whereTotal = whereTotal != "" ? whereTotal + " AND DispositivoID = " + dispositivo : " DispositivoID = " + dispositivo;
                }
                else
                {
                    if ( perfil != PERFIL_ADMINISTRADOR && idUser!="") {
                         string idDispositivos = UserHelper.obtenerDispositivoAsociados(idUser);
                        idDispositivos = idDispositivos != "" ? idDispositivos : "0";
                         whereTotal = whereTotal != "" ? whereTotal + " AND DispositivoID IN (" + idDispositivos + ") ": " DispositivoID IN (" + idDispositivos + ") ";
                    }

                }

                whereTotal = whereTotal != "" ? "WHERE " + whereTotal : "";

                string sqlCountMedidas = " SELECT COUNT(1) FROM Medidas " + whereTotal;

                using (SqlCommand cmdTotal = new SqlCommand())
                {
                    // consulta total items encontrado
                    try
                    {
                        sqlConnection1.Open();
                        cmdTotal.CommandType = CommandType.Text;
                        cmdTotal.Connection = sqlConnection1;
                        cmdTotal.CommandText = sqlCountMedidas;

                        reader = cmdTotal.ExecuteReader();

                        while (reader.Read())
                        {
                            consultaTotalMedidasEncontradas = (int)reader[0];
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("ERROR Medidas.cs Func Listar: ");
                        Debug.WriteLine("ERROR EN EL SISTEMA : " + ex.GetBaseException());
                    }
                    finally
                    {
                        sqlConnection1.Close();
                    }

                }

                string paginacion = "  WHERE consecutivo BETWEEN(" + pageIndex + ") and(" + (pageIndex + pageSize) + ")";
                consultaFiltroTotal = "SELECT * FROM(SELECT ROW_NUMBER() OVER (ORDER BY MedidaID DESC) consecutivo, * from Medidas " + whereTotal + ") Medidas " + paginacion + " ORDER BY DispositivoID ASC, FechaHora DESC ";
             
                using (SqlCommand cmd = new SqlCommand())
                {

                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = sqlConnection1;
                        cmd.CommandText = consultaFiltroTotal;
                        sqlConnection1.Open();
                        reader = cmd.ExecuteReader();
                        Medida medida = null;
                        while (reader.Read())
                        {
                            medida = new Medida();
                            medida.MedidaID = (int)reader["MedidaID"];
                            medida.Valor = (decimal)reader["Valor"];

                            if (reader["FechaHora"] != DBNull.Value) medida.FechaHora = (DateTime)reader["FechaHora"];
                            if (reader["DispositivoID"] != DBNull.Value) medida.DispositivoID = (int)reader["DispositivoID"];
                            orders.Add(medida);
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("ERROR Medidas.cs Func Listar: ");
                        Debug.WriteLine("ERROR EN EL SISTEMA : " + ex.GetBaseException());
                    }
                    finally
                    {
                        sqlConnection1.Close();
                    }

                }

                pageCount = consultaTotalMedidasEncontradas;

                return orders;
            }
        }

        /*
         * Función que permite listar promedios de medidas dados filtros como fechas, identificador de dispositivos
         **/
        public List<Medida> ListarPromedios(int pageIndex, int pageSize, out int pageCount,
            int dispositivo, string fechaInicio, string fechaFin, string idUser = "",
            string perfil = "", int filtroTiempo = 0)
        {
            string chainConexion = ConfigurationManager.ConnectionStrings["TempsenseConnection"].ConnectionString;
            using (SqlConnection sqlConnection1 = new SqlConnection(chainConexion))
            {
                SqlDataReader reader;
                List<Medida> listaMedidas = new List<Medida>();

                string fechaInicioSt = "";
                string fechaFinSt = "";

                if (fechaInicio != "" && fechaFin != "")
                {
                    fechaInicioSt = fechaInicio + " 00:00";
                    fechaFinSt = fechaFin + " 23:59";
                }
                else {
                    var fechaActual = DateTime.Now;
                    var hor = fechaActual.Hour;
                    var min = fechaActual.Minute;

                    var fechaAyer = fechaActual.Date.AddDays(-1).AddHours(hor).AddMinutes(min);

                    fechaFinSt = String.Format("{0:yyyy-MM-dd HH:mm:ss}", fechaActual);
                    fechaInicioSt = String.Format("{0:yyyy-MM-dd HH:mm:ss}", fechaAyer);
                }

                string whereTotal = " FechaHora BETWEEN ('" + fechaInicioSt + "') AND ('" + fechaFinSt + "') ";

                if (dispositivo != 0)
                {
                    whereTotal = whereTotal != "" ? whereTotal + " AND DispositivoID = " + dispositivo : " DispositivoID = " + dispositivo;
                }
                else
                {
                    if (perfil != PERFIL_ADMINISTRADOR && idUser != "")
                    {
                        string idDispositivos = UserHelper.obtenerDispositivoAsociados(idUser);
                        idDispositivos = idDispositivos != "" ? idDispositivos : "0";
                        whereTotal = whereTotal != "" ? whereTotal + " AND DispositivoID IN (" + idDispositivos + ") " : " DispositivoID IN (" + idDispositivos + ") ";
                    }

                }

                whereTotal = whereTotal != "" ? "WHERE " + whereTotal : "";

                string querySearch = " Select(DATEPART(MINUTE, FechaHora) / " + filtroTiempo + ") as minuto, " +
                                     " DATEPART(hh, FechaHora) as hora, "+
                                     " DATEPART(DAY, FechaHora) as dia, ";

                string queryGroupSearch = " (DATEPART(MINUTE, FechaHora) / " + filtroTiempo + "), " +
                                          " DATEPART(hh, FechaHora), " ;

                string postPagination = " ORDER BY dia, hora, mes ASC";

                string row_number = " SELECT(ROW_NUMBER() OVER(ORDER BY T.minuto ASC, T.mes ASC)) as consecutivo, *";

                if ( filtroTiempo == FILTRO_DIAS) {
                    postPagination = " ORDER BY dia, mes ASC";
                    querySearch = "Select (DATEPART(DAY, FechaHora)) as dia, ";
                    queryGroupSearch = "";
                    row_number = " SELECT(ROW_NUMBER() OVER(ORDER BY T.dia ASC)) as consecutivo, *";
                }

                string  sqlCountMedidas = "SELECT COUNT(Promedios.promedio) FROM ( "+
                      querySearch +
                    " AVG(Valor) as promedio " +
                    " FROM Medidas " + whereTotal +
                    " Group by " +
                    queryGroupSearch +
                    " DATEPART(DAY, FechaHora) " +
                    " ) AS Promedios";

                int consultaTotalMedidasEncontradas = 0;

                using (SqlCommand cmdTotal = new SqlCommand())
                {

                    // consulta total items encontrado
                    try
                    {
                        sqlConnection1.Open();
                        cmdTotal.CommandType = CommandType.Text;
                        cmdTotal.Connection = sqlConnection1;
                        cmdTotal.CommandText = sqlCountMedidas;

                        reader = cmdTotal.ExecuteReader();

                        while (reader.Read())
                        {
                            consultaTotalMedidasEncontradas = (int)reader[0];
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("ERROR Medidas.cs func ListarPromedios : ");
                        Debug.WriteLine("ERROR EN EL SISTEMA : " + ex.GetBaseException());
                    }
                    finally
                    {
                        sqlConnection1.Close();
                    }

                }

                string paginacion = "  WHERE consecutivo BETWEEN(" + pageIndex + ") and(" + (pageIndex + pageSize) + ") "+ postPagination;

                string consultaFiltroTotal = " SELECT * FROM( " +
                                        row_number +
                                        " FROM( "+
                                          querySearch + 
                                        " DATEPART(MONTH, FechaHora) mes ," +
                                        " DATEPART(YEAR, FechaHora) Years, " +
                                        " AVG(Valor) as valor " +
                                        " FROM Medidas " + whereTotal +
                                        " Group by "+
                                          queryGroupSearch +
                                        " DATEPART(DAY, FechaHora), "+
                                        " DATEPART(MONTH, FechaHora)," +
                                        " DATEPART(YEAR, FechaHora) " +
                                       
                                        " )t )q " + paginacion;

                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = sqlConnection1;
                        cmd.CommandText = consultaFiltroTotal;
                        sqlConnection1.Open();
                        reader = cmd.ExecuteReader();
                        Medida medida = null;
                        string fechaD = "";
                        while (reader.Read())
                        {
                            medida = new Medida();
                            medida.Valor = (decimal)reader["valor"];
                            string hora = "00";
                            if (filtroTiempo != FILTRO_DIAS)
                            {
                                if (reader["hora"] != DBNull.Value)
                                {
                                    hora = reader["hora"].ToString();
                                }
                            }

                            fechaD = reader["dia"].ToString() + "/" + reader["mes"].ToString() + "/" + reader["Years"].ToString() + " " + hora + ":00:00";/* "01/08/2008 14:50:50.42" */
                            medida.FechaHora = Convert.ToDateTime(fechaD);
                            medida.DispositivoID = dispositivo;
                            listaMedidas.Add(medida);
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("ERROR Medidas.cs Func ListarPromedios: ");
                        Debug.WriteLine("ERROR EN EL SISTEMA : " + ex.GetBaseException());
                    }
                    finally
                    {
                        sqlConnection1.Close();
                    }

                }

                pageCount = consultaTotalMedidasEncontradas;

                return listaMedidas;
            }
        }

    }
}
