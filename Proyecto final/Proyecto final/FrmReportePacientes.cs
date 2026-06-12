using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using Capa_datos;

namespace Proyecto_final
{
    public partial class FrmReportePacientes : Form
    {
        public FrmReportePacientes()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void FrmReportePacientes_Load(object sender, EventArgs e)
        {
            try
            {
                reportViewer1.Dock = DockStyle.Fill;
                reportViewer1.ZoomMode = ZoomMode.PageWidth;

                string rutaReporte = Application.StartupPath + "\\Report1.rdlc";

                if (!File.Exists(rutaReporte))
                {
                    rutaReporte = Application.StartupPath + "\\..\\..\\Report1.rdlc";
                    rutaReporte = Path.GetFullPath(rutaReporte);
                }

                if (!File.Exists(rutaReporte))
                {
                    MessageBox.Show("No se encontró el archivo Report1.rdlc", "Error");
                    return;
                }

                reportViewer1.LocalReport.ReportPath = rutaReporte;

                SqlConnection cn = Conexion.ObtenerConexion();
                SqlDataAdapter da = new SqlDataAdapter("sp_ReportePacientesPorMes", cn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);

                int cantidadMaxima = 0;
                int totalPacientes = 0;

                foreach (DataRow row in dt.Rows)
                {
                    int cantidad = Convert.ToInt32(row["CantidadPacientes"]);
                    totalPacientes += cantidad;

                    if (cantidad > cantidadMaxima)
                    {
                        cantidadMaxima = cantidad;
                    }
                }

                int promedioEntero = dt.Rows.Count > 0 ? totalPacientes / dt.Rows.Count : 0;

                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetPacientesMes", dt));

                ReportParameter[] parametros = new ReportParameter[]
                {
                    new ReportParameter("CantidadMaxima", cantidadMaxima.ToString()),
                    new ReportParameter("PromedioPorMes", promedioEntero.ToString()),
                    new ReportParameter("FechaReporte", DateTime.Now.ToString("dd/MM/yyyy"))
                };

                reportViewer1.LocalReport.SetParameters(parametros);

                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}