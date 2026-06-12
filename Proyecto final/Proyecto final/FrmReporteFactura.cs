using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using Capa_datos;

namespace Proyecto_final
{
    public partial class FrmReporteFactura : Form
    {
        int facturaID;

        public FrmReporteFactura(int id)
        {
            InitializeComponent();
            facturaID = id;
            this.Load += FrmReporteFactura_Load;
            reportViewer1.ZoomMode = ZoomMode.PageWidth;
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            reportViewer1.Dock = DockStyle.Fill;
        }

        private void FrmReporteFactura_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection cn = Conexion.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("sp_ReporteFacturacion", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.Parameters.AddWithValue("@FacturaID", facturaID);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show($"No hay datos para factura {facturaID}");
                    this.Close();
                    return;
                }

                string rutaReporte = @"C:\Users\roger\OneDrive\Desktop\Roger Hamer saviñon Natera\C#\Proyecto final\Proyecto final\Report4.rdlc";
                string nombreDataSet = ObtenerNombreDataSet(rutaReporte);

                if (nombreDataSet == null)
                {
                    nombreDataSet = "DataSet1"; 
                }

                
                reportViewer1.LocalReport.DataSources.Clear();
                ReportDataSource rds = new ReportDataSource(nombreDataSet, dt);
                reportViewer1.LocalReport.DataSources.Add(rds);

                reportViewer1.LocalReport.ReportPath = rutaReporte;
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

       
        private string ObtenerNombreDataSet(string rutaRdlc)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(rutaRdlc);

                XmlNodeList dataSetNodes = doc.GetElementsByTagName("DataSetName");
                if (dataSetNodes.Count > 0)
                {
                    string nombre = dataSetNodes[0].InnerText;
                    return nombre;
                }

                XmlNodeList dataSourceNodes = doc.GetElementsByTagName("Name");
                foreach (XmlNode node in dataSourceNodes)
                {
                    if (node.ParentNode.Name == "DataSet")
                    {
                        return node.InnerText;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}