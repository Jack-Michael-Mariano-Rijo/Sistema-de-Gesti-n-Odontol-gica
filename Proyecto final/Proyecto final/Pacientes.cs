using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Capa_negocio;

namespace Proyecto_final
{
    public partial class Pacientes : Form
    {
        PacientesService service = new PacientesService();
        DataTable dtPacientes;

        Panel sidebar;
        Panel topbar;
        Panel dashboard;
        string nombreUsuario;
        string rolUsuario;
        string rolNombre;  // Normalizado
        DataGridView tablaPacientes;
        TextBox txtBuscar;
        Label lblTotalPacientes;
        Label lblPacientesActivos;
        Label lblConsultasMes;
        Label lblNuevosMes;

        public Pacientes()
        {
            InitializeComponent();
            nombreUsuario = "Usuario";
            rolUsuario = "Recepcionista";
            ConvertirRol();
            ConfigurarFormulario();
            CrearSidebar();
            CrearTopbar();
            CrearDashboard();
            this.Resize += Pacientes_Resize;
        }

        public Pacientes(string usuario, string rol)
        {
            InitializeComponent();
            nombreUsuario = usuario;
            rolUsuario = rol;
            ConvertirRol();
            ConfigurarFormulario();
            CrearSidebar();
            CrearTopbar();
            CrearDashboard();
            this.Resize += Pacientes_Resize;
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

            // DOCTOR
            if (rolNombre == "Doctor")
            {
                if (modulo == "Pacientes")
                {
                    if (accion == "Eliminar") return false;
                    return true;
                }
                if (modulo == "Citas") return true;
                if (modulo == "Facturacion") return true;  // Doctor puede facturar
                if (modulo == "Reportes") return false;
                if (modulo == "NuevosDoctores") return false;
                return true;
            }

            // RECEPCIONISTA
            if (rolNombre == "Recepcionista")
            {
                if (modulo == "Pacientes")
                {
                    return true;  // Todos los permisos
                }
                if (modulo == "Citas") return true;
                if (modulo == "Facturacion") return false;  // Recepcionista NO ve facturación
                if (modulo == "Reportes") return false;
                if (modulo == "NuevosDoctores") return false;
                return true;
            }

            return false;
        }

        private void Pacientes_Resize(object sender, EventArgs e)
        {
            AjustarDashboard();
        }

        void ConfigurarFormulario()
        {
            this.Text = "Pacientes - Centro Odontologico";
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

            CrearBotonMenu("Pacientes", topPosition, true);
            topPosition += 70;

            if (TienePermiso("Citas", null))
            {
                CrearBotonMenu("Citas", topPosition, false);
                topPosition += 70;
            }

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
                else if (texto == "Citas")
                {
                    Citas frm = new Citas(nombreUsuario, rolUsuario);
                    frm.Show();
                    this.Hide();
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
            titulo.Text = "Pacientes";
            titulo.Font = new Font("Segoe UI Semibold", 28, FontStyle.Bold);
            titulo.ForeColor = Color.FromArgb(15, 15, 40);
            titulo.AutoSize = true;
            titulo.Location = new Point(startX, startY);
            dashboard.Controls.Add(titulo);

            Label subtitulo = new Label();
            subtitulo.Text = "Gestion de pacientes registrados en el sistema";
            subtitulo.Font = new Font("Segoe UI", 14);
            subtitulo.ForeColor = Color.Gray;
            subtitulo.AutoSize = true;
            subtitulo.Location = new Point(startX + 5, startY + 45);
            dashboard.Controls.Add(subtitulo);

            int cardY = 110;
            CrearCard("Total Pacientes", "0", startX, cardY, "Registrados", out lblTotalPacientes);
            CrearCard("Consultas hoy", "0", startX + 220, cardY, "Hoy", out lblPacientesActivos);
            CrearCard("Consultas Mes", "0", startX + 440, cardY, "Este mes", out lblConsultasMes);
            CrearCard("Nuevos Pacientes", "0", startX + 660, cardY, "Ultimos 30 dias", out lblNuevosMes);

            Panel panelBuscar = new Panel();
            panelBuscar.Size = new Size(500, 50);
            panelBuscar.Location = new Point(startX, 280);
            panelBuscar.BackColor = Color.White;
            panelBuscar.BorderStyle = BorderStyle.FixedSingle;
            RedondearControl(panelBuscar, 12);
            dashboard.Controls.Add(panelBuscar);

            Label lblBuscar = new Label();
            lblBuscar.Text = "Buscar Paciente:";
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
            txtBuscar.TextChanged += (s, e) => FiltrarPacientes();
            panelBuscar.Controls.Add(txtBuscar);

            CrearTablaPacientes(startX, 350);
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

        void CrearTablaPacientes(int x, int y)
        {
            Panel panel = new Panel();
            panel.Size = new Size(dashboard.Width - 380, dashboard.Height - 350);
            panel.Location = new Point(x, y);
            panel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            panel.BackColor = Color.White;
            RedondearControl(panel, 20);
            dashboard.Controls.Add(panel);

            Label titulo = new Label();
            titulo.Text = "Listado de Pacientes";
            titulo.Font = new Font("Segoe UI Semibold", 18, FontStyle.Bold);
            titulo.Location = new Point(20, 20);
            titulo.AutoSize = true;
            panel.Controls.Add(titulo);

            tablaPacientes = new DataGridView();
            tablaPacientes.Size = new Size(panel.Width - 40, panel.Height - 80);
            tablaPacientes.Location = new Point(20, 60);
            tablaPacientes.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            tablaPacientes.BackgroundColor = Color.White;
            tablaPacientes.BorderStyle = BorderStyle.None;
            tablaPacientes.RowHeadersVisible = false;
            tablaPacientes.AllowUserToAddRows = false;
            tablaPacientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tablaPacientes.EnableHeadersVisualStyles = false;
            tablaPacientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tablaPacientes.MultiSelect = false;
            tablaPacientes.ReadOnly = true;
            tablaPacientes.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 92, 230);
            tablaPacientes.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            tablaPacientes.ColumnHeadersHeight = 45;
            tablaPacientes.RowTemplate.Height = 38;

            tablaPacientes.ColumnCount = 6;
            tablaPacientes.Columns[0].Name = "ID";
            tablaPacientes.Columns[0].Visible = false;
            tablaPacientes.Columns[1].Name = "Nombre Completo";
            tablaPacientes.Columns[2].Name = "Cedula";
            tablaPacientes.Columns[3].Name = "Telefono";
            tablaPacientes.Columns[4].Name = "Sexo";
            tablaPacientes.Columns[5].Name = "Direccion";

            panel.Controls.Add(tablaPacientes);
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

            // Botón Nuevo Paciente
            if (TienePermiso("Pacientes", "Nuevo"))
            {
                Button btnNuevo = CrearBotonAccion("Nuevo Paciente", topPosition, Color.FromArgb(0, 92, 230));
                btnNuevo.Click += (s, e) => Nuevo();
                panel.Controls.Add(btnNuevo);
                topPosition += 65;
            }

            // Botón Editar Paciente
            if (TienePermiso("Pacientes", "Editar"))
            {
                Button btnEditar = CrearBotonAccion("Editar Paciente", topPosition, Color.FromArgb(0, 92, 230));
                btnEditar.Click += (s, e) => Editar();
                panel.Controls.Add(btnEditar);
                topPosition += 65;
            }

            // Botón Eliminar Paciente
            if (TienePermiso("Pacientes", "Eliminar"))
            {
                Button btnEliminar = CrearBotonAccion("Eliminar Paciente", topPosition, Color.FromArgb(220, 53, 69));
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

        void Nuevo()
        {
            if (!TienePermiso("Pacientes", "Nuevo"))
            {
                MessageBox.Show("No tienes permiso para registrar nuevos pacientes", "Acceso Denegado",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Form f = new Form
            {
                Text = "Nuevo Paciente",
                Size = new Size(500, 550),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(0, 44, 120)
            };

            Label titulo = new Label
            {
                Text = "REGISTRAR PACIENTE",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 18)
            };

            header.Controls.Add(titulo);
            f.Controls.Add(header);

            Panel body = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.White
            };

            f.Controls.Add(body);

            int y = 90;
            int labelWidth = 80;
            int controlWidth = 340;

            Label lblNombres = new Label
            {
                Text = "Nombres:",
                Location = new Point(30, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 44, 120)
            };
            TextBox txtNombres = new TextBox
            {
                Location = new Point(110, y),
                Width = controlWidth,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };
            y += 50;

            Label lblApellidos = new Label
            {
                Text = "Apellidos:",
                Location = new Point(30, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 44, 120)
            };
            TextBox txtApellidos = new TextBox
            {
                Location = new Point(110, y),
                Width = controlWidth,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };
            y += 50;

            Label lblCedula = new Label
            {
                Text = "Cédula:",
                Location = new Point(30, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 44, 120)
            };
            TextBox txtCedula = new TextBox
            {
                Location = new Point(110, y),
                Width = controlWidth,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };
            y += 50;

            Label lblSexo = new Label
            {
                Text = "Sexo:",
                Location = new Point(30, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 44, 120)
            };
            ComboBox cmbSexo = new ComboBox
            {
                Location = new Point(110, y),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11)
            };
            cmbSexo.Items.AddRange(new[] { "Masculino", "Femenino" });
            cmbSexo.SelectedIndex = 0;
            y += 50;

            Label lblTelefono = new Label
            {
                Text = "Teléfono:",
                Location = new Point(30, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 44, 120)
            };
            TextBox txtTelefono = new TextBox
            {
                Location = new Point(110, y),
                Width = controlWidth,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };
            y += 50;

            Label lblDireccion = new Label
            {
                Text = "Dirección:",
                Location = new Point(30, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 44, 120)
            };
            TextBox txtDireccion = new TextBox
            {
                Location = new Point(110, y),
                Width = controlWidth,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };

            txtCedula.MaxLength = 11;
            txtTelefono.MaxLength = 12;
            txtCedula.KeyPress += SoloNumeros;
            txtTelefono.KeyPress += SoloNumeros;
            txtCedula.TextChanged += FormatoCedula;
            txtTelefono.TextChanged += FormatoTelefono;

            Button btnGuardar = new Button
            {
                Text = "Guardar",
                Location = new Point(130, y + 50),
                Width = 120,
                Height = 40,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(270, y + 50),
                Width = 100,
                Height = 40,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnGuardar.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNombres.Text) || string.IsNullOrWhiteSpace(txtApellidos.Text))
                {
                    MessageBox.Show("Complete los campos obligatorios", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!string.IsNullOrEmpty(txtCedula.Text) && !ValidarCedula(txtCedula.Text))
                {
                    MessageBox.Show("Formato de cédula inválido. Ejemplo: 001-1234567", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCedula.Focus();
                    return;
                }

                if (!string.IsNullOrEmpty(txtTelefono.Text) && !ValidarTelefono(txtTelefono.Text))
                {
                    MessageBox.Show("Formato de teléfono inválido. Ejemplo: 809-555-1234", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTelefono.Focus();
                    return;
                }

                string sexo = (cmbSexo.SelectedItem.ToString() == "Masculino") ? "M" : "F";

                try
                {
                    service.Insertar(
                        txtNombres.Text.Trim(),
                        txtApellidos.Text.Trim(),
                        txtCedula.Text.Replace("-", "").Trim(),
                        sexo,
                        txtTelefono.Text.Trim(),
                        txtDireccion.Text.Trim()
                    );

                    MessageBox.Show("Paciente guardado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarDatosReales();
                    f.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnCancelar.Click += (s, e) => f.Close();

            RedondearControl(btnGuardar, 8);
            RedondearControl(btnCancelar, 8);

            body.Controls.AddRange(new Control[] {
                lblNombres, txtNombres,
                lblApellidos, txtApellidos,
                lblCedula, txtCedula,
                lblSexo, cmbSexo,
                lblTelefono, txtTelefono,
                lblDireccion, txtDireccion,
                btnGuardar, btnCancelar
            });

            f.ShowDialog(this);
        }

        void Editar()
        {
            if (!TienePermiso("Pacientes", "Editar"))
            {
                MessageBox.Show("No tienes permiso para editar pacientes", "Acceso Denegado",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (tablaPacientes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un paciente para editar", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(tablaPacientes.SelectedRows[0].Cells[0].Value);
            string nombreCompleto = tablaPacientes.SelectedRows[0].Cells[1].Value.ToString();
            string cedula = tablaPacientes.SelectedRows[0].Cells[2].Value.ToString();
            string telefono = tablaPacientes.SelectedRows[0].Cells[3].Value.ToString();
            string sexo = tablaPacientes.SelectedRows[0].Cells[4].Value.ToString();
            string direccion = tablaPacientes.SelectedRows[0].Cells[5].Value.ToString();

            string[] partes = nombreCompleto.Split(' ');
            string nombres = partes.Length > 0 ? partes[0] : "";
            string apellidos = partes.Length > 1 ? string.Join(" ", partes, 1, partes.Length - 1) : "";

            Form f = new Form
            {
                Text = "Editar Paciente",
                Size = new Size(500, 550),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(0, 44, 120)
            };

            Label titulo = new Label
            {
                Text = "EDITAR PACIENTE",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 18)
            };

            header.Controls.Add(titulo);
            f.Controls.Add(header);

            Panel body = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.White
            };

            f.Controls.Add(body);

            int y = 20;
            int labelWidth = 80;
            int controlWidth = 340;

            Label lblNombres = new Label
            {
                Text = "Nombres:",
                Location = new Point(30, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 44, 120)
            };
            TextBox txtNombres = new TextBox
            {
                Text = nombres,
                Location = new Point(110, y),
                Width = controlWidth,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };
            y += 50;

            Label lblApellidos = new Label
            {
                Text = "Apellidos:",
                Location = new Point(30, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 44, 120)
            };
            TextBox txtApellidos = new TextBox
            {
                Text = apellidos,
                Location = new Point(110, y),
                Width = controlWidth,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };
            y += 50;

            Label lblCedula = new Label
            {
                Text = "Cédula:",
                Location = new Point(30, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 44, 120)
            };
            TextBox txtCedula = new TextBox
            {
                Text = cedula,
                Location = new Point(110, y),
                Width = controlWidth,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };
            y += 50;

            Label lblSexo = new Label
            {
                Text = "Sexo:",
                Location = new Point(30, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 44, 120)
            };
            ComboBox cmbSexo = new ComboBox
            {
                Location = new Point(110, y),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11)
            };
            cmbSexo.Items.AddRange(new[] { "Masculino", "Femenino" });
            cmbSexo.SelectedItem = (sexo == "M") ? "Masculino" : "Femenino";
            y += 50;

            Label lblTelefono = new Label
            {
                Text = "Teléfono:",
                Location = new Point(30, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 44, 120)
            };
            TextBox txtTelefono = new TextBox
            {
                Text = telefono,
                Location = new Point(110, y),
                Width = controlWidth,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };
            y += 50;

            Label lblDireccion = new Label
            {
                Text = "Dirección:",
                Location = new Point(30, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 44, 120)
            };
            TextBox txtDireccion = new TextBox
            {
                Text = direccion,
                Location = new Point(110, y),
                Width = controlWidth,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };

            txtCedula.MaxLength = 11;
            txtTelefono.MaxLength = 12;
            txtCedula.KeyPress += SoloNumeros;
            txtTelefono.KeyPress += SoloNumeros;
            txtCedula.TextChanged += FormatoCedula;
            txtTelefono.TextChanged += FormatoTelefono;

            Button btnActualizar = new Button
            {
                Text = "Actualizar",
                Location = new Point(130, y + 50),
                Width = 120,
                Height = 40,
                BackColor = Color.FromArgb(0, 92, 230),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(270, y + 50),
                Width = 100,
                Height = 40,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnActualizar.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNombres.Text) || string.IsNullOrWhiteSpace(txtApellidos.Text))
                {
                    MessageBox.Show("Complete los campos obligatorios", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!string.IsNullOrEmpty(txtCedula.Text) && !ValidarCedula(txtCedula.Text))
                {
                    MessageBox.Show("Formato de cédula inválido. Ejemplo: 001-1234567", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCedula.Focus();
                    return;
                }

                if (!string.IsNullOrEmpty(txtTelefono.Text) && !ValidarTelefono(txtTelefono.Text))
                {
                    MessageBox.Show("Formato de teléfono inválido. Ejemplo: 809-555-1234", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTelefono.Focus();
                    return;
                }

                try
                {
                    service.Editar(
                        id,
                        txtNombres.Text.Trim(),
                        txtApellidos.Text.Trim(),
                        txtCedula.Text.Replace("-", "").Trim(),
                        cmbSexo.SelectedItem.ToString() == "Masculino" ? "M" : "F",
                        txtTelefono.Text.Trim(),
                        txtDireccion.Text.Trim()
                    );

                    MessageBox.Show("Paciente actualizado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarDatosReales();
                    f.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnCancelar.Click += (s, e) => f.Close();

            RedondearControl(btnActualizar, 8);
            RedondearControl(btnCancelar, 8);

            body.Controls.AddRange(new Control[] {
                lblNombres, txtNombres,
                lblApellidos, txtApellidos,
                lblCedula, txtCedula,
                lblSexo, cmbSexo,
                lblTelefono, txtTelefono,
                lblDireccion, txtDireccion,
                btnActualizar, btnCancelar
            });

            f.ShowDialog(this);
        }

        void Eliminar()
        {
            if (!TienePermiso("Pacientes", "Eliminar"))
            {
                MessageBox.Show("No tienes permiso para eliminar pacientes", "Acceso Denegado",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (tablaPacientes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un paciente para eliminar", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(tablaPacientes.SelectedRows[0].Cells[0].Value);
            string nombre = tablaPacientes.SelectedRows[0].Cells[1].Value.ToString();

            DialogResult result = MessageBox.Show($"¿Está seguro que desea eliminar al paciente {nombre}?",
                "Confirmar Eliminacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    service.Eliminar(id);
                    MessageBox.Show("Paciente eliminado correctamente", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarDatosReales();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SoloNumeros(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        private void FormatoCedula(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            string numeros = new string(txt.Text.Where(char.IsDigit).ToArray());
            if (numeros.Length > 10) numeros = numeros.Substring(0, 10);
            string resultado = numeros;
            if (numeros.Length > 3) resultado = numeros.Insert(3, "-");
            txt.TextChanged -= FormatoCedula;
            txt.Text = resultado;
            txt.SelectionStart = txt.Text.Length;
            txt.TextChanged += FormatoCedula;
        }

        private void FormatoTelefono(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            string numeros = new string(txt.Text.Where(char.IsDigit).ToArray());
            if (numeros.Length > 10) numeros = numeros.Substring(0, 10);
            string resultado = numeros;
            if (numeros.Length > 3) resultado = numeros.Insert(3, "-");
            if (numeros.Length > 6) resultado = resultado.Insert(7, "-");
            txt.TextChanged -= FormatoTelefono;
            txt.Text = resultado;
            txt.SelectionStart = txt.Text.Length;
            txt.TextChanged += FormatoTelefono;
        }

        bool ValidarCedula(string cedula)
        {
            if (string.IsNullOrEmpty(cedula)) return true;
            Regex regex = new Regex(@"^\d{3}-\d{7}$");
            return regex.IsMatch(cedula);
        }

        bool ValidarTelefono(string telefono)
        {
            if (string.IsNullOrEmpty(telefono)) return true;
            Regex regex = new Regex(@"^(809|829|849)-\d{3}-\d{4}$");
            return regex.IsMatch(telefono);
        }

        void CargarDatosReales()
        {
            try
            {
                dtPacientes = service.Listar();

                if (dtPacientes != null && dtPacientes.Rows.Count > 0)
                {
                    CargarTabla();
                    ActualizarCards();
                }
                else
                {
                    tablaPacientes.Rows.Clear();
                    ActualizarCardsCero();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar pacientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void CargarTabla()
        {
            tablaPacientes.Rows.Clear();
            foreach (DataRow row in dtPacientes.Rows)
            {
                tablaPacientes.Rows.Add(
                    row["PacienteID"],
                    row["Nombres"] + " " + row["Apellidos"],
                    row["Cedula"]?.ToString() ?? "N/A",
                    row["Telefono"]?.ToString() ?? "N/A",
                    row["Sexo"]?.ToString() ?? "N/A",
                    row["Direccion"]?.ToString() ?? "N/A"
                );
            }
            lblTotalPacientes.Text = tablaPacientes.Rows.Count.ToString();
        }

        void FiltrarPacientes()
        {
            if (dtPacientes == null) return;
            string filtro = txtBuscar.Text.ToLower();
            if (string.IsNullOrEmpty(filtro))
            {
                CargarTabla();
                return;
            }
            tablaPacientes.Rows.Clear();
            foreach (DataRow row in dtPacientes.Rows)
            {
                string nombreCompleto = (row["Nombres"] + " " + row["Apellidos"]).ToLower();
                string cedula = row["Cedula"]?.ToString().ToLower() ?? "";
                if (nombreCompleto.Contains(filtro) || cedula.Contains(filtro))
                {
                    tablaPacientes.Rows.Add(
                        row["PacienteID"],
                        row["Nombres"] + " " + row["Apellidos"],
                        row["Cedula"]?.ToString() ?? "N/A",
                        row["Telefono"]?.ToString() ?? "N/A",
                        row["Sexo"]?.ToString() ?? "N/A",
                        row["Direccion"]?.ToString() ?? "N/A"
                    );
                }
            }
        }

        void ActualizarCards()
        {
            lblTotalPacientes.Text = dtPacientes.Rows.Count.ToString();
            int nuevos = 0;
            foreach (DataRow row in dtPacientes.Rows)
            {
                if (row.Table.Columns.Contains("FechaRegistro") && row["FechaRegistro"] != DBNull.Value)
                {
                    DateTime fechaReg = Convert.ToDateTime(row["FechaRegistro"]);
                    if (fechaReg >= DateTime.Now.AddDays(-30))
                        nuevos++;
                }
            }
            lblNuevosMes.Text = nuevos.ToString();
            lblPacientesActivos.Text = "0";
            lblConsultasMes.Text = "0";
        }

        void ActualizarCardsCero()
        {
            lblTotalPacientes.Text = "0";
            lblNuevosMes.Text = "0";
            lblPacientesActivos.Text = "0";
            lblConsultasMes.Text = "0";
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