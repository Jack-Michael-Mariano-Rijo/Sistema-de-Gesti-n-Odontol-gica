using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Capa_negocio;

namespace Proyecto_final
{
    public partial class Citas : Form
    {
        CitasService service = new CitasService();
        DataTable dtCitas;
        DataTable dtPacientes;

        // Nuevas variables para Doctores
        DoctoresBLL doctoresBLL = new DoctoresBLL();
        DataTable dtDoctores;

        Panel sidebar;
        Panel topbar;
        Panel dashboard;
        string nombreUsuario;
        string rolUsuario;
        string rolNombre;  // Normalizado
        int doctorIDActual = 0;  // Para filtrar citas del doctor actual
        DataGridView tablaCitas;
        TextBox txtBuscar;
        Label lblTotalCitas;
        Label lblCitasHoy;
        Label lblCitasPendientes;
        Label lblCitasCompletadas;

        public Citas()
        {
            InitializeComponent();
            nombreUsuario = "Usuario";
            rolUsuario = "Recepcionista";
            ConvertirRol();
            ConfigurarFormulario();
            CrearSidebar();
            CrearTopbar();
            CrearDashboard();
            this.Resize += Citas_Resize;
        }

        public Citas(string usuario, string rol)
        {
            InitializeComponent();
            nombreUsuario = usuario;
            rolUsuario = rol;
            ConvertirRol();
            ConfigurarFormulario();
            CrearSidebar();
            CrearTopbar();
            CrearDashboard();
            this.Resize += Citas_Resize;
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
                    ObtenerDoctorID();
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

        // Obtener el ID del doctor basado en el nombre de usuario
        void ObtenerDoctorID()
        {
            try
            {
                DataTable dt = doctoresBLL.Mostrar();
                foreach (DataRow row in dt.Rows)
                {
                    if (row["Nombres"].ToString().ToLower() == nombreUsuario.ToLower())
                    {
                        doctorIDActual = Convert.ToInt32(row["DoctorID"]);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al obtener DoctorID: " + ex.Message);
            }
        }

        // ======================================================
        // VERIFICAR PERMISOS
        // ======================================================
        bool TienePermiso(string modulo, string accion = null)
        {
            // ADMINISTRADOR: todo
            if (rolNombre == "Administrador") return true;

            // DOCTOR
            if (rolNombre == "Doctor")
            {
                if (modulo == "Citas")
                {
                    if (accion == "Eliminar") return false;
                    return true;
                }
                if (modulo == "Facturacion") return true;
                if (modulo == "Reportes") return false;
                if (modulo == "NuevosDoctores") return false;
                if (modulo == "Pacientes") return true;
                return true;
            }

            // RECEPCIONISTA
            if (rolNombre == "Recepcionista")
            {
                if (modulo == "Citas")
                {
                    return true;
                }
                if (modulo == "Facturacion") return false;
                if (modulo == "Reportes") return false;
                if (modulo == "NuevosDoctores") return false;
                if (modulo == "Pacientes") return true;
                return true;
            }

            return false;
        }

        private void Citas_Resize(object sender, EventArgs e)
        {
            AjustarDashboard();
        }

        void ConfigurarFormulario()
        {
            this.Text = "Citas - Centro Odontologico";
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
            logo.Text = "Centro Odontologico\nDoctora Norabia";
            logo.ForeColor = Color.White;
            logo.Font = new Font("Segoe UI Semibold", 15, FontStyle.Bold);
            logo.Size = new Size(240, 70);
            logo.Location = new Point(10, 190);
            logo.TextAlign = ContentAlignment.MiddleCenter;
            sidebar.Controls.Add(logo);

            int topPosition = 300;

            CrearBotonMenu("Pagina Principal", topPosition, false);
            topPosition += 70;

            if (TienePermiso("Pacientes", null))
            {
                CrearBotonMenu("Pacientes", topPosition, false);
                topPosition += 70;
            }

            CrearBotonMenu("Citas", topPosition, true);
            topPosition += 70;

            if (TienePermiso("Facturacion", null))
            {
                CrearBotonMenu("Facturacion", topPosition, false);
                topPosition += 70;
            }

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
                if (!activo) btn.BackColor = Color.FromArgb(0, 92, 230);
            };

            btn.MouseLeave += (s, e) =>
            {
                if (!activo) btn.BackColor = Color.FromArgb(0, 44, 120);
            };

            btn.Click += (s, e) =>
            {
                if (texto == "Pagina Principal")
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
                else if (texto == "Facturacion")
                {
                    Facturacion frm = new Facturacion(nombreUsuario, rolUsuario);
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
            titulo.Text = "Centro Odontologico Doctora Norabia";
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
            titulo.Text = "Citas";
            titulo.Font = new Font("Segoe UI Semibold", 28, FontStyle.Bold);
            titulo.ForeColor = Color.FromArgb(15, 15, 40);
            titulo.AutoSize = true;
            titulo.Location = new Point(startX, startY);
            dashboard.Controls.Add(titulo);

            Label subtitulo = new Label();
            subtitulo.Text = "Gestion de citas programadas en el sistema";
            subtitulo.Font = new Font("Segoe UI", 14);
            subtitulo.ForeColor = Color.Gray;
            subtitulo.AutoSize = true;
            subtitulo.Location = new Point(startX + 5, startY + 45);
            dashboard.Controls.Add(subtitulo);

            int cardY = 110;
            CrearCard("Total Citas", "0", startX, cardY, "Registradas", out lblTotalCitas);
            CrearCard("Citas Hoy", "0", startX + 220, cardY, "Hoy", out lblCitasHoy);
            CrearCard("Citas Pendientes", "0", startX + 440, cardY, "Por realizar", out lblCitasPendientes);
            CrearCard("Citas Completadas", "0", startX + 660, cardY, "Este mes", out lblCitasCompletadas);

            Panel panelBuscar = new Panel();
            panelBuscar.Size = new Size(500, 50);
            panelBuscar.Location = new Point(startX, 280);
            panelBuscar.BackColor = Color.White;
            panelBuscar.BorderStyle = BorderStyle.FixedSingle;
            RedondearControl(panelBuscar, 12);
            dashboard.Controls.Add(panelBuscar);

            Label lblBuscar = new Label();
            lblBuscar.Text = "Buscar Cita:";
            lblBuscar.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
            lblBuscar.ForeColor = Color.FromArgb(0, 44, 120);
            lblBuscar.Location = new Point(15, 14);
            lblBuscar.AutoSize = true;
            panelBuscar.Controls.Add(lblBuscar);

            txtBuscar = new TextBox();
            txtBuscar.Size = new Size(300, 30);
            txtBuscar.Location = new Point(150, 11);
            txtBuscar.Font = new Font("Segoe UI", 11);
            txtBuscar.BorderStyle = BorderStyle.None;
            txtBuscar.TextChanged += (s, e) => FiltrarCitas();
            panelBuscar.Controls.Add(txtBuscar);

            CrearTablaCitas(startX, 350);
            CrearPanelAcciones(dashboard.Width - 300, 350);
            CargarDatosReales();
        }

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

        void CrearTablaCitas(int x, int y)
        {
            Panel panel = new Panel();
            panel.Size = new Size(dashboard.Width - 380, dashboard.Height - 350);
            panel.Location = new Point(x, y);
            panel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            panel.BackColor = Color.White;
            RedondearControl(panel, 20);
            dashboard.Controls.Add(panel);

            Label titulo = new Label();
            titulo.Text = "Listado de Citas";
            titulo.Font = new Font("Segoe UI Semibold", 18, FontStyle.Bold);
            titulo.Location = new Point(20, 20);
            titulo.AutoSize = true;
            panel.Controls.Add(titulo);

            tablaCitas = new DataGridView();
            tablaCitas.Size = new Size(panel.Width - 40, panel.Height - 80);
            tablaCitas.Location = new Point(20, 60);
            tablaCitas.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            tablaCitas.BackgroundColor = Color.White;
            tablaCitas.BorderStyle = BorderStyle.None;
            tablaCitas.RowHeadersVisible = false;
            tablaCitas.AllowUserToAddRows = false;
            tablaCitas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tablaCitas.EnableHeadersVisualStyles = false;
            tablaCitas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tablaCitas.MultiSelect = false;
            tablaCitas.ReadOnly = true;
            tablaCitas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 92, 230);
            tablaCitas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            tablaCitas.ColumnHeadersHeight = 45;
            tablaCitas.RowTemplate.Height = 38;

            tablaCitas.Columns.Clear();

            tablaCitas.Columns.Add("CitaID", "ID");
            tablaCitas.Columns["CitaID"].Visible = false;

            tablaCitas.Columns.Add("PacienteID", "PacienteID");
            tablaCitas.Columns["PacienteID"].Visible = false;

            tablaCitas.Columns.Add("DoctorID", "DoctorID");
            tablaCitas.Columns["DoctorID"].Visible = false;

            tablaCitas.Columns.Add("Paciente", "Paciente");
            tablaCitas.Columns.Add("Doctor", "Doctor");
            tablaCitas.Columns.Add("Fecha", "Fecha");
            tablaCitas.Columns.Add("Hora", "Hora");
            tablaCitas.Columns.Add("Estado", "Estado");
            tablaCitas.Columns.Add("Observaciones", "Observaciones");

            panel.Controls.Add(tablaCitas);
        }

        void CrearPanelAcciones(int x, int y)
        {
            Panel panel = new Panel();
            panel.Size = new Size(240, 260);
            panel.Location = new Point(x, y);
            panel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panel.BackColor = Color.White;
            RedondearControl(panel, 20);
            dashboard.Controls.Add(panel);

            Label titulo = new Label();
            titulo.Text = "Accesos Rapidos";
            titulo.Font = new Font("Segoe UI Semibold", 14, FontStyle.Bold);
            titulo.Location = new Point(20, 20);
            titulo.AutoSize = true;
            panel.Controls.Add(titulo);

            int topPosition = 70;

            Button btnNuevo = CrearBotonAccion("Nueva Cita", topPosition, Color.FromArgb(0, 92, 230));
            btnNuevo.Click += (s, e) => Nueva();
            panel.Controls.Add(btnNuevo);
            topPosition += 65;

            Button btnEditar = CrearBotonAccion("Editar Cita", topPosition, Color.FromArgb(0, 92, 230));
            btnEditar.Click += (s, e) => Editar();
            panel.Controls.Add(btnEditar);
            topPosition += 65;

            if (TienePermiso("Citas", "Eliminar"))
            {
                Button btnEliminar = CrearBotonAccion("Eliminar Cita", topPosition, Color.FromArgb(220, 53, 69));
                btnEliminar.Click += (s, e) => Eliminar();
                panel.Controls.Add(btnEliminar);
            }
        }

        Button CrearBotonAccion(string texto, int top, Color color)
        {
            Button btn = new Button();
            btn.Text = texto;
            btn.Size = new Size(190, 42);
            btn.Location = new Point(20, top);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            RedondearControl(btn, 10);

            btn.MouseEnter += (s, e) => { btn.BackColor = ControlPaint.Light(color, 0.2f); };
            btn.MouseLeave += (s, e) => { btn.BackColor = color; };

            return btn;
        }

        void Nueva()
        {
            if (!TienePermiso("Citas", "Nueva"))
            {
                MessageBox.Show("No tienes permiso para crear nuevas citas", "Acceso Denegado",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Form f = new Form
            {
                Text = "Nueva Cita",
                Size = new Size(450, 480),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = Color.FromArgb(0, 44, 120)
            };

            Label titulo = new Label
            {
                Text = "REGISTRAR NUEVA CITA",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 16)
            };

            header.Controls.Add(titulo);
            f.Controls.Add(header);

            Panel body = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.White,
                AutoScroll = true
            };

            f.Controls.Add(body);

            int y = 70;
            int labelWidth = 100;
            int controlWidth = 250;

            // PACIENTE
            Label lblPaciente = new Label
            {
                Text = "Paciente:",
                Location = new Point(10, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            ComboBox cmbPaciente = new ComboBox
            {
                Location = new Point(120, y),
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11)
            };
            CargarPacientes(cmbPaciente);
            y += 45;

            // DOCTOR
            Label lblDoctor = new Label
            {
                Text = "Doctor:",
                Location = new Point(10, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            ComboBox cmbDoctor = new ComboBox
            {
                Location = new Point(120, y),
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11)
            };
            CargarDoctores(cmbDoctor);
            y += 45;

            // FECHA
            Label lblFecha = new Label
            {
                Text = "Fecha:",
                Location = new Point(10, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            DateTimePicker dtpFecha = new DateTimePicker
            {
                Location = new Point(120, y),
                Width = controlWidth,
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 11),
                MinDate = DateTime.Today,
                MaxDate = DateTime.Today.AddYears(5)
            };
            y += 45;

            // HORA
            Label lblHora = new Label
            {
                Text = "Hora:",
                Location = new Point(10, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            DateTimePicker dtpHora = new DateTimePicker
            {
                Location = new Point(120, y),
                Width = controlWidth,
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Font = new Font("Segoe UI", 11)
            };
            y += 45;

            // ESTADO
            Label lblEstado = new Label
            {
                Text = "Estado:",
                Location = new Point(10, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            ComboBox cmbEstado = new ComboBox
            {
                Location = new Point(120, y),
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11)
            };
            cmbEstado.Items.AddRange(new[] { "Programada", "Confirmada", "Cancelada", "Completada" });
            cmbEstado.SelectedIndex = 0;
            y += 45;

            // OBSERVACIONES
            Label lblObs = new Label
            {
                Text = "Observaciones:",
                Location = new Point(10, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            TextBox txtObs = new TextBox
            {
                Location = new Point(120, y),
                Width = controlWidth,
                Height = 60,
                Multiline = true,
                Font = new Font("Segoe UI", 11)
            };
            y += 80;

            // BOTONES
            Button btnGuardar = new Button
            {
                Text = "Guardar",
                Location = new Point(120, y),
                Width = 120,
                Height = 38,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnGuardar.Click += (sender, e) =>
            {
                if (cmbPaciente.SelectedValue == null)
                {
                    MessageBox.Show("Seleccione un paciente", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbDoctor.SelectedValue == null)
                {
                    MessageBox.Show("Seleccione un doctor", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    bool resultado = service.InsertarCita(
                        Convert.ToInt32(cmbPaciente.SelectedValue),
                        Convert.ToInt32(cmbDoctor.SelectedValue),
                        dtpFecha.Value.Date,
                        dtpHora.Value.TimeOfDay,
                        cmbEstado.SelectedItem.ToString(),
                        txtObs.Text.Trim()
                    );

                    if (resultado)
                    {
                        MessageBox.Show("Cita guardada correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarDatosReales();
                        f.Close();
                    }
                    else
                    {
                        MessageBox.Show("Error al guardar la cita", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(250, y),
                Width = 100,
                Height = 38,
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancelar.Click += (sender, e) => f.Close();

            body.Controls.AddRange(new Control[]
            {
                lblPaciente, cmbPaciente,
                lblDoctor, cmbDoctor,
                lblFecha, dtpFecha,
                lblHora, dtpHora,
                lblEstado, cmbEstado,
                lblObs, txtObs,
                btnGuardar, btnCancelar
            });

            f.ShowDialog(this);
        }

        void Editar()
        {
            if (!TienePermiso("Citas", "Editar"))
            {
                MessageBox.Show("No tienes permiso para editar citas", "Acceso Denegado",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (tablaCitas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una cita para editar", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int citaID = Convert.ToInt32(tablaCitas.SelectedRows[0].Cells["CitaID"].Value);
            int pacienteID = Convert.ToInt32(tablaCitas.SelectedRows[0].Cells["PacienteID"].Value);
            int doctorID = Convert.ToInt32(tablaCitas.SelectedRows[0].Cells["DoctorID"].Value);
            string fecha = tablaCitas.SelectedRows[0].Cells["Fecha"].Value.ToString();
            string hora = tablaCitas.SelectedRows[0].Cells["Hora"].Value.ToString();
            string estado = tablaCitas.SelectedRows[0].Cells["Estado"].Value.ToString();
            string observaciones = tablaCitas.SelectedRows[0].Cells["Observaciones"].Value.ToString();

            Form f = new Form
            {
                Text = "Editar Cita",
                Size = new Size(450, 550),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = Color.FromArgb(0, 44, 120)
            };

            Label titulo = new Label
            {
                Text = "EDITAR CITA",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 16)
            };

            header.Controls.Add(titulo);
            f.Controls.Add(header);

            Panel body = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.White,
                AutoScroll = true
            };

            f.Controls.Add(body);

            int y = 20;
            int labelWidth = 100;
            int controlWidth = 250;

            // PACIENTE
            Label lblPaciente = new Label
            {
                Text = "Paciente:",
                Location = new Point(10, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            ComboBox cmbPaciente = new ComboBox
            {
                Location = new Point(120, y),
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11)
            };
            CargarPacientes(cmbPaciente);
            y += 45;

            // DOCTOR
            Label lblDoctor = new Label
            {
                Text = "Doctor:",
                Location = new Point(10, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            ComboBox cmbDoctor = new ComboBox
            {
                Location = new Point(120, y),
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11)
            };
            CargarDoctores(cmbDoctor);
            y += 45;

            // FECHA
            Label lblFecha = new Label
            {
                Text = "Fecha:",
                Location = new Point(10, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            DateTimePicker dtpFecha = new DateTimePicker
            {
                Location = new Point(120, y),
                Width = controlWidth,
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 11),
                MinDate = new DateTime(2020, 1, 1),
                MaxDate = new DateTime(2030, 12, 31)
            };
            y += 45;

            // HORA
            Label lblHora = new Label
            {
                Text = "Hora:",
                Location = new Point(10, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            DateTimePicker dtpHora = new DateTimePicker
            {
                Location = new Point(120, y),
                Width = controlWidth,
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Font = new Font("Segoe UI", 11)
            };
            y += 45;

            // ESTADO
            Label lblEstado = new Label
            {
                Text = "Estado:",
                Location = new Point(10, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            ComboBox cmbEstado = new ComboBox
            {
                Location = new Point(120, y),
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11)
            };
            cmbEstado.Items.AddRange(new[] { "Programada", "Confirmada", "Cancelada", "Completada" });
            y += 45;

            // OBSERVACIONES
            Label lblObs = new Label
            {
                Text = "Observaciones:",
                Location = new Point(10, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            TextBox txtObs = new TextBox
            {
                Location = new Point(120, y),
                Width = controlWidth,
                Height = 60,
                Multiline = true,
                Font = new Font("Segoe UI", 11)
            };
            y += 80;

            // BOTONES
            Button btnActualizar = new Button
            {
                Text = "Actualizar",
                Location = new Point(120, y),
                Width = 120,
                Height = 38,
                BackColor = Color.FromArgb(0, 92, 230),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnActualizar.Click += (sender, e) =>
            {
                try
                {
                    bool resultado = service.EditarCita(
                        citaID,
                        Convert.ToInt32(cmbPaciente.SelectedValue),
                        Convert.ToInt32(cmbDoctor.SelectedValue),
                        dtpFecha.Value.Date,
                        dtpHora.Value.TimeOfDay,
                        cmbEstado.SelectedItem.ToString(),
                        txtObs.Text.Trim()
                    );

                    if (resultado)
                    {
                        MessageBox.Show("Cita actualizada correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarDatosReales();
                        f.Close();
                    }
                    else
                    {
                        MessageBox.Show("Error al actualizar la cita", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(250, y),
                Width = 100,
                Height = 38,
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancelar.Click += (sender, e) => f.Close();

            // ======================================================
            // ASIGNAR VALORES CON VALIDACIÓN CORREGIDA
            // ======================================================
            try
            {
                // Asignar paciente y doctor
                if (cmbPaciente.Items.Count > 0)
                    cmbPaciente.SelectedValue = pacienteID;

                if (cmbDoctor.Items.Count > 0)
                    cmbDoctor.SelectedValue = doctorID;

                // Asignar fecha con validación
                DateTime fechaCita;
                if (DateTime.TryParse(fecha, out fechaCita))
                {
                    // Validar que la fecha esté en el rango permitido
                    if (fechaCita < dtpFecha.MinDate)
                        fechaCita = dtpFecha.MinDate;
                    if (fechaCita > dtpFecha.MaxDate)
                        fechaCita = dtpFecha.MaxDate;

                    dtpFecha.Value = fechaCita;
                }
                else
                {
                    dtpFecha.Value = DateTime.Today;
                }

                // Asignar hora
                if (!string.IsNullOrEmpty(hora))
                {
                    try
                    {
                        // Extraer solo la hora si viene con fecha completa
                        string soloHora = hora.Contains(" ") ? hora.Split(' ')[1] : hora;
                        DateTime horaCompleta = DateTime.Parse("2000-01-01 " + soloHora);
                        dtpHora.Value = horaCompleta;
                    }
                    catch
                    {
                        dtpHora.Value = DateTime.Now;
                    }
                }

                // Asignar estado
                if (cmbEstado.Items.Contains(estado))
                    cmbEstado.SelectedItem = estado;
                else
                    cmbEstado.SelectedIndex = 0;

                // Asignar observaciones
                txtObs.Text = observaciones;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            body.Controls.AddRange(new Control[]
            {
                lblPaciente, cmbPaciente,
                lblDoctor, cmbDoctor,
                lblFecha, dtpFecha,
                lblHora, dtpHora,
                lblEstado, cmbEstado,
                lblObs, txtObs,
                btnActualizar, btnCancelar
            });

            f.ShowDialog(this);
        }

        void Eliminar()
        {
            if (!TienePermiso("Citas", "Eliminar"))
            {
                MessageBox.Show("No tienes permiso para eliminar citas", "Acceso Denegado",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (tablaCitas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una cita para eliminar", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int citaID = Convert.ToInt32(tablaCitas.SelectedRows[0].Cells["CitaID"].Value);
            string paciente = tablaCitas.SelectedRows[0].Cells["Paciente"].Value.ToString();

            DialogResult result = MessageBox.Show($"¿Está seguro que desea eliminar la cita de {paciente}?",
                "Confirmar Eliminacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    bool resultado = service.EliminarCita(citaID);

                    if (resultado)
                    {
                        MessageBox.Show("Cita eliminada correctamente", "Exito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarDatosReales();
                    }
                    else
                    {
                        MessageBox.Show("Error al eliminar la cita", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void CargarDatosReales()
        {
            try
            {
                dtCitas = service.MostrarCitas();

                if (dtCitas != null && dtCitas.Rows.Count > 0)
                {
                    CargarTabla();
                    ActualizarCards();
                }
                else
                {
                    tablaCitas.Rows.Clear();
                    ActualizarCardsCero();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar citas: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void CargarTabla()
        {
            tablaCitas.Rows.Clear();

            foreach (DataRow row in dtCitas.Rows)
            {
                if (rolNombre == "Doctor")
                {
                    int doctorID = Convert.ToInt32(row["DoctorID"]);
                    if (doctorID != doctorIDActual)
                        continue;
                }

                // Formatear la hora sin decimales
                string horaFormateada = "N/A";
                if (row["HoraCita"] != DBNull.Value)
                {
                    TimeSpan horaTimeSpan = (TimeSpan)row["HoraCita"];
                    horaFormateada = horaTimeSpan.ToString(@"hh\:mm");
                }

                tablaCitas.Rows.Add(
                    row["CitaID"],
                    row["PacienteID"],
                    row["DoctorID"],
                    row["Paciente"]?.ToString() ?? "N/A",
                    row["Doctor"]?.ToString() ?? "N/A",
                    Convert.ToDateTime(row["FechaCita"]).ToString("dd/MM/yyyy"),
                    horaFormateada,
                    row["Estado"]?.ToString() ?? "N/A",
                    row["Observaciones"]?.ToString() ?? "N/A"
                );
            }

            lblTotalCitas.Text = tablaCitas.Rows.Count.ToString();
        }

        void FiltrarCitas()
        {
            if (dtCitas == null) return;

            string filtro = txtBuscar.Text.ToLower();

            if (string.IsNullOrEmpty(filtro))
            {
                CargarTabla();
                return;
            }

            tablaCitas.Rows.Clear();

            foreach (DataRow row in dtCitas.Rows)
            {
                if (rolNombre == "Doctor")
                {
                    int doctorID = Convert.ToInt32(row["DoctorID"]);
                    if (doctorID != doctorIDActual)
                        continue;
                }

                // Formatear la hora para el filtro también
                string horaFormateada = "N/A";
                if (row["HoraCita"] != DBNull.Value)
                {
                    TimeSpan horaTimeSpan = (TimeSpan)row["HoraCita"];
                    horaFormateada = horaTimeSpan.ToString(@"hh\:mm");
                }

                string paciente = row["Paciente"]?.ToString().ToLower() ?? "";
                string doctor = row["Doctor"]?.ToString().ToLower() ?? "";
                string fecha = Convert.ToDateTime(row["FechaCita"]).ToString("dd/MM/yyyy").ToLower();

                if (paciente.Contains(filtro) || doctor.Contains(filtro) || fecha.Contains(filtro))
                {
                    tablaCitas.Rows.Add(
                        row["CitaID"],
                        row["PacienteID"],
                        row["DoctorID"],
                        row["Paciente"]?.ToString() ?? "N/A",
                        row["Doctor"]?.ToString() ?? "N/A",
                        Convert.ToDateTime(row["FechaCita"]).ToString("dd/MM/yyyy"),
                        horaFormateada,
                        row["Estado"]?.ToString() ?? "N/A",
                        row["Observaciones"]?.ToString() ?? "N/A"
                    );
                }
            }
        }

        void ActualizarCards()
        {
            int totalCitas = 0;
            int citasHoy = 0;
            int citasPendientes = 0;
            int citasCompletadas = 0;

            foreach (DataRow row in dtCitas.Rows)
            {
                if (rolNombre == "Doctor")
                {
                    int doctorID = Convert.ToInt32(row["DoctorID"]);
                    if (doctorID != doctorIDActual)
                        continue;
                }

                totalCitas++;

                if (row.Table.Columns.Contains("FechaCita") && row["FechaCita"] != DBNull.Value)
                {
                    DateTime fechaCita = Convert.ToDateTime(row["FechaCita"]);
                    if (fechaCita.Date == DateTime.Today)
                        citasHoy++;
                }

                if (row.Table.Columns.Contains("Estado"))
                {
                    string estado = row["Estado"]?.ToString() ?? "";
                    if (estado == "Programada" || estado == "Confirmada")
                        citasPendientes++;
                    else if (estado == "Completada")
                        citasCompletadas++;
                }
            }

            lblTotalCitas.Text = totalCitas.ToString();
            lblCitasHoy.Text = citasHoy.ToString();
            lblCitasPendientes.Text = citasPendientes.ToString();
            lblCitasCompletadas.Text = citasCompletadas.ToString();
        }

        void ActualizarCardsCero()
        {
            lblTotalCitas.Text = "0";
            lblCitasHoy.Text = "0";
            lblCitasPendientes.Text = "0";
            lblCitasCompletadas.Text = "0";
        }

        void CargarPacientes(ComboBox cmb)
        {
            try
            {
                PacientesService pacientesService = new PacientesService();
                dtPacientes = pacientesService.Listar();

                cmb.DataSource = dtPacientes;
                cmb.DisplayMember = "Nombres";
                cmb.ValueMember = "PacienteID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar pacientes: " + ex.Message);
            }
        }

        void CargarDoctores(ComboBox cmb)
        {
            try
            {
                dtDoctores = doctoresBLL.Mostrar();

                cmb.DataSource = dtDoctores;
                cmb.DisplayMember = "Nombres";
                cmb.ValueMember = "DoctorID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar doctores: " + ex.Message);
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