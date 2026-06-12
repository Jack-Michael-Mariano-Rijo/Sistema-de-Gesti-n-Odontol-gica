using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Capa_datos;

namespace Proyecto_final
{
    public partial class FrmTratamientosMasRealizados : Form
    {
        public FrmTratamientosMasRealizados()
        {
            InitializeComponent();

            this.Load += FrmTratamientosMasRealizados_Load;

            reportViewer1.ZoomMode = ZoomMode.PageWidth;

            this.WindowState = FormWindowState.Maximized;

            this.StartPosition = FormStartPosition.CenterScreen;

            reportViewer1.Dock = DockStyle.Fill;
        }

        private void FrmTratamientosMasRealizados_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            using (SqlConnection cn = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand(
                    "sp_DoctoresMasConsultas", cn);

                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(dt);
            }

            reportViewer1.LocalReport.DataSources.Clear();

            ReportDataSource rds =
                new ReportDataSource("DataSet1", dt);

            reportViewer1.LocalReport.DataSources.Add(rds);

            reportViewer1.LocalReport.ReportPath =
                @"C:\Users\roger\OneDrive\Desktop\Roger Hamer saviñon Natera\C#\Proyecto final\Proyecto final\Report3.rdlc";

            ReportParameter parametro =
                new ReportParameter(
                    "FechaReporte",
                    DateTime.Now.ToString("dd/MM/yyyy"));

            reportViewer1.LocalReport.SetParameters(parametro);

            reportViewer1.RefreshReport();
        }
    }
}