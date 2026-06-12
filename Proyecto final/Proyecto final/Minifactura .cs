using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Capa_datos;
using Proyecto_final;

namespace Proyecto_final
{
    public partial class MiniFactura : Form
    {
        ComboBox cbPacientes;
        ComboBox cbTratamientos;
        ComboBox cbMetodoPago;
        ComboBox cbEstado;
        TextBox txtMonto;
        Label lblTotal;

        FacturasDAL facturasDAL = new FacturasDAL();

        public MiniFactura()
        {
            InitializeComponent();

            ConfigurarFormulario();

            CrearDiseño();

            CargarPacientes();

            CargarTratamientos();

            if (cbTratamientos.Items.Count > 0)
            {
                cbTratamientos.SelectedIndex = 0;
            }
        }

        void ConfigurarFormulario()
        {
            this.Text = "Nueva Factura";
            this.Size = new Size(500, 760);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);
            this.DoubleBuffered = true;
            this.AutoScroll = true;
        }

        void CrearDiseño()
        {
            Panel top = new Panel();
            top.Dock = DockStyle.Top;
            top.Height = 60;
            top.BackColor = Color.FromArgb(0, 92, 230);
            this.Controls.Add(top);

            Label titulo = new Label();
            titulo.Text = "Nueva Factura";
            titulo.ForeColor = Color.White;
            titulo.Font = new Font("Segoe UI Semibold", 16, FontStyle.Bold);
            titulo.AutoSize = true;
            titulo.Location = new Point(20, 18);
            top.Controls.Add(titulo);

            Button cerrar = new Button();
            cerrar.Text = "✕";
            cerrar.Size = new Size(35, 35);
            cerrar.Location = new Point(445, 12);
            cerrar.FlatStyle = FlatStyle.Flat;
            cerrar.FlatAppearance.BorderSize = 0;
            cerrar.BackColor = Color.Transparent;
            cerrar.ForeColor = Color.White;
            cerrar.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            cerrar.Cursor = Cursors.Hand;
            cerrar.Click += (s, e) => this.Close();
            top.Controls.Add(cerrar);

            int y = 90;
            int margen = 25;
            int anchoControl = 420;

            CrearLabel("Paciente", margen, y);

            cbPacientes = new ComboBox();
            cbPacientes.Size = new Size(anchoControl, 35);
            cbPacientes.Location = new Point(margen, y + 25);
            cbPacientes.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Controls.Add(cbPacientes);

            y += 80;

            CrearLabel("Tratamiento", margen, y);

            cbTratamientos = new ComboBox();
            cbTratamientos.Size = new Size(anchoControl, 35);
            cbTratamientos.Location = new Point(margen, y + 25);
            cbTratamientos.DropDownStyle = ComboBoxStyle.DropDownList;
            cbTratamientos.SelectedIndexChanged += CbTratamientos_SelectedIndexChanged;
            this.Controls.Add(cbTratamientos);

            y += 80;

            CrearLabel("Monto", margen, y);

            txtMonto = new TextBox();
            txtMonto.Size = new Size(anchoControl, 35);
            txtMonto.Location = new Point(margen, y + 25);
            txtMonto.ReadOnly = true;
            txtMonto.BackColor = Color.FromArgb(240, 240, 240);

            txtMonto.TextChanged += (s, e) =>
            {
                if (decimal.TryParse(txtMonto.Text, out decimal monto))
                {
                    lblTotal.Text = "$ " + monto.ToString("N0");
                }
                else
                {
                    lblTotal.Text = "$ 0";
                }
            };

            this.Controls.Add(txtMonto);

            y += 80;

            CrearLabel("Método de Pago", margen, y);

            cbMetodoPago = new ComboBox();
            cbMetodoPago.Size = new Size(anchoControl, 35);
            cbMetodoPago.Location = new Point(margen, y + 25);
            cbMetodoPago.DropDownStyle = ComboBoxStyle.DropDownList;

            cbMetodoPago.Items.Add("Efectivo");
            cbMetodoPago.Items.Add("Transferencia");
            cbMetodoPago.Items.Add("Tarjeta");

            cbMetodoPago.SelectedIndex = 0;

            this.Controls.Add(cbMetodoPago);

            y += 80;

            CrearLabel("Estado", margen, y);

            cbEstado = new ComboBox();
            cbEstado.Size = new Size(anchoControl, 35);
            cbEstado.Location = new Point(margen, y + 25);
            cbEstado.DropDownStyle = ComboBoxStyle.DropDownList;

            cbEstado.Items.Add("Pagada");
            cbEstado.Items.Add("Pendiente");
            cbEstado.Items.Add("Abonada");

            cbEstado.SelectedIndex = 1;

            cbEstado.SelectedIndexChanged += (s, e) =>
            {
                if (cbEstado.Text == "Abonada")
                {
                    txtMonto.ReadOnly = false;
                    txtMonto.BackColor = Color.White;
                    txtMonto.Text = "";
                }
                else
                {
                    txtMonto.ReadOnly = true;
                    txtMonto.BackColor = Color.FromArgb(240, 240, 240);

                    if (cbTratamientos.SelectedValue != null &&
                        cbTratamientos.SelectedValue is int)
                    {
                        try
                        {
                            int tratamientoID = (int)cbTratamientos.SelectedValue;
                            decimal precio = facturasDAL.ObtenerPrecioTratamiento(tratamientoID);
                            txtMonto.Text = precio.ToString();
                        }
                        catch
                        {
                            txtMonto.Text = "";
                        }
                    }
                }
            };

            this.Controls.Add(cbEstado);

            y += 90;

            Panel totalPanel = new Panel();
            totalPanel.Size = new Size(anchoControl, 80);
            totalPanel.Location = new Point(margen, y);
            totalPanel.BackColor = Color.FromArgb(240, 244, 250);
            this.Controls.Add(totalPanel);

            Label txt = new Label();
            txt.Text = "TOTAL";
            txt.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            txt.Location = new Point(15, 12);
            txt.AutoSize = true;
            totalPanel.Controls.Add(txt);

            lblTotal = new Label();
            lblTotal.Text = "$ 0";
            lblTotal.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblTotal.ForeColor = Color.FromArgb(0, 92, 230);
            lblTotal.Location = new Point(15, 35);
            lblTotal.AutoSize = true;
            totalPanel.Controls.Add(lblTotal);

            y += 110;

            Button btnGuardar = new Button();
            btnGuardar.Text = "Guardar Factura";
            btnGuardar.Size = new Size(200, 45);
            btnGuardar.Location = new Point(margen, y);
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.BackColor = Color.FromArgb(0, 92, 230);
            btnGuardar.ForeColor = Color.White;
            btnGuardar.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
            btnGuardar.Cursor = Cursors.Hand;
            btnGuardar.Click += BtnGuardar_Click;
            this.Controls.Add(btnGuardar);

            Button btnCancelar = new Button();
            btnCancelar.Text = "Cancelar";
            btnCancelar.Size = new Size(200, 45);
            btnCancelar.Location = new Point(margen + 220, y);
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.BackColor = Color.FromArgb(200, 200, 200);
            btnCancelar.ForeColor = Color.Black;
            btnCancelar.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
            btnCancelar.Cursor = Cursors.Hand;
            btnCancelar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancelar);
        }

        void CargarPacientes()
        {
            DataTable dt = facturasDAL.ObtenerPacientes();

            cbPacientes.DataSource = dt;
            cbPacientes.DisplayMember = "NombreCompleto";
            cbPacientes.ValueMember = "PacienteID";
        }

        void CargarTratamientos()
        {
            DataTable dt = facturasDAL.ObtenerTratamientos();

            cbTratamientos.DataSource = dt;
            cbTratamientos.DisplayMember = "NombreTratamiento";
            cbTratamientos.ValueMember = "TratamientoID";
        }

        private void CbTratamientos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTratamientos.SelectedValue == null)
                return;

            if (cbEstado.Text == "Abonada")
                return;

            if (!(cbTratamientos.SelectedValue is int))
                return;

            try
            {
                int tratamientoID = (int)cbTratamientos.SelectedValue;
                decimal precio = facturasDAL.ObtenerPrecioTratamiento(tratamientoID);
                txtMonto.Text = precio.ToString();
            }
            catch
            {
                txtMonto.Text = "";
            }
        }

        // ✅ BOTÓN GUARDAR MEJORADO - ABRE EL REPORTE
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            // Validación 1: Paciente
            if (cbPacientes.SelectedValue == null)
            {
                MessageBox.Show(
                    "Seleccione un paciente",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Validación 2: Tratamiento
            if (cbTratamientos.SelectedValue == null)
            {
                MessageBox.Show(
                    "Seleccione un tratamiento",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Validación 3: Monto
            decimal monto;
            if (!decimal.TryParse(txtMonto.Text, out monto))
            {
                MessageBox.Show(
                    "Monto inválido",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Crear factura
            int facturaID = facturasDAL.CrearFacturaCompleta(
                Convert.ToInt32(cbPacientes.SelectedValue),
                Convert.ToInt32(cbTratamientos.SelectedValue),
                monto,
                cbMetodoPago.Text,
                cbEstado.Text);

            // Verificar resultado
            if (facturaID > 0)
            {
                MessageBox.Show(
                    "Factura guardada correctamente",
                    "Éxito",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Abrir reporte en pantalla completa
                FrmReporteFactura frm = new FrmReporteFactura(facturaID);
                frm.ShowDialog();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(
                    "Error al guardar factura",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        void CrearLabel(string texto, int x, int y)
        {
            Label lbl = new Label();

            lbl.Text = texto;
            lbl.Font = new Font("Segoe UI Semibold", 9, FontStyle.Bold);
            lbl.Location = new Point(x, y);
            lbl.AutoSize = true;

            this.Controls.Add(lbl);
        }
    }
}