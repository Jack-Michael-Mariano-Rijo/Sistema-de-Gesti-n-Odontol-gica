using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Capa_datos;
using Capa_negocio;

namespace Proyecto_final
{
    public partial class Facturacion : Form
    {
        Panel sidebar;
        Panel topbar;
        Panel dashboard;

        string nombreUsuario;
        string rolUsuario;
        string rolNombre;  // Normalizado

        FacturasDAL facturasDAL = new FacturasDAL();

        DataGridView tablaFacturas;

        Label lblTotalFacturado;
        Label lblPendienteCobro;
        Label lblPagadoHoy;
        Label lblFacturasPendientes;

        public Facturacion(string usuario, string rol)
        {
            InitializeComponent();

            nombreUsuario = usuario;
            rolUsuario = rol;
            ConvertirRol();

            ConfigurarFormulario();
            CrearSidebar();
            CrearTopbar();
            CrearDashboard();

            CargarFacturas();

            // Solo Administrador carga estadísticas
            if (rolNombre == "Administrador")
            {
                CargarEstadisticas();
            }

            this.Resize += Facturacion_Resize;
        }

        // ======================================================
        // CONVERTIR ROL
        // ======================================================
        void ConvertirRol()
        {
            switch (rolUsuario.ToLower())
            {
                case "admin":
                case "administrador":
                case "1":
                    rolNombre = "Administrador";
                    break;
                case "doctor":
                case "2":
                    rolNombre = "Doctor";
                    break;
                case "secretaria":
                case "recepcionista":
                case "3":
                    rolNombre = "Recepcionista";
                    break;
                default:
                    rolNombre = "Recepcionista";
                    break;
            }
        }

        // ======================================================
        // VERIFICAR PERMISOS
        // ======================================================
        bool TienePermiso(string modulo, string accion = null)
        {
            // ADMINISTRADOR: todo
            if (rolNombre == "Administrador") return true;

            // DOCTOR - Puede facturar pero NO ver estadísticas
            if (rolNombre == "Doctor")
            {
                if (modulo == "Facturacion")
                {
                    return true;
                }
                if (modulo == "Citas") return true;
                if (modulo == "Pacientes") return true;
                if (modulo == "Reportes") return false;
                return true;
            }

            // RECEPCIONISTA - NO ve facturación
            if (rolNombre == "Recepcionista")
            {
                return false;
            }

            return false;
        }

        private void Facturacion_Resize(object sender, EventArgs e)
        {
            AjustarDashboard();
            CargarFacturas();
            if (rolNombre == "Administrador")
            {
                CargarEstadisticas();
            }
        }

        void ConfigurarFormulario()
        {
            this.Text = "Facturación";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(240, 244, 250);
            this.DoubleBuffered = true;
            this.Font = new Font("Segoe UI", 10);
        }

        // ======================================================
        // SIDEBAR
        // ======================================================

        void CrearSidebar()
        {
            sidebar = new Panel();
            sidebar.Width = 260;
            sidebar.Dock = DockStyle.Left;
            sidebar.BackColor = Color.FromArgb(0, 44, 120);
            this.Controls.Add(sidebar);

            PictureBox logoImg = new PictureBox();
            logoImg.Size = new Size(180, 180);
            logoImg.Location = new Point(40, 10);
            logoImg.SizeMode = PictureBoxSizeMode.Zoom;
            try
            {
                logoImg.Image = Image.FromFile(@"C:\Users\roger\Downloads\Proyecto final fotos\logo.png");
            }
            catch { }
            sidebar.Controls.Add(logoImg);

            Label logo = new Label();
            logo.Text = "Centro Odontológico\nDoctora Norabia";
            logo.ForeColor = Color.White;
            logo.Font = new Font("Segoe UI Semibold", 15, FontStyle.Bold);
            logo.Size = new Size(240, 70);
            logo.Location = new Point(10, 190);
            logo.TextAlign = ContentAlignment.MiddleCenter;
            sidebar.Controls.Add(logo);

            int topPosition = 300;

            CrearBotonMenu("Página Principal", topPosition, false);
            topPosition += 70;

            if (rolNombre == "Administrador" || rolNombre == "Doctor")
            {
                CrearBotonMenu("Pacientes", topPosition, false);
                topPosition += 70;

                CrearBotonMenu("Citas", topPosition, false);
                topPosition += 70;
            }

            CrearBotonMenu("Facturación", topPosition, true);
            topPosition += 70;

            if (rolNombre == "Administrador")
            {
                CrearBotonMenu("Reportes", topPosition, false);
                topPosition += 70;
            }

            Button salir = new Button();
            salir.Text = "Salir";
            salir.Size = new Size(200, 50);
            salir.Location = new Point(30, sidebar.Height - 80);
            salir.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            salir.FlatStyle = FlatStyle.Flat;
            salir.FlatAppearance.BorderSize = 0;
            salir.BackColor = Color.FromArgb(220, 53, 69);
            salir.ForeColor = Color.White;
            salir.Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold);
            salir.Cursor = Cursors.Hand;
            salir.Click += (s, e) => { Application.Exit(); };
            RedondearControl(salir, 15);
            sidebar.Controls.Add(salir);
        }

        void CrearBotonMenu(string texto, int top, bool activo)
        {
            Button btn = new Button();
            btn.Text = texto;
            btn.Size = new Size(200, 52);
            btn.Location = new Point(30, top);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(20, 0, 0, 0);

            if (activo)
            {
                btn.BackColor = Color.White;
                btn.ForeColor = Color.FromArgb(0, 44, 120);
            }
            else
            {
                btn.BackColor = Color.FromArgb(0, 44, 120);
                btn.ForeColor = Color.White;
            }

            btn.MouseEnter += (s, e) =>
            {
                if (!activo)
                {
                    btn.BackColor = Color.FromArgb(0, 92, 230);
                }
            };

            btn.MouseLeave += (s, e) =>
            {
                if (!activo)
                {
                    btn.BackColor = Color.FromArgb(0, 44, 120);
                }
            };

            btn.Click += (s, e) =>
            {
                if (texto == "Página Principal")
                {
                    Form1 frm = new Form1(nombreUsuario, rolUsuario);
                    frm.Show();
                    this.Hide();
                }
                else if (texto == "Pacientes")
                {
                    Pacientes frm = new Pacientes(nombreUsuario, rolUsuario);
                    frm.Show();
                    this.Hide();
                }
                else if (texto == "Facturación")
                {
                    // Ya estás aquí
                }
                else if (texto == "Citas")
                {
                    Citas frm = new Citas(nombreUsuario, rolUsuario);
                    frm.Show();
                    this.Hide();
                }
                else if (texto == "Reportes")
                {
                    Reportes frm = new Reportes(nombreUsuario, rolUsuario);
                    frm.ShowDialog();
                }
            };

            RedondearControl(btn, 15);
            sidebar.Controls.Add(btn);
        }

        // ======================================================
        // TOPBAR
        // ======================================================

        void CrearTopbar()
        {
            topbar = new Panel();
            topbar.Height = 80;
            topbar.Dock = DockStyle.Top;
            topbar.BackColor = Color.White;
            this.Controls.Add(topbar);
            topbar.BringToFront();

            Label titulo = new Label();
            titulo.Text = "Centro Odontológico Doctora Norabia";
            titulo.Font = new Font("Segoe UI Semibold", 20, FontStyle.Bold);
            titulo.ForeColor = Color.FromArgb(15, 15, 40);
            titulo.AutoSize = true;
            topbar.Controls.Add(titulo);

            PictureBox iconoUser = new PictureBox();
            iconoUser.Size = new Size(58, 58);
            iconoUser.Location = new Point(15, 10);
            iconoUser.SizeMode = PictureBoxSizeMode.Zoom;
            try
            {
                iconoUser.Image = Image.FromFile(@"C:\Users\roger\Downloads\Proyecto final fotos\medico.png");
            }
            catch { }
            topbar.Controls.Add(iconoUser);

            Label nombre = new Label();
            nombre.Text = nombreUsuario;
            nombre.Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold);
            nombre.AutoSize = true;
            nombre.Location = new Point(72, 15);
            topbar.Controls.Add(nombre);

            Label rol = new Label();
            rol.Text = rolNombre;
            rol.Font = new Font("Segoe UI", 9);
            rol.ForeColor = Color.Gray;
            rol.AutoSize = true;
            rol.Location = new Point(72, 38);
            topbar.Controls.Add(rol);

            Label fecha = new Label();
            fecha.Text = DateTime.Now.ToString("dddd dd MMMM yyyy   hh:mm tt");
            fecha.Font = new Font("Segoe UI", 10);
            fecha.ForeColor = Color.Gray;
            fecha.AutoSize = true;
            topbar.Controls.Add(fecha);

            topbar.Resize += (s, e) =>
            {
                titulo.Location = new Point((topbar.Width / 2) - (titulo.Width / 2), 22);
                fecha.Location = new Point(topbar.Width - fecha.Width - 30, 30);
            };
        }

        // ======================================================
        // DASHBOARD
        // ======================================================

        void CrearDashboard()
        {
            dashboard = new Panel();
            dashboard.Dock = DockStyle.Fill;
            dashboard.BackColor = Color.FromArgb(240, 244, 250);
            this.Controls.Add(dashboard);
            dashboard.BringToFront();
            AjustarDashboard();
        }

        void AjustarDashboard()
        {
            dashboard.Controls.Clear();

            int startX = 40;
            int startY = 20;

            Label titulo = new Label();
            titulo.Text = "Facturación";
            titulo.Font = new Font("Segoe UI Semibold", 28, FontStyle.Bold);
            titulo.ForeColor = Color.FromArgb(15, 15, 40);
            titulo.AutoSize = true;
            titulo.Location = new Point(startX, startY);
            dashboard.Controls.Add(titulo);

            Label subtitulo = new Label();
            subtitulo.Text = "Gestión de facturas y pagos";
            subtitulo.Font = new Font("Segoe UI", 14);
            subtitulo.ForeColor = Color.Gray;
            subtitulo.AutoSize = true;
            subtitulo.Location = new Point(startX + 5, startY + 45);
            dashboard.Controls.Add(subtitulo);

            int cardY = 110;
            int currentX = startX;

            // CARDS ESTADÍSTICOS - SOLO PARA ADMINISTRADOR
            if (rolNombre == "Administrador")
            {
                CrearCard("Total Facturado", "$0", currentX, cardY, "Este mes", out lblTotalFacturado);
                currentX += 220;

                CrearCard("Pendiente de Cobro", "$0", currentX, cardY, "Este mes", out lblPendienteCobro);
                currentX += 220;

                CrearCard("Total Pagado Hoy", "$0", currentX, cardY, "Hoy", out lblPagadoHoy);
                currentX += 220;

                CrearCard("Facturas Pendientes", "0", currentX, cardY, "Pendientes", out lblFacturasPendientes);
            }
            else
            {
                // Para Doctor: no crear cards
                lblTotalFacturado = null;
                lblPendienteCobro = null;
                lblPagadoHoy = null;
                lblFacturasPendientes = null;
            }

            // TABLA FACTURAS
            int tablaY = (rolNombre == "Administrador") ? cardY + 150 : 110;
            CrearTablaFacturas(startX, tablaY);

            // PANEL ACCIONES - Mantener como estaba (Nueva Factura y Registrar Pago)
            CrearPanelAcciones(startX + 740, tablaY);
        }

        // ======================================================
        // CARDS
        // ======================================================

        void CrearCard(string tituloTexto, string valor, int x, int y, string subtituloTexto, out Label valorLabel)
        {
            Panel card = new Panel();
            card.Size = new Size(200, 135);
            card.Location = new Point(x, y);
            card.BackColor = Color.White;
            RedondearControl(card, 20);
            dashboard.Controls.Add(card);

            Label titulo = new Label();
            titulo.Text = tituloTexto;
            titulo.Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold);
            titulo.ForeColor = Color.FromArgb(50, 50, 50);
            titulo.Location = new Point(20, 18);
            titulo.AutoSize = true;
            card.Controls.Add(titulo);

            valorLabel = new Label();
            valorLabel.Text = valor;
            valorLabel.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            valorLabel.ForeColor = Color.FromArgb(10, 10, 40);
            valorLabel.Location = new Point(20, 50);
            valorLabel.AutoSize = true;
            card.Controls.Add(valorLabel);

            Label subtitulo = new Label();
            subtitulo.Text = subtituloTexto;
            subtitulo.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            subtitulo.ForeColor = Color.FromArgb(0, 92, 230);
            subtitulo.Location = new Point(20, 95);
            subtitulo.AutoSize = true;
            card.Controls.Add(subtitulo);
        }

        // ======================================================
        // TABLA FACTURAS
        // ======================================================

        void CrearTablaFacturas(int x, int y)
        {
            Panel panel = new Panel();
            panel.Size = new Size(700, 400);
            panel.Location = new Point(x, y);
            panel.BackColor = Color.White;
            RedondearControl(panel, 20);
            dashboard.Controls.Add(panel);

            Label titulo = new Label();
            titulo.Text = "Listado de Facturas";
            titulo.Font = new Font("Segoe UI Semibold", 18, FontStyle.Bold);
            titulo.Location = new Point(20, 15);
            titulo.AutoSize = true;
            panel.Controls.Add(titulo);

            tablaFacturas = new DataGridView();
            tablaFacturas.Size = new Size(660, 310);
            tablaFacturas.Location = new Point(20, 55);
            tablaFacturas.BackgroundColor = Color.White;
            tablaFacturas.BorderStyle = BorderStyle.None;
            tablaFacturas.RowHeadersVisible = false;
            tablaFacturas.AllowUserToAddRows = false;
            tablaFacturas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tablaFacturas.EnableHeadersVisualStyles = false;
            tablaFacturas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tablaFacturas.MultiSelect = false;
            tablaFacturas.ReadOnly = true;
            tablaFacturas.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 255);
            tablaFacturas.DefaultCellStyle.SelectionForeColor = Color.FromArgb(0, 44, 120);
            tablaFacturas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 92, 230);
            tablaFacturas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            tablaFacturas.ColumnHeadersHeight = 45;

            tablaFacturas.ColumnCount = 7;
            tablaFacturas.Columns[0].Name = "Factura #";
            tablaFacturas.Columns[1].Name = "Paciente";
            tablaFacturas.Columns[2].Name = "Fecha";
            tablaFacturas.Columns[3].Name = "Total";
            tablaFacturas.Columns[4].Name = "Monto Pagado";
            tablaFacturas.Columns[5].Name = "Balance";
            tablaFacturas.Columns[6].Name = "Estado";

            tablaFacturas.CellFormatting += (s, e) =>
            {
                if (tablaFacturas.Columns[e.ColumnIndex].Name == "Estado" && e.Value != null)
                {
                    string estado = e.Value.ToString();
                    if (estado == "Pagada")
                    {
                        e.CellStyle.ForeColor = Color.FromArgb(40, 167, 69);
                    }
                    else if (estado == "Pendiente")
                    {
                        e.CellStyle.ForeColor = Color.FromArgb(220, 53, 69);
                    }
                }
            };

            panel.Controls.Add(tablaFacturas);
        }

        // ======================================================
        // PANEL ACCIONES - IGUAL QUE ANTES
        // ======================================================

        void CrearPanelAcciones(int x, int y)
        {
            Panel panel = new Panel();
            panel.Size = new Size(220, 250);
            panel.Location = new Point(x, y);
            panel.BackColor = Color.White;
            RedondearControl(panel, 20);
            dashboard.Controls.Add(panel);

            Label titulo = new Label();
            titulo.Text = "Accesos Rápidos";
            titulo.Font = new Font("Segoe UI Semibold", 14, FontStyle.Bold);
            titulo.Location = new Point(20, 20);
            titulo.AutoSize = true;
            panel.Controls.Add(titulo);

            // Botón Nueva Factura
            Button btnNuevaFactura = new Button();
            btnNuevaFactura.Text = "Nueva Factura";
            btnNuevaFactura.Size = new Size(180, 45);
            btnNuevaFactura.Location = new Point(20, 70);
            btnNuevaFactura.FlatStyle = FlatStyle.Flat;
            btnNuevaFactura.FlatAppearance.BorderSize = 0;
            btnNuevaFactura.BackColor = Color.FromArgb(0, 92, 230);
            btnNuevaFactura.ForeColor = Color.White;
            btnNuevaFactura.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
            RedondearControl(btnNuevaFactura, 12);
            panel.Controls.Add(btnNuevaFactura);

            // Botón Registrar Pago
            Button btnRegistrarPago = new Button();
            btnRegistrarPago.Text = "Registrar Pago";
            btnRegistrarPago.Size = new Size(180, 45);
            btnRegistrarPago.Location = new Point(20, 130);
            btnRegistrarPago.FlatStyle = FlatStyle.Flat;
            btnRegistrarPago.FlatAppearance.BorderSize = 0;
            btnRegistrarPago.BackColor = Color.FromArgb(40, 167, 69);
            btnRegistrarPago.ForeColor = Color.White;
            btnRegistrarPago.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
            RedondearControl(btnRegistrarPago, 12);
            panel.Controls.Add(btnRegistrarPago);

            // EVENTOS CLICK
            btnNuevaFactura.Click += (s, e) =>
            {
                MiniFactura frmMiniFactura = new MiniFactura();
                frmMiniFactura.ShowDialog();

                CargarFacturas();
                if (rolNombre == "Administrador")
                {
                    CargarEstadisticas();
                }
            };

            btnRegistrarPago.Click += (s, e) =>
            {
                if (tablaFacturas.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Seleccione una factura.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult resultado = MessageBox.Show("¿Deseas registrar este pago?",
                    "Confirmar Pago", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    int facturaID = Convert.ToInt32(tablaFacturas.SelectedRows[0].Cells[0].Value);
                    string totalTexto = tablaFacturas.SelectedRows[0].Cells[3].Value.ToString().Replace("$", "");
                    decimal monto = Convert.ToDecimal(totalTexto);

                    bool pago = facturasDAL.RegistrarPago(facturaID, monto, "Efectivo");

                    if (pago)
                    {
                        MessageBox.Show("Pago registrado correctamente.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        CargarFacturas();
                        if (rolNombre == "Administrador")
                        {
                            CargarEstadisticas();
                        }
                    }
                }
            };
        }

        // ======================================================
        // CARGAR FACTURAS
        // ======================================================

        void CargarFacturas()
        {
            if (tablaFacturas == null)
                return;

            tablaFacturas.Rows.Clear();

            DataTable datos = facturasDAL.MostrarFacturas();

            foreach (DataRow fila in datos.Rows)
            {
                string estado = fila["Estado"].ToString();
                string total = fila["Total"].ToString();
                string pagado = estado == "Pagada" ? total : "0";
                string balance = estado == "Pagada" ? "0" : total;

                tablaFacturas.Rows.Add(
                    fila["FacturaID"],
                    fila["Paciente"],
                    Convert.ToDateTime(fila["FechaFactura"]).ToString("dd/MM/yyyy"),
                    "$" + total,
                    "$" + pagado,
                    "$" + balance,
                    estado);
            }
        }

        // ======================================================
        // CARGAR ESTADISTICAS - SOLO ADMINISTRADOR
        // ======================================================

        void CargarEstadisticas()
        {
            if (lblTotalFacturado == null) return;

            DataTable datos = facturasDAL.ObtenerEstadisticas();

            if (datos.Rows.Count > 0)
            {
                DataRow fila = datos.Rows[0];

                lblTotalFacturado.Text = "$" + fila["TotalFacturado"].ToString();
                lblPendienteCobro.Text = "$" + fila["PendienteCobro"].ToString();
                lblPagadoHoy.Text = "$" + fila["TotalPagadoHoy"].ToString();
                lblFacturasPendientes.Text = fila["FacturasPendientes"].ToString();
            }
        }

        // ======================================================
        // REDONDEAR
        // ======================================================

        void RedondearControl(Control control, int radio)
        {
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(new Rectangle(0, 0, radio, radio), 180, 90);
            path.AddArc(new Rectangle(control.Width - radio, 0, radio, radio), 270, 90);
            path.AddArc(new Rectangle(control.Width - radio, control.Height - radio, radio, radio), 0, 90);
            path.AddArc(new Rectangle(0, control.Height - radio, radio, radio), 90, 90);
            path.CloseFigure();
            control.Region = new Region(path);
        }
    }
}
