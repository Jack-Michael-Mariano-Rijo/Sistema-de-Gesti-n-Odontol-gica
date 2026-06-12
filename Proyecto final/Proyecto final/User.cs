using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Capa_negocio;

namespace Proyecto_final
{
    public partial class User : Form
    {
        private TextBox txtUsuario;
        private TextBox txtPassword;
        private CheckBox chkRecordar;
        private Button btnIniciar;
        private PictureBox iconoOjo;

        public User()
        {
            InitializeComponent();
            ConfigurarFormulario();
            CrearInterfazLogin();
        }

        private void ConfigurarFormulario()
        {
            this.Text = "Centro Odontológico - Inicio de Sesión";
            this.Size = new Size(460, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
        }

        private void CrearInterfazLogin()
        {
            // Panel Superior
            Panel panelSuperior = new Panel
            {
                Height = 150,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(0, 44, 120)
            };
            this.Controls.Add(panelSuperior);

            // Icono Dental
            Label iconoLabel = new Label
            {
                Text = "🦷",
                Font = new Font("Segoe UI Emoji", 38),
                ForeColor = Color.White,
                Size = new Size(80, 60),
                Location = new Point((this.Width - 80) / 2, 12),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelSuperior.Controls.Add(iconoLabel);

            // Título
            Label tituloClinica = new Label
            {
                Text = "Centro Odontológico",
                Font = new Font("Segoe UI Semibold", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(350, 35),
                Location = new Point((this.Width - 350) / 2, 70),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelSuperior.Controls.Add(tituloClinica);

            // Subtítulo
            Label subtituloDoctora = new Label
            {
                Text = "Doctora Norabia",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(210, 225, 255),
                Size = new Size(200, 20),
                Location = new Point((this.Width - 200) / 2, 108),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelSuperior.Controls.Add(subtituloDoctora);

            // Panel Formulario
            Panel panelFormulario = new Panel
            {
                BackColor = Color.White,
                Size = new Size(360, 300),
                Location = new Point((this.Width - 360) / 2, 165)
            };
            this.Controls.Add(panelFormulario);

            // Bienvenida
            Label lblBienvenido = new Label
            {
                Text = "Bienvenido",
                Font = new Font("Segoe UI Semibold", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 50),
                Size = new Size(300, 30),
                Location = new Point(25, 8)
            };
            panelFormulario.Controls.Add(lblBienvenido);

            // Texto instructivo
            Label lblInstructivo = new Label
            {
                Text = "Inicia sesión para acceder al sistema",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                Size = new Size(300, 20),
                Location = new Point(25, 40)
            };
            panelFormulario.Controls.Add(lblInstructivo);

            // Usuario
            Label lblUsuario = new Label
            {
                Text = "Usuario",
                Font = new Font("Segoe UI Semibold", 9),
                ForeColor = Color.FromArgb(60, 60, 80),
                Location = new Point(25, 75),
                AutoSize = true
            };
            panelFormulario.Controls.Add(lblUsuario);

            txtUsuario = new TextBox
            {
                Size = new Size(310, 32),
                Location = new Point(25, 97),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250)
            };
            panelFormulario.Controls.Add(txtUsuario);

            // Contraseña
            Label lblPassword = new Label
            {
                Text = "Contraseña",
                Font = new Font("Segoe UI Semibold", 9),
                ForeColor = Color.FromArgb(60, 60, 80),
                Location = new Point(25, 140),
                AutoSize = true
            };
            panelFormulario.Controls.Add(lblPassword);

            Panel panelPassword = new Panel
            {
                Size = new Size(310, 34),
                Location = new Point(25, 162),
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            txtPassword = new TextBox
            {
                Size = new Size(272, 28),
                Location = new Point(6, 3),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(248, 249, 250),
                UseSystemPasswordChar = true
            };

            iconoOjo = new PictureBox
            {
                Size = new Size(18, 18),
                Location = new Point(284, 8),
                Image = SystemIcons.Information.ToBitmap(),
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand
            };
            iconoOjo.Click += IconoOjo_Click;

            panelPassword.Controls.Add(txtPassword);
            panelPassword.Controls.Add(iconoOjo);
            panelFormulario.Controls.Add(panelPassword);

            // Checkbox
            chkRecordar = new CheckBox
            {
                Text = "Recordarme",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(80, 80, 100),
                Location = new Point(25, 208),
                AutoSize = true
            };
            panelFormulario.Controls.Add(chkRecordar);

            // Botón
            btnIniciar = new Button
            {
                Text = "Iniciar Sesión",
                Size = new Size(310, 42),
                Location = new Point(25, 245),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 92, 230),
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnIniciar.FlatAppearance.BorderSize = 0;
            btnIniciar.Click += BtnIniciar_Click;
            btnIniciar.MouseEnter += (s, e) => btnIniciar.BackColor = Color.FromArgb(0, 44, 120);
            btnIniciar.MouseLeave += (s, e) => btnIniciar.BackColor = Color.FromArgb(0, 92, 230);
            panelFormulario.Controls.Add(btnIniciar);
        }

        private void IconoOjo_Click(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
            iconoOjo.Image = txtPassword.UseSystemPasswordChar ?
                SystemIcons.Information.ToBitmap() : SystemIcons.Shield.ToBitmap();
        }

        private void BtnIniciar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Completa los campos", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            UsuariosBLL bll = new UsuariosBLL();
            DataTable dt = bll.Login(usuario, password);

            if (dt != null && dt.Rows.Count > 0)
            {
                MessageBox.Show("Inicio de sesión exitoso", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                string nombre = dt.Rows[0]["NombreUsuario"].ToString();

                string rol = dt.Rows[0]["NombreRol"].ToString();

                this.Hide();

                Form1 formPrincipal = new Form1(nombre, rol);

                formPrincipal.Show();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                txtPassword.Clear();

                txtPassword.Focus();
            }
        }
    }
}