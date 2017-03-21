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

namespace ActiveSense.Tempsense.model.Model
{
    public class Measure
    {
        [Key]
        [DisplayName("Measure")]
        public int MeasureID { get; set; }

        public decimal Value { get; set; }

        public DateTime DateTime { get; set; }

        [DisplayName("Device")]
        public int DeviceID { get; set; }
        public virtual devices Device { get; set; }
        
        public const int NUMBER_DEVICES = 10;

        private const string ADMINISTRATOR_PROFILE = "Administrador";

        private const int FILTER_DAYS = 1440;

        public string getIdDevice(string idUser)
        {
            string ChainConnection = ConfigurationManager.ConnectionStrings["TempsenseConnection"].ConnectionString;
            SqlDataReader reader;
            using (SqlConnection sqlConnection1 = new SqlConnection(ChainConnection))
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
* Function allows lists the Measures for a paginated or graphical table
* given a few filters such as dates, caller ID devices
         **/
        public List<Measure> List(int pageIndex, int pageSize, out int pageCount, 
            int device, string Datehome, string Dateend, string idUser = "", 
            string perfil = "", int  FilterTime = 0)
        {
            string ChainConnection = ConfigurationManager.ConnectionStrings["TempsenseConnection"].ConnectionString;
            using (SqlConnection sqlConnection1 = new SqlConnection(ChainConnection))
            {
                SqlDataReader reader;
                List<Measure> orders = new List<Measure>();

                string whereTotal = "";
                string consultaFiltroTotal = "";
                int SeeTotalMeasuresFound = 0;

                if (Datehome != "" && Dateend != "")
                {
                    whereTotal = " DateTime BETWEEN ('" + Datehome + " 00:00" + "') AND ('" + Dateend + " 23:59" + "') ";
                }

                if (device != 0)
                {
                    whereTotal = whereTotal != "" ? whereTotal + " AND DeviceID = " + Device : " DeviceID = " + device;
                }
                else
                {
                    if ( perfil != ADMINISTRATOR_PROFILE && idUser!="") {
                         string idDevice = UserHelper.GetAssociatedDevice(idUser);
                        idDevice = idDevice != "" ? idDevice : "0";
                         whereTotal = whereTotal != "" ? whereTotal + " AND DeviceID IN (" + idDevice + ") ": " DeviceID IN (" + idDevice + ") ";
                    }

                }

                whereTotal = whereTotal != "" ? "WHERE " + whereTotal : "";

                string sqlCountMeasures = " SELECT COUNT(1) FROM Measures " + whereTotal;

                using (SqlCommand cmdTotal = new SqlCommand())
                {
                    // consulta total items encontrado
                    try
                    {
                        sqlConnection1.Open();
                        cmdTotal.CommandType = CommandType.Text;
                        cmdTotal.Connection = sqlConnection1;
                        cmdTotal.CommandText = sqlCountMeasures;

                        reader = cmdTotal.ExecuteReader();

                        while (reader.Read())
                        {
                            SeeTotalMeasuresFound = (int)reader[0];
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("ERROR Measures.cs Func List: ");
                        Debug.WriteLine("ERROR IN THE SYSTEM : " + ex.GetBaseException());
                    }
                    finally
                    {
                        sqlConnection1.Close();
                    }

                }

                string paginacion = "  WHERE consecutive BETWEEN(" + pageIndex + ") and(" + (pageIndex + pageSize) + ")";
                consultaFiltroTotal = "SELECT * FROM(SELECT ROW_NUMBER() OVER (ORDER BY MeasureID DESC) consecutive, * from Measures " + whereTotal + ") Measures " + paginacion + " ORDER BY DeviceID ASC, DateTime DESC ";
             
                using (SqlCommand cmd = new SqlCommand())
                {

                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = sqlConnection1;
                        cmd.CommandText = consultaFiltroTotal;
                        sqlConnection1.Open();
                        reader = cmd.ExecuteReader();
                        Measure Measure = null;
                        while (reader.Read())
                        {
                            Measure = new Measure();
                            Measure.MeasureID = (int)reader["MeasureID"];
                            Measure.Value = (decimal)reader["Value"];

                            if (reader["DateTime"] != DBNull.Value) Measure.DateTime = (DateTime)reader["DateTime"];
                            if (reader["DeviceID"] != DBNull.Value) Measure.DeviceID = (int)reader["DeviceID"];
                            orders.Add(Measure);
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("ERROR Measures.cs Func List: ");
                        Debug.WriteLine("ERROR IN THE SYSTEM : " + ex.GetBaseException());
                    }
                    finally
                    {
                        sqlConnection1.Close();
                    }

                }

                pageCount = SeeTotalMeasuresFound;

                return orders;
            }
        }

        /*
* Function that lets you list averages of Measures given filters such as dates, caller ID devices
         **/
        public List<Measure> ListAverages(int pageIndex, int pageSize, out int pageCount,
            int device, string Datehome, string Dateend, string idUser = "",
            string perfil = "", int FilterTime = 0)
        {
            string ChainConnection = ConfigurationManager.ConnectionStrings["TempsenseConnection"].ConnectionString;
            using (SqlConnection sqlConnection1 = new SqlConnection(ChainConnection))
            {
                SqlDataReader reader;
                List<Measure> listMeasures = new List<Measure>();

                string DatehoMontht = "";
                string DateendSt = "";

                if (Datehome != "" && Dateend != "")
                {
                    DatehoMontht = Datehome + " 00:00";
                    DateendSt = Dateend + " 23:59";
                }
                else {
                    var Currentdate = DateTime.Now;
                    var hor = Currentdate.Hour;
                    var min = Currentdate.Minute;

                    var YesterdayDate = Currentdate.Date.AddDays(-1).AddHours(hor).AddMinutes(min);

                    DateendSt = String.Format("{0:yyyy-MM-dd HH:mm:ss}", Currentdate);
                    DatehoMontht = String.Format("{0:yyyy-MM-dd HH:mm:ss}", YesterdayDate);
                }

                string whereTotal = " DateTime BETWEEN ('" + DatehoMontht + "') AND ('" + DateendSt + "') ";

                if (device != 0)
                {
                    whereTotal = whereTotal != "" ? whereTotal + " AND DeviceID = " + Device : " DeviceID = " + DeviceID;
                }
                else
                {
                    if (perfil != ADMINISTRATOR_PROFILE && idUser != "")
                    {
                        string idDevice = UserHelper.GetAssociatedDevice(idUser);
                        idDevice = idDevice != "" ? idDevice : "0";
                        whereTotal = whereTotal != "" ? whereTotal + " AND DeviceID IN (" + idDevice + ") " : " DeviceID IN (" + idDevice + ") ";
                    }

                }

                whereTotal = whereTotal != "" ? "WHERE " + whereTotal : "";

                string querySearch = " Select(DATEPART(MINUTE, DateTime) / " + FilterTime + ") as minute, " +
                                     " DATEPART(hh, DateTime) as time, "+
                                     " DATEPART(DAY, DateTime) as day, ";

                string queryGroupSearch = " (DATEPART(MINUTE, DateTime) / " + FilterTime + "), " +
                                          " DATEPART(hh, DateTime), " ;

                string postPagination = " ORDER BY day, time, month ASC";

                string row_number = " SELECT(ROW_NUMBER() OVER(ORDER BY T.minute ASC, T.Month ASC)) as consecutive, *";

                if ( FilterTime == FILTER_DAYS) {
                    postPagination = " ORDER BY day, month ASC";
                    querySearch = "Select (DATEPART(DAY, DateTime)) as day, ";
                    queryGroupSearch = "";
                    row_number = " SELECT(ROW_NUMBER() OVER(ORDER BY T.day ASC)) as consecutive, *";
                }

                string  sqlCountMeasures = "SELECT COUNT(Average.averages) FROM ( " +
                      querySearch +
                    " AVG(Value) as average " +
                    " FROM Measures " + whereTotal +
                    " Group by " +
                    queryGroupSearch +
                    " DATEPART(DAY, DateTime) " +
                    " ) AS averages";

                int SeeTotalMeasuresFound = 0;

                using (SqlCommand cmdTotal = new SqlCommand())
                {

                    // consulta total items encontrado
                    try
                    {
                        sqlConnection1.Open();
                        cmdTotal.CommandType = CommandType.Text;
                        cmdTotal.Connection = sqlConnection1;
                        cmdTotal.CommandText = sqlCountMeasures;

                        reader = cmdTotal.ExecuteReader();

                        while (reader.Read())
                        {
                            SeeTotalMeasuresFound = (int)reader[0];
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("ERROR Measures.cs func ListAverages : ");
                        Debug.WriteLine("ERROR IN SYSTEM : " + ex.GetBaseException());
                    }
                    finally
                    {
                        sqlConnection1.Close();
                    }

                }

                string paginacion = "  WHERE consecutive BETWEEN(" + pageIndex + ") and(" + (pageIndex + pageSize) + ") "+ postPagination;

                string consultaFiltroTotal = " SELECT * FROM( " +
                                        row_number +
                                        " FROM( "+
                                          querySearch + 
                                        " DATEPART(MONTH, DateTime) months ," +
                                        " DATEPART(YEAR, DateTime) Years, " +
                                        " AVG(Value) as Value " +
                                        " FROM Measures " + whereTotal +
                                        " Group by "+
                                          queryGroupSearch +
                                        " DATEPART(DAY, DateTime), "+
                                        " DATEPART(MONTH, DateTime)," +
                                        " DATEPART(YEAR, DateTime) " +
                                       
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
                        Measure Measure = null;
                        string dateD = "";
                        while (reader.Read())
                        {
                            Measure = new Measure();
                            Measure.Value = (decimal)reader["Value"];
                            string time = "00";
                            if (FilterTime != FILTER_DAYS)
                            {
                                if (reader["time"] != DBNull.Value)
                                {
                                    time = reader["time"].ToString();
                                }
                            }

                            dateD = reader["day"].ToString() + "/" + reader["Month"].ToString() + "/" + reader["Years"].ToString() + " " + time + ":00:00";/* "01/08/2008 14:50:50.42" */
                            Measure.DateTime = Convert.ToDateTime(dateD);
                            Measure.DeviceID = device;
                            listMeasures.Add(Measure);
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("ERROR Measures.cs Func ListAverages: ");
                        Debug.WriteLine("ERROR IN THE SYSTEM : " + ex.GetBaseException());
                    }
                    finally
                    {
                        sqlConnection1.Close();
                    }

                }

                pageCount = SeeTotalMeasuresFound;

                return listMeasures;
            }
        }

    }
}
