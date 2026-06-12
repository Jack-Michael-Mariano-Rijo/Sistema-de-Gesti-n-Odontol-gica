using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Capa_datos;

namespace Proyecto_final
{
    public partial class Horarios : Form
    {
        HorariosDAL horariosDAL = new HorariosDAL();
        DataTable tablaHorarios = new DataTable();

        string usuario;
        string rol;
        int horarioIDSeleccionado = 0;

        DataGridView grid;
        ComboBox cmbDoctor;
        ComboBox cmbDia;
        DateTimePicker dtpHoraInicio;
        DateTimePicker dtpHoraFin;

        Color colorPrimario = Color.FromArgb(52, 73, 94);
        Color colorSecundario = Color.FromArgb(41, 128, 185);
        Color colorFondo = Color.FromArgb(236, 240, 241);
        Color colorPanel = Color.FromArgb(255, 255, 255);
        Color colorTexto = Color.FromArgb(44, 62, 80);
        Color colorBotonRegresar = Color.FromArgb(231, 76, 60);

        public Horarios(string user, string rolUser)
        {
            InitializeComponent();

            usuario = user;
            rol = rolUser;

            ConfigurarFormulario();
            CrearUI();
            CargarDoctores();
            CargarHorarios();
        }

        void ConfigurarFormulario()
        {
            this.Text = "Módulo de Horarios - Hospital Management System";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = colorFondo;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        void CrearUI()
        {
            // ===== HEADER =====
            Panel header = new Panel();
            header.Size = new Size(this.Width, 80);
            header.Location = new Point(0, 0);
            header.BackColor = colorPrimario;
            header.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.Controls.Add(header);

            // Título en el header
            Label titulo = new Label();
            titulo.Text = "Gestión de Horarios";
            titulo.Font = new Font("Segoe UI", 28, FontStyle.Bold);
            titulo.ForeColor = Color.White;
            titulo.Location = new Point(30, 15);
            titulo.AutoSize = true;
            header.Controls.Add(titulo);

            // ===== BOTÓN REGRESAR =====
            Button btnRegresar = new Button();
            btnRegresar.Text = "← Volver al Menú Principal";
            btnRegresar.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnRegresar.Size = new Size(220, 45);
            btnRegresar.Location = new Point(this.Width - 260, 18);
            btnRegresar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRegresar.BackColor = colorBotonRegresar;
            btnRegresar.ForeColor = Color.White;
            btnRegresar.FlatStyle = FlatStyle.Flat;
            btnRegresar.FlatAppearance.BorderSize = 0;
            btnRegresar.Cursor = Cursors.Hand;

            btnRegresar.MouseEnter += (s, e) => {
                btnRegresar.BackColor = Color.FromArgb(192, 57, 43);
            };
            btnRegresar.MouseLeave += (s, e) => {
                btnRegresar.BackColor = colorBotonRegresar;
            };

            btnRegresar.Click += BtnRegresar_Click;
            header.Controls.Add(btnRegresar);

            // ===== PANEL DE FORMULARIO =====
            Panel panelFormulario = new Panel();
            panelFormulario.Size = new Size(450, 380);
            panelFormulario.Location = new Point(30, 110);
            panelFormulario.BackColor = colorPanel;
            panelFormulario.BorderStyle = BorderStyle.None;
            panelFormulario.Padding = new Padding(20);
            this.Controls.Add(panelFormulario);

            Panel sombraPanel = new Panel();
            sombraPanel.Size = new Size(452, 382);
            sombraPanel.Location = new Point(29, 109);
            sombraPanel.BackColor = Color.FromArgb(189, 195, 199);
            this.Controls.Add(sombraPanel);
            sombraPanel.SendToBack();

            Label lblPanelTitulo = new Label();
            lblPanelTitulo.Text = "📅 Registrar Horario";
            lblPanelTitulo.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblPanelTitulo.ForeColor = colorPrimario;
            lblPanelTitulo.Location = new Point(20, 15);
            lblPanelTitulo.AutoSize = true;
            panelFormulario.Controls.Add(lblPanelTitulo);

            // ComboBox para Doctores
            Label lblDoctor = new Label();
            lblDoctor.Text = "Seleccionar Doctor";
            lblDoctor.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblDoctor.ForeColor = Color.FromArgb(71, 85, 105);
            lblDoctor.Location = new Point(20, 60);
            lblDoctor.AutoSize = true;
            panelFormulario.Controls.Add(lblDoctor);

            cmbDoctor = new ComboBox();
            cmbDoctor.Size = new Size(400, 35);
            cmbDoctor.Location = new Point(20, 82);
            cmbDoctor.Font = new Font("Segoe UI", 10);
            cmbDoctor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDoctor.BackColor = Color.FromArgb(248, 249, 250);
            panelFormulario.Controls.Add(cmbDoctor);

            // ComboBox para Días
            Label lblDia = new Label();
            lblDia.Text = "Día de Trabajo";
            lblDia.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblDia.ForeColor = Color.FromArgb(71, 85, 105);
            lblDia.Location = new Point(20, 120);
            lblDia.AutoSize = true;
            panelFormulario.Controls.Add(lblDia);

            cmbDia = new ComboBox();
            cmbDia.Size = new Size(400, 35);
            cmbDia.Location = new Point(20, 142);
            cmbDia.Font = new Font("Segoe UI", 10);
            cmbDia.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDia.BackColor = Color.FromArgb(248, 249, 250);
            cmbDia.Items.AddRange(new string[] {
                "Lunes", "Martes", "Miércoles", "Jueves",
                "Viernes", "Sábado", "Domingo"
            });
            cmbDia.SelectedIndex = 0;
            panelFormulario.Controls.Add(cmbDia);

            // DateTimePicker para Hora Inicio
            Label lblHoraInicio = new Label();
            lblHoraInicio.Text = "Hora de Entrada";
            lblHoraInicio.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblHoraInicio.ForeColor = Color.FromArgb(71, 85, 105);
            lblHoraInicio.Location = new Point(20, 180);
            lblHoraInicio.AutoSize = true;
            panelFormulario.Controls.Add(lblHoraInicio);

            dtpHoraInicio = new DateTimePicker();
            dtpHoraInicio.Size = new Size(400, 35);
            dtpHoraInicio.Location = new Point(20, 202);
            dtpHoraInicio.Font = new Font("Segoe UI", 10);
            dtpHoraInicio.Format = DateTimePickerFormat.Time;
            dtpHoraInicio.ShowUpDown = true;
            dtpHoraInicio.Value = DateTime.Parse("08:00");
            panelFormulario.Controls.Add(dtpHoraInicio);

            // DateTimePicker para Hora Fin
            Label lblHoraFin = new Label();
            lblHoraFin.Text = "Hora de Salida";
            lblHoraFin.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblHoraFin.ForeColor = Color.FromArgb(71, 85, 105);
            lblHoraFin.Location = new Point(20, 240);
            lblHoraFin.AutoSize = true;
            panelFormulario.Controls.Add(lblHoraFin);

            dtpHoraFin = new DateTimePicker();
            dtpHoraFin.Size = new Size(400, 35);
            dtpHoraFin.Location = new Point(20, 262);
            dtpHoraFin.Font = new Font("Segoe UI", 10);
            dtpHoraFin.Format = DateTimePickerFormat.Time;
            dtpHoraFin.ShowUpDown = true;
            dtpHoraFin.Value = DateTime.Parse("17:00");
            panelFormulario.Controls.Add(dtpHoraFin);

            // Botones
            Button btnAgregar = CrearBoton(panelFormulario, "✓ Registrar", 320, Color.FromArgb(39, 174, 96));
            btnAgregar.Click += Agregar;

            Button btnEditar = CrearBoton(panelFormulario, "✎ Editar", 320, colorSecundario);
            btnEditar.Left = 130;
            btnEditar.Click += Editar;

            Button btnEliminar = CrearBoton(panelFormulario, "✕ Eliminar", 320, Color.FromArgb(231, 76, 60));
            btnEliminar.Left = 240;
            btnEliminar.Click += Eliminar;

            Button btnLimpiar = CrearBoton(panelFormulario, "↺ Limpiar", 320, Color.FromArgb(149, 165, 166));
            btnLimpiar.Left = 350;
            btnLimpiar.Click += Limpiar;

            // ===== GRID =====
            Label lblGrid = new Label();
            lblGrid.Text = "📋 Horarios Registrados";
            lblGrid.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblGrid.ForeColor = colorPrimario;
            lblGrid.Location = new Point(510, 110);
            lblGrid.AutoSize = true;
            this.Controls.Add(lblGrid);

            Label lblContador = new Label();
            lblContador.Text = "0 registros";
            lblContador.Name = "lblContador";
            lblContador.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblContador.ForeColor = Color.FromArgb(127, 140, 141);
            lblContador.Location = new Point(510, 140);
            lblContador.AutoSize = true;
            this.Controls.Add(lblContador);

            grid = new DataGridView();
            grid.Size = new Size(750, 450);
            grid.Location = new Point(510, 165);
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.RowHeadersVisible = false;

            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = colorPrimario;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.ColumnHeadersHeight = 40;

            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);
            grid.DefaultCellStyle.ForeColor = colorTexto;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            grid.DefaultCellStyle.SelectionForeColor = Color.White;

            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            grid.RowTemplate.Height = 35;

            grid.CellClick += Grid_CellClick;

            this.Controls.Add(grid);

            Label lblInfo = new Label();
            lblInfo.Text = "💡 Seleccione un registro para editar o eliminar";
            lblInfo.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            lblInfo.ForeColor = Color.FromArgb(127, 140, 141);
            lblInfo.Location = new Point(510, 625);
            lblInfo.AutoSize = true;
            this.Controls.Add(lblInfo);
        }

        void CargarDoctores()
        {
            try
            {
                DoctoresDAL doctoresDAL = new DoctoresDAL();
                DataTable doctores = doctoresDAL.ObtenerDoctores();

                if (doctores != null && doctores.Rows.Count > 0)
                {
                    cmbDoctor.DataSource = doctores;
                    cmbDoctor.DisplayMember = "NombreCompleto";
                    cmbDoctor.ValueMember = "DoctorID";
                }
                else
                {
                    MessageBox.Show("No hay doctores registrados en el sistema. Por favor registre doctores primero.",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar doctores: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void CargarHorarios()
        {
            try
            {
                tablaHorarios = horariosDAL.MostrarHorarios();
                grid.DataSource = tablaHorarios;

                if (grid.Columns["HorarioID"] != null)
                    grid.Columns["HorarioID"].Visible = false;
                if (grid.Columns["DoctorID"] != null)
                    grid.Columns["DoctorID"].Visible = false;

                if (grid.Columns["NombreDoctor"] != null)
                    grid.Columns["NombreDoctor"].HeaderText = "Doctor";
                if (grid.Columns["DiaSemana"] != null)
                    grid.Columns["DiaSemana"].HeaderText = "Día";
                if (grid.Columns["HoraInicio"] != null)
                    grid.Columns["HoraInicio"].HeaderText = "Hora Entrada";
                if (grid.Columns["HoraFin"] != null)
                    grid.Columns["HoraFin"].HeaderText = "Hora Salida";

                if (grid.Columns["HoraInicio"] != null)
                {
                    grid.Columns["HoraInicio"].DefaultCellStyle.Format = "hh\\:mm";
                }
                if (grid.Columns["HoraFin"] != null)
                {
                    grid.Columns["HoraFin"].DefaultCellStyle.Format = "hh\\:mm";
                }

                ActualizarContador();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar horarios: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void Agregar(object sender, EventArgs e)
        {
            if (cmbDoctor.SelectedValue == null)
            {
                MessageBox.Show("Por favor seleccione un doctor.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(cmbDia.Text))
            {
                MessageBox.Show("Por favor seleccione un día.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int doctorID = Convert.ToInt32(cmbDoctor.SelectedValue);
                string dia = cmbDia.Text;
                TimeSpan horaInicio = dtpHoraInicio.Value.TimeOfDay;
                TimeSpan horaFin = dtpHoraFin.Value.TimeOfDay;

                if (horariosDAL.InsertarHorario(doctorID, dia, horaInicio, horaFin))
                {
                    MessageBox.Show("Horario registrado exitosamente.",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarHorarios();
                    LimpiarCampos();
                }
                else
                {
                    MessageBox.Show("Error al registrar el horario.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void Editar(object sender, EventArgs e)
        {
            if (horarioIDSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un registro para editar.",
                    "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (cmbDoctor.SelectedValue == null)
            {
                MessageBox.Show("Por favor seleccione un doctor.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("¿Está seguro de modificar este horario?",
                "Confirmar Edición", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    int doctorID = Convert.ToInt32(cmbDoctor.SelectedValue);
                    string dia = cmbDia.Text;
                    TimeSpan horaInicio = dtpHoraInicio.Value.TimeOfDay;
                    TimeSpan horaFin = dtpHoraFin.Value.TimeOfDay;

                    if (horariosDAL.EditarHorario(horarioIDSeleccionado, doctorID, dia, horaInicio, horaFin))
                    {
                        MessageBox.Show("Horario modificado exitosamente.",
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarHorarios();
                        LimpiarCampos();
                    }
                    else
                    {
                        MessageBox.Show("Error al modificar el horario.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void Eliminar(object sender, EventArgs e)
        {
            if (horarioIDSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un registro para eliminar.",
                    "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show("¿Está seguro de eliminar este horario?\nEsta acción no se puede deshacer.",
                "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    if (horariosDAL.EliminarHorario(horarioIDSeleccionado))
                    {
                        MessageBox.Show("Horario eliminado exitosamente.",
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarHorarios();
                        LimpiarCampos();
                    }
                    else
                    {
                        MessageBox.Show("Error al eliminar el horario.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void Limpiar(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        void LimpiarCampos()
        {
            if (cmbDoctor.Items.Count > 0)
                cmbDoctor.SelectedIndex = -1;

            cmbDia.SelectedIndex = 0;
            dtpHoraInicio.Value = DateTime.Parse("08:00");
            dtpHoraFin.Value = DateTime.Parse("17:00");
            horarioIDSeleccionado = 0;
            cmbDoctor.Focus();
        }

        void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow row = grid.Rows[e.RowIndex];

                    // Obtener el ID del horario seleccionado (columna oculta pero accesible)
                    if (row.Cells["HorarioID"].Value != null)
                    {
                        horarioIDSeleccionado = Convert.ToInt32(row.Cells["HorarioID"].Value);
                    }

                    // Obtener el DoctorID (columna oculta pero accesible)
                    if (row.Cells["DoctorID"].Value != null)
                    {
                        int doctorID = Convert.ToInt32(row.Cells["DoctorID"].Value);
                        cmbDoctor.SelectedValue = doctorID;
                    }

                    // Seleccionar el día
                    if (row.Cells["DiaSemana"].Value != null)
                    {
                        cmbDia.Text = row.Cells["DiaSemana"].Value.ToString();
                    }

                    // Establecer las horas
                    if (row.Cells["HoraInicio"].Value != null)
                    {
                        if (row.Cells["HoraInicio"].Value is TimeSpan)
                        {
                            dtpHoraInicio.Value = DateTime.Today.Add((TimeSpan)row.Cells["HoraInicio"].Value);
                        }
                        else
                        {
                            DateTime fechaHora = Convert.ToDateTime(row.Cells["HoraInicio"].Value);
                            dtpHoraInicio.Value = fechaHora;
                        }
                    }

                    if (row.Cells["HoraFin"].Value != null)
                    {
                        if (row.Cells["HoraFin"].Value is TimeSpan)
                        {
                            dtpHoraFin.Value = DateTime.Today.Add((TimeSpan)row.Cells["HoraFin"].Value);
                        }
                        else
                        {
                            DateTime fechaHora = Convert.ToDateTime(row.Cells["HoraFin"].Value);
                            dtpHoraFin.Value = fechaHora;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar datos de la fila: " + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void ActualizarContador()
        {
            Label lblContador = this.Controls.Find("lblContador", true)[0] as Label;
            if (lblContador != null)
            {
                int total = tablaHorarios?.Rows.Count ?? 0;
                lblContador.Text = $"{total} registros";
            }
        }

        void BtnRegresar_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 formPrincipal = new Form1(usuario, rol);
            formPrincipal.Show();
        }

        Button CrearBoton(Panel panel, string texto, int top, Color color)
        {
            Button btn = new Button();
            btn.Text = texto;
            btn.Size = new Size(110, 35);
            btn.Location = new Point(20, top);
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;

            btn.MouseEnter += (s, e) => {
                btn.BackColor = ControlPaint.Dark(color, 0.1f);
            };

            btn.MouseLeave += (s, e) => {
                btn.BackColor = color;
            };

            panel.Controls.Add(btn);
            return btn;
        }
    }
}