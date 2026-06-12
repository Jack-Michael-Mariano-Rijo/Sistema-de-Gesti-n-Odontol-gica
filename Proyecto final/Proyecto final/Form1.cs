using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Capa_negocio;
using Capa_datos;

namespace Proyecto_final
{
    public partial class Form1 : Form
    {
        Panel sidebar;
        Panel topbar;
        Panel dashboard;

        string nombreUsuario;
        string rolUsuario;      // Viene como "Admin", "Doctor", "Secretaria"
        string rolNombre;       // Normalizado a "Administrador", "Doctor", "Recepcionista"

        CitasService citasService = new CitasService();
        PacientesService pacientesService = new PacientesService();
        FacturasDAL facturasDAL = new FacturasDAL();

        Label lblCitasDia;
        Label lblPacientesRegistrados;
        Label lblFacturasPendientes;
        Label lblIngresosHoy;

        DataGridView tablaCitasHoy;

        public Form1(string usuario, string rol)
        {
            InitializeComponent();

            nombreUsuario = usuario;
            rolUsuario = rol;

            // ======================================================
            // CONVERTIR el rol
            // ======================================================
            switch (rol.ToLower())
            {
                case "admin":
                case "administrador":
                    rolNombre = "Administrador";
                    break;
                case "doctor":
                    rolNombre = "Doctor";
                    break;
                case "secretaria":
                case "recepcionista":
                    rolNombre = "Recepcionista";
                    break;
                default:
                    if (rol == "1") rolNombre = "Administrador";
                    else if (rol == "2") rolNombre = "Doctor";
                    else if (rol == "3") rolNombre = "Recepcionista";
                    else rolNombre = "Recepcionista";
                    break;
            }

            ConfigurarFormulario();
            CrearSidebar();
            CrearTopbar();
            CrearDashboard();

            CargarDatosDashboard();

            this.Resize += Form1_Resize;
        }

        // ======================================================
        // MÉTODO PARA VERIFICAR PERMISOS
        // ======================================================
        bool TienePermiso(string modulo, string accion = null)
        {
            // ADMINISTRADOR: tiene todos los permisos
            if (rolNombre == "Administrador") return true;

            // DOCTOR - Puede facturar
            if (rolNombre == "Doctor")
            {
                // Dashboard - solo Ver Citas y Pacientes
                if (modulo == "Dashboard")
                {
                    if (accion == "FacturasPendientes" || accion == "IngresosHoy")
                        return false;
                    return true;
                }

                // Pacientes - SI
                if (modulo == "Pacientes") return true;

                // Citas - SI
                if (modulo == "Citas") return true;

                // Facturación - SI (Doctor puede facturar)
                if (modulo == "Facturacion") return true;

                // Reportes - NO
                if (modulo == "Reportes") return false;

                // Nuevos Doctores - NO
                if (modulo == "NuevosDoctores") return false;

                // Horarios - SI
                if (modulo == "Horarios") return true;

                // Inicio - SI
                if (modulo == "Inicio") return true;

                return true;
            }

            // RECEPCIONISTA - NO ve Facturación
            if (rolNombre == "Recepcionista")
            {
                // Dashboard - solo Ver Citas y Pacientes
                if (modulo == "Dashboard")
                {
                    if (accion == "FacturasPendientes" || accion == "IngresosHoy")
                        return false;
                    return true;
                }

                // Pacientes - SI
                if (modulo == "Pacientes") return true;

                // Citas - SI
                if (modulo == "Citas") return true;

                // Facturación - NO (Recepcionista NO ve facturación)
                if (modulo == "Facturacion") return false;

                // Reportes - NO
                if (modulo == "Reportes") return false;

                // Nuevos Doctores - NO
                if (modulo == "NuevosDoctores") return false;

                // Horarios - SI
                if (modulo == "Horarios") return true;

                // Inicio - SI
                if (modulo == "Inicio") return true;

                return true;
            }

            return false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            AjustarDashboard();
            CargarDatosDashboard();
        }

        void ConfigurarFormulario()
        {
            this.Text = "Centro Odontológico";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(240, 244, 250);
            this.DoubleBuffered = true;
            this.Font = new Font("Segoe UI", 10);
        }

        void CrearSidebar()
        {
            sidebar = new Panel();
            sidebar.Width = 260;
            sidebar.Dock = DockStyle.Left;
            sidebar.BackColor = Color.FromArgb(0, 44, 120);
            this.Controls.Add(sidebar);

            PictureBox logoImg = new PictureBox();
            logoImg.Size = new Size(190, 190);
            logoImg.Location = new Point((sidebar.Width - logoImg.Width) / 2, 10);
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
            logo.Location = new Point(10, 195);
            logo.TextAlign = ContentAlignment.MiddleCenter;
            sidebar.Controls.Add(logo);

            int topPosition = 300;

            // Botón Página Principal (siempre visible)
            CrearBotonMenu("Página Principal", topPosition, true);
            topPosition += 70;

            // Botón Pacientes (según permiso)
            if (TienePermiso("Pacientes"))
            {
                CrearBotonMenu("Pacientes", topPosition, false);
                topPosition += 70;
            }

            // Botón Citas (según permiso)
            if (TienePermiso("Citas"))
            {
                CrearBotonMenu("Citas", topPosition, false);
                topPosition += 70;
            }

            // Botón Facturación (solo Administrador y Doctor - Recepcionista NO)
            if (TienePermiso("Facturacion"))
            {
                CrearBotonMenu("Facturación", topPosition, false);
                topPosition += 70;
            }

            // Botón Reportes (solo Administrador)
            if (TienePermiso("Reportes"))
            {
                CrearBotonMenu("Reportes", topPosition, false);
                topPosition += 70;
            }

            // Botón Salir (siempre visible)
            Button salir = new Button();
            salir.Text = "Salir";
            salir.Size = new Size(200, 50);
            salir.Location = new Point(30, sidebar.Height - 70);
            salir.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            salir.FlatStyle = FlatStyle.Flat;
            salir.FlatAppearance.BorderSize = 0;
            salir.BackColor = Color.FromArgb(220, 53, 69);
            salir.ForeColor = Color.White;
            salir.Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold);
            salir.Cursor = Cursors.Hand;
            RedondearControl(salir, 15);
            salir.Click += Salir_Click;
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
                    btn.BackColor = Color.FromArgb(0, 92, 230);
            };

            btn.MouseLeave += (s, e) =>
            {
                if (!activo)
                    btn.BackColor = Color.FromArgb(0, 44, 120);
            };

            btn.Click += (s, e) =>
            {
                if (texto == "Página Principal")
                {
                    CargarDatosDashboard();
                }
                else if (texto == "Facturación")
                {
                    Facturacion frm = new Facturacion(nombreUsuario, rolUsuario);
                    frm.Show();
                    this.Hide();
                }
                else if (texto == "Pacientes")
                {
                    Pacientes frm = new Pacientes(nombreUsuario, rolUsuario);
                    frm.Show();
                    this.Hide();
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

            titulo.Location = new Point((topbar.Width / 2) - (titulo.Width / 2), 22);
            fecha.Location = new Point(topbar.Width - fecha.Width - 30, 30);
        }

        void CrearDashboard()
        {
            dashboard = new Panel();
            dashboard.Dock = DockStyle.Fill;
            dashboard.AutoScroll = true;
            dashboard.BackColor = Color.FromArgb(240, 244, 250);
            this.Controls.Add(dashboard);
            dashboard.BringToFront();
            AjustarDashboard();
        }

        void AjustarDashboard()
        {
            dashboard.Controls.Clear();

            int margen = 40;
            int startX = margen;
            int startY = 30;

            Label titulo = new Label();
            titulo.Text = "Página Principal";
            titulo.Font = new Font("Segoe UI Semibold", 28, FontStyle.Bold);
            titulo.ForeColor = Color.FromArgb(15, 15, 40);
            titulo.AutoSize = true;
            titulo.Location = new Point(startX, startY);
            dashboard.Controls.Add(titulo);

            Label subtitulo = new Label();
            subtitulo.Text = "Panel de Control";
            subtitulo.Font = new Font("Segoe UI", 14);
            subtitulo.ForeColor = Color.Gray;
            subtitulo.AutoSize = true;
            subtitulo.Location = new Point(startX + 5, startY + 50);
            dashboard.Controls.Add(subtitulo);

            int cardY = startY + 100;
            int cardWidth = 220;
            int cardHeight = 140;
            int spacing = 20;

            int currentX = startX;

            // Card 1: Citas del Día (Todos)
            Panel cardCitas = CrearCardBase(currentX, cardY);
            cardCitas.BackColor = Color.FromArgb(0, 92, 230);
            Label tituloCitas = new Label();
            tituloCitas.Text = "Citas del Día";
            tituloCitas.ForeColor = Color.White;
            tituloCitas.Font = new Font("Segoe UI Semibold", 13, FontStyle.Bold);
            tituloCitas.Dock = DockStyle.Top;
            tituloCitas.Height = 50;
            tituloCitas.TextAlign = ContentAlignment.MiddleCenter;
            lblCitasDia = new Label();
            lblCitasDia.Text = "0";
            lblCitasDia.ForeColor = Color.White;
            lblCitasDia.Font = new Font("Segoe UI", 34, FontStyle.Bold);
            lblCitasDia.Dock = DockStyle.Fill;
            lblCitasDia.TextAlign = ContentAlignment.MiddleCenter;
            cardCitas.Controls.Add(lblCitasDia);
            cardCitas.Controls.Add(tituloCitas);
            currentX += cardWidth + spacing;

            // Card 2: Pacientes Registrados (Todos)
            CrearCardBlanca("Pacientes Registrados", "0", currentX, cardY, out lblPacientesRegistrados);
            currentX += cardWidth + spacing;

            // Card 3: Facturas Pendientes (Solo Administrador)
            if (TienePermiso("Dashboard", "FacturasPendientes"))
            {
                CrearCardBlanca("Facturas Pendientes", "$0", currentX, cardY, out lblFacturasPendientes);
                currentX += cardWidth + spacing;
            }

            // Card 4: Ingresos Hoy (Solo Administrador)
            if (TienePermiso("Dashboard", "IngresosHoy"))
            {
                CrearCardBlanca("Ingresos Hoy", "$0", currentX, cardY, out lblIngresosHoy);
                currentX += cardWidth + spacing;
            }

            int tablaY = cardY + cardHeight + 40;
            int anchoDisponible = dashboard.Width - 80;
            int tablaWidth = anchoDisponible - 320;

            // Tabla de Citas de Hoy (Todos)
            CrearTablaPanel("Citas de Hoy", startX, tablaY, tablaWidth);

            // Panel de Accesos Rápidos
            CrearPanelDerecha(startX + tablaWidth + 20, tablaY, 280);
        }

        Panel CrearCardBase(int x, int y)
        {
            Panel card = new Panel();
            card.Size = new Size(220, 140);
            card.Location = new Point(x, y);
            card.BackColor = Color.White;
            RedondearControl(card, 20);
            dashboard.Controls.Add(card);
            return card;
        }

        void CrearCardBlanca(string tituloTexto, string valor, int x, int y, out Label valorLabel)
        {
            Panel card = CrearCardBase(x, y);

            Label titulo = new Label();
            titulo.Text = tituloTexto;
            titulo.ForeColor = Color.FromArgb(30, 30, 30);
            titulo.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
            titulo.Dock = DockStyle.Top;
            titulo.Height = 55;
            titulo.TextAlign = ContentAlignment.MiddleCenter;

            valorLabel = new Label();
            valorLabel.Text = valor;
            valorLabel.ForeColor = Color.FromArgb(0, 44, 120);
            valorLabel.Font = new Font("Segoe UI", 28, FontStyle.Bold);
            valorLabel.Dock = DockStyle.Fill;
            valorLabel.TextAlign = ContentAlignment.MiddleCenter;

            card.Controls.Add(valorLabel);
            card.Controls.Add(titulo);
        }

        void CrearTablaPanel(string tituloTexto, int x, int y, int ancho)
        {
            Panel panel = new Panel();
            panel.Size = new Size(ancho, 380);
            panel.Location = new Point(x, y);
            panel.BackColor = Color.White;
            RedondearControl(panel, 20);
            dashboard.Controls.Add(panel);

            Label titulo = new Label();
            titulo.Text = tituloTexto;
            titulo.Font = new Font("Segoe UI Semibold", 18, FontStyle.Bold);
            titulo.Location = new Point(25, 20);
            titulo.AutoSize = true;
            panel.Controls.Add(titulo);

            tablaCitasHoy = new DataGridView();
            tablaCitasHoy.Size = new Size(ancho - 50, 280);
            tablaCitasHoy.Location = new Point(25, 70);
            tablaCitasHoy.BackgroundColor = Color.White;
            tablaCitasHoy.BorderStyle = BorderStyle.None;
            tablaCitasHoy.RowHeadersVisible = false;
            tablaCitasHoy.AllowUserToAddRows = false;
            tablaCitasHoy.AllowUserToResizeRows = false;
            tablaCitasHoy.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tablaCitasHoy.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tablaCitasHoy.EnableHeadersVisualStyles = false;
            tablaCitasHoy.ReadOnly = true;
            tablaCitasHoy.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 92, 230);
            tablaCitasHoy.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            tablaCitasHoy.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
            tablaCitasHoy.ColumnHeadersHeight = 42;
            tablaCitasHoy.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            tablaCitasHoy.RowTemplate.Height = 36;
            tablaCitasHoy.GridColor = Color.FromArgb(230, 230, 230);

            tablaCitasHoy.ColumnCount = 4;
            tablaCitasHoy.Columns[0].Name = "Hora";
            tablaCitasHoy.Columns[1].Name = "Paciente";
            tablaCitasHoy.Columns[2].Name = "Doctor";
            tablaCitasHoy.Columns[3].Name = "Estado";

            panel.Controls.Add(tablaCitasHoy);
        }

        void CrearPanelDerecha(int x, int y, int ancho)
        {
            Panel panel = new Panel();
            panel.Size = new Size(ancho, 380);
            panel.Location = new Point(x, y);
            panel.BackColor = Color.White;
            RedondearControl(panel, 20);
            dashboard.Controls.Add(panel);

            Label titulo = new Label();
            titulo.Text = "Accesos Rápidos";
            titulo.Font = new Font("Segoe UI Semibold", 16, FontStyle.Bold);
            titulo.Location = new Point(25, 20);
            titulo.AutoSize = true;
            panel.Controls.Add(titulo);

            int botonTop = 80;
            int botonHeight = 50;
            int botonSpacing = 15;

            // Botón Horarios (todos lo ven)
            Button btnHorarios = new Button();
            btnHorarios.Text = "Horarios";
            btnHorarios.Size = new Size(ancho - 50, botonHeight);
            btnHorarios.Location = new Point(25, botonTop);
            btnHorarios.FlatStyle = FlatStyle.Flat;
            btnHorarios.FlatAppearance.BorderSize = 0;
            btnHorarios.BackColor = Color.FromArgb(0, 92, 230);
            btnHorarios.ForeColor = Color.White;
            btnHorarios.Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold);
            btnHorarios.Cursor = Cursors.Hand;
            RedondearControl(btnHorarios, 12);
            btnHorarios.Click += (s, e) =>
            {
                Horarios frm = new Horarios(nombreUsuario, rolUsuario);
                frm.Show();
                this.Hide();
            };
            panel.Controls.Add(btnHorarios);
            botonTop += botonHeight + botonSpacing;

            // Botón Inicio / Cerrar Sesión (todos lo ven)
            Button btnInicio = new Button();
            btnInicio.Text = "Cerrar Sesión";
            btnInicio.Size = new Size(ancho - 50, botonHeight);
            btnInicio.Location = new Point(25, botonTop);
            btnInicio.FlatStyle = FlatStyle.Flat;
            btnInicio.FlatAppearance.BorderSize = 0;
            btnInicio.BackColor = Color.FromArgb(220, 53, 69);
            btnInicio.ForeColor = Color.White;
            btnInicio.Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold);
            btnInicio.Cursor = Cursors.Hand;
            RedondearControl(btnInicio, 12);
            btnInicio.Click += (s, e) =>
            {
                if (MessageBox.Show("¿Seguro que deseas cerrar sesión?", "Cerrar Sesión",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    User user = new User();
                    user.Show();
                    this.Close();
                }
            };
            panel.Controls.Add(btnInicio);
            botonTop += botonHeight + botonSpacing;

            // Botón Nuevos Doctores (solo Administrador)
            if (TienePermiso("NuevosDoctores"))
            {
                Button btnDoctores = new Button();
                btnDoctores.Text = "Nuevos Doctores";
                btnDoctores.Size = new Size(ancho - 50, botonHeight);
                btnDoctores.Location = new Point(25, botonTop);
                btnDoctores.FlatStyle = FlatStyle.Flat;
                btnDoctores.FlatAppearance.BorderSize = 0;
                btnDoctores.BackColor = Color.FromArgb(0, 92, 230);
                btnDoctores.ForeColor = Color.White;
                btnDoctores.Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold);
                btnDoctores.Cursor = Cursors.Hand;
                RedondearControl(btnDoctores, 12);
                btnDoctores.Click += (s, e) =>
                {
                    Doctores frm = new Doctores(nombreUsuario, rolUsuario);
                    frm.Show();
                    this.Hide();
                };
                panel.Controls.Add(btnDoctores);
            }
        }

        void CargarDatosDashboard()
        {
            try
            {
                // 1. Cargar Citas del Día
                DataTable dtCitas = citasService.MostrarCitas();
                int citasHoy = 0;

                if (dtCitas != null && dtCitas.Rows.Count > 0 && tablaCitasHoy != null)
                {
                    tablaCitasHoy.Rows.Clear();

                    foreach (DataRow row in dtCitas.Rows)
                    {
                        if (row["FechaCita"] != DBNull.Value)
                        {
                            DateTime fechaCita = Convert.ToDateTime(row["FechaCita"]);
                            if (fechaCita.Date == DateTime.Today)
                            {
                                citasHoy++;
                                tablaCitasHoy.Rows.Add(
                                    row["HoraCita"]?.ToString() ?? "N/A",
                                    row["Paciente"]?.ToString() ?? "N/A",
                                    row["Doctor"]?.ToString() ?? "N/A",
                                    row["Estado"]?.ToString() ?? "N/A"
                                );
                            }
                        }
                    }
                }

                if (lblCitasDia != null)
                    lblCitasDia.Text = citasHoy.ToString();

                // 2. Cargar Pacientes Registrados
                DataTable dtPacientes = pacientesService.Listar();
                int totalPacientes = (dtPacientes != null) ? dtPacientes.Rows.Count : 0;

                if (lblPacientesRegistrados != null)
                    lblPacientesRegistrados.Text = totalPacientes.ToString();

                // 3. Cargar Facturas Pendientes (solo Admin)
                if (lblFacturasPendientes != null)
                {
                    DataTable dtFacturas = facturasDAL.MostrarFacturas();
                    decimal totalPendiente = 0;

                    if (dtFacturas != null && dtFacturas.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtFacturas.Rows)
                        {
                            string estado = row["Estado"]?.ToString() ?? "";
                            if (estado == "Pendiente")
                            {
                                decimal total = Convert.ToDecimal(row["Total"]);
                                totalPendiente += total;
                            }
                        }
                    }

                    lblFacturasPendientes.Text = "$" + totalPendiente.ToString("N2");
                }

                // 4. Cargar Ingresos Hoy (solo Admin)
                if (lblIngresosHoy != null)
                {
                    DataTable dtEstadisticas = facturasDAL.ObtenerEstadisticas();
                    decimal ingresosHoy = 0;

                    if (dtEstadisticas != null && dtEstadisticas.Rows.Count > 0)
                    {
                        DataRow fila = dtEstadisticas.Rows[0];
                        if (fila["TotalPagadoHoy"] != DBNull.Value)
                        {
                            ingresosHoy = Convert.ToDecimal(fila["TotalPagadoHoy"]);
                        }
                    }

                    lblIngresosHoy.Text = "$" + ingresosHoy.ToString("N2");
                }
            }
            catch
            {
                // Error silencioso - mantener valores por defecto
                if (lblCitasDia != null) lblCitasDia.Text = "0";
                if (lblPacientesRegistrados != null) lblPacientesRegistrados.Text = "0";
                if (lblFacturasPendientes != null) lblFacturasPendientes.Text = "$0";
                if (lblIngresosHoy != null) lblIngresosHoy.Text = "$0";
            }
        }

        private void Salir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Seguro que deseas salir?", "Confirmación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

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