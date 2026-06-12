using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Capa_datos;

namespace Proyecto_final
{
    public partial class Doctores : Form
    {
        DoctoresDAL doctoresDAL = new DoctoresDAL();
        DataTable dtDoctores;

        string nombreUsuario;
        string rolUsuario;
        int doctorIDSeleccionado = 0;

        DataGridView grid;
        TextBox txtBuscar;
        Label lblContador;
        Label lblSeleccionado;

        Color colorPrimario = Color.FromArgb(52, 73, 94);
        Color colorSecundario = Color.FromArgb(41, 128, 185);
        Color colorFondo = Color.FromArgb(236, 240, 241);
        Color colorPanel = Color.FromArgb(255, 255, 255);
        Color colorTexto = Color.FromArgb(44, 62, 80);
        Color colorBotonRegresar = Color.FromArgb(231, 76, 60);
        Color colorSeleccionado = Color.FromArgb(46, 204, 113);

        public Doctores()
        {
            InitializeComponent();
            nombreUsuario = "Usuario";
            rolUsuario = "Administrador";
            ConfigurarFormulario();
            CrearUI();
            CargarDatos();
        }

        public Doctores(string usuario, string rol)
        {
            InitializeComponent();
            nombreUsuario = usuario;
            rolUsuario = rol;
            ConfigurarFormulario();
            CrearUI();
            CargarDatos();
        }

        void ConfigurarFormulario()
        {
            this.Text = "Modulo de Doctores - Centro Odontologico";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = colorFondo;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9);
        }

        void CrearUI()
        {
            Panel header = new Panel();
            header.Size = new Size(this.Width, 70);
            header.Location = new Point(0, 0);
            header.BackColor = colorPrimario;
            header.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.Controls.Add(header);

            Label titulo = new Label();
            titulo.Text = "Gestion de Doctores";
            titulo.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            titulo.ForeColor = Color.White;
            titulo.Location = new Point(25, 15);
            titulo.AutoSize = true;
            header.Controls.Add(titulo);

            Label subtitulo = new Label();
            subtitulo.Text = $"Usuario: {nombreUsuario} | Rol: {rolUsuario}";
            subtitulo.Font = new Font("Segoe UI", 10);
            subtitulo.ForeColor = Color.FromArgb(189, 195, 199);
            subtitulo.Location = new Point(25, 48);
            subtitulo.AutoSize = true;
            header.Controls.Add(subtitulo);

            Button btnRegresar = new Button();
            btnRegresar.Text = "Volver al Menu Principal";
            btnRegresar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnRegresar.Size = new Size(200, 40);
            btnRegresar.Location = new Point(this.Width - 230, 15);
            btnRegresar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRegresar.BackColor = colorBotonRegresar;
            btnRegresar.ForeColor = Color.White;
            btnRegresar.FlatStyle = FlatStyle.Flat;
            btnRegresar.FlatAppearance.BorderSize = 0;
            btnRegresar.Cursor = Cursors.Hand;
            btnRegresar.Click += (s, e) => {
                this.Hide();
                Form1 formPrincipal = new Form1(nombreUsuario, rolUsuario);
                formPrincipal.Show();
            };
            header.Controls.Add(btnRegresar);

            Panel panelIzquierdo = new Panel();
            panelIzquierdo.Size = new Size(380, this.Height - 110);
            panelIzquierdo.Location = new Point(25, 95);
            panelIzquierdo.BackColor = colorPanel;
            panelIzquierdo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(panelIzquierdo);

            Label lblBuscar = new Label();
            lblBuscar.Text = "Buscar Doctor:";
            lblBuscar.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblBuscar.ForeColor = colorPrimario;
            lblBuscar.Location = new Point(15, 15);
            lblBuscar.AutoSize = true;
            panelIzquierdo.Controls.Add(lblBuscar);

            txtBuscar = new TextBox();
            txtBuscar.Size = new Size(350, 28);
            txtBuscar.Location = new Point(15, 40);
            txtBuscar.Font = new Font("Segoe UI", 10);
            txtBuscar.BackColor = Color.FromArgb(248, 249, 250);
            txtBuscar.BorderStyle = BorderStyle.FixedSingle;
            txtBuscar.TextChanged += (s, e) => FiltrarDoctores();
            panelIzquierdo.Controls.Add(txtBuscar);

            lblSeleccionado = new Label();
            lblSeleccionado.Text = "Ningun doctor seleccionado";
            lblSeleccionado.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblSeleccionado.ForeColor = Color.FromArgb(231, 76, 60);
            lblSeleccionado.Location = new Point(15, 80);
            lblSeleccionado.AutoSize = true;
            panelIzquierdo.Controls.Add(lblSeleccionado);

            Button btnNuevo = new Button();
            btnNuevo.Text = "Nuevo Doctor";
            btnNuevo.Size = new Size(160, 38);
            btnNuevo.Location = new Point(15, 115);
            btnNuevo.BackColor = Color.FromArgb(39, 174, 96);
            btnNuevo.ForeColor = Color.White;
            btnNuevo.FlatStyle = FlatStyle.Flat;
            btnNuevo.FlatAppearance.BorderSize = 0;
            btnNuevo.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnNuevo.Cursor = Cursors.Hand;
            btnNuevo.Click += (s, e) => NuevoDoctor();
            panelIzquierdo.Controls.Add(btnNuevo);

            Button btnEditar = new Button();
            btnEditar.Text = "Editar Doctor";
            btnEditar.Size = new Size(160, 38);
            btnEditar.Location = new Point(190, 115);
            btnEditar.BackColor = colorSecundario;
            btnEditar.ForeColor = Color.White;
            btnEditar.FlatStyle = FlatStyle.Flat;
            btnEditar.FlatAppearance.BorderSize = 0;
            btnEditar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnEditar.Cursor = Cursors.Hand;
            btnEditar.Click += (s, e) => EditarDoctor();
            panelIzquierdo.Controls.Add(btnEditar);

            Button btnEliminar = new Button();
            btnEliminar.Text = "Eliminar Doctor";
            btnEliminar.Size = new Size(350, 38);
            btnEliminar.Location = new Point(15, 165);
            btnEliminar.BackColor = Color.FromArgb(231, 76, 60);
            btnEliminar.ForeColor = Color.White;
            btnEliminar.FlatStyle = FlatStyle.Flat;
            btnEliminar.FlatAppearance.BorderSize = 0;
            btnEliminar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnEliminar.Cursor = Cursors.Hand;
            btnEliminar.Click += (s, e) => EliminarDoctor();
            panelIzquierdo.Controls.Add(btnEliminar);

            Label lblStats = new Label();
            lblStats.Text = "Estadisticas";
            lblStats.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblStats.ForeColor = colorPrimario;
            lblStats.Location = new Point(15, 225);
            lblStats.AutoSize = true;
            panelIzquierdo.Controls.Add(lblStats);

            CrearMiniCard(panelIzquierdo, "Total", "0", 255, Color.FromArgb(52, 73, 94));
            CrearMiniCard(panelIzquierdo, "Activos", "0", 290, Color.FromArgb(39, 174, 96));
            CrearMiniCard(panelIzquierdo, "Especialidades", "0", 325, Color.FromArgb(41, 128, 185));
          
            Label lblGrid = new Label();
            lblGrid.Text = "Doctores Registrados";
            lblGrid.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblGrid.ForeColor = colorPrimario;
            lblGrid.Location = new Point(425, 115); // Antes: 95, Ahora: 115
            lblGrid.AutoSize = true;
            this.Controls.Add(lblGrid);

            // Contador - BAJADO 20px
            lblContador = new Label();
            lblContador.Text = "0 registros encontrados";
            lblContador.Font = new Font("Segoe UI", 9);
            lblContador.ForeColor = Color.FromArgb(127, 140, 141);
            lblContador.Location = new Point(425, 140); // Antes: 120, Ahora: 140
            lblContador.AutoSize = true;
            this.Controls.Add(lblContador);

            // Grid - BAJADO 20px y más alto
            grid = new DataGridView();
            grid.Location = new Point(425, 165); // Antes: 145, Ahora: 165
            grid.Size = new Size(this.Width - 460, this.Height - 210); // Antes: -190, Ahora: -210
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.RowHeadersVisible = false;
            grid.MultiSelect = false;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = colorPrimario;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            grid.ColumnHeadersHeight = 40;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            grid.DefaultCellStyle.ForeColor = colorTexto;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            grid.RowTemplate.Height = 32;
            grid.CellClick += Grid_CellClick;
            grid.SelectionChanged += Grid_SelectionChanged;
            this.Controls.Add(grid);
        }

        void CrearMiniCard(Panel panel, string titulo, string valor, int top, Color color)
        {
            Panel card = new Panel();
            card.Size = new Size(350, 28);
            card.Location = new Point(15, top);
            card.BackColor = Color.FromArgb(248, 249, 250);
            panel.Controls.Add(card);

            Label lblTitulo = new Label();
            lblTitulo.Text = titulo;
            lblTitulo.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(100, 100, 100);
            lblTitulo.Location = new Point(10, 5);
            lblTitulo.AutoSize = true;
            card.Controls.Add(lblTitulo);

            Label lblValor = new Label();
            lblValor.Text = valor;
            lblValor.Name = "lblValor";
            lblValor.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblValor.ForeColor = color;
            lblValor.Location = new Point(250, 3);
            lblValor.AutoSize = true;
            lblValor.TextAlign = ContentAlignment.MiddleRight;
            card.Controls.Add(lblValor);
        }

        void ActualizarIndicadorSeleccion()
        {
            if (doctorIDSeleccionado > 0 && dtDoctores != null)
            {
                string colNombre = null;
                if (dtDoctores.Columns.Contains("Nombre")) colNombre = "Nombre";
                else if (dtDoctores.Columns.Contains("Nombres")) colNombre = "Nombres";
                else if (dtDoctores.Columns.Contains("NombreCompleto")) colNombre = "NombreCompleto";

                string colID = null;
                if (dtDoctores.Columns.Contains("DoctorID")) colID = "DoctorID";
                else if (dtDoctores.Columns.Contains("ID")) colID = "ID";

                if (colID != null && colNombre != null)
                {
                    DataRow[] rows = dtDoctores.Select($"{colID} = {doctorIDSeleccionado}");
                    if (rows.Length > 0)
                    {
                        string nombre = rows[0][colNombre].ToString();
                        lblSeleccionado.Text = $"Seleccionado: {nombre}";
                        lblSeleccionado.ForeColor = colorSeleccionado;
                        return;
                    }
                }
            }

            lblSeleccionado.Text = "Ningun doctor seleccionado";
            lblSeleccionado.ForeColor = Color.FromArgb(231, 76, 60);
        }

        void Grid_SelectionChanged(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count > 0)
            {
                DataGridViewRow row = grid.SelectedRows[0];
                foreach (DataGridViewColumn col in grid.Columns)
                {
                    if (col.Name.ToLower().Contains("id") && row.Cells[col.Name].Value != null)
                    {
                        doctorIDSeleccionado = Convert.ToInt32(row.Cells[col.Name].Value);
                        ActualizarIndicadorSeleccion();
                        break;
                    }
                }
            }
        }

        void NuevoDoctor()
        {
            Form f = new Form
            {
                Text = "Nuevo Doctor",
                Size = new Size(420, 370),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Panel header = new Panel { Dock = DockStyle.Top, Height = 45, BackColor = colorPrimario };
            Label titulo = new Label
            {
                Text = "REGISTRAR NUEVO DOCTOR",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 12)
            };
            header.Controls.Add(titulo);
            f.Controls.Add(header);

            Panel body = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15), BackColor = Color.White };
            f.Controls.Add(body);

            int y = 60;
            int spacing = 55;

            // Nombre Completo
            Label lblNombre = new Label { Text = "Nombre Completo:", Location = new Point(10, y), Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = colorPrimario, AutoSize = true };
            TextBox txtNombre = new TextBox { Location = new Point(10, y + 20), Width = 375, Height = 26, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(248, 249, 250) };
            body.Controls.Add(lblNombre);
            body.Controls.Add(txtNombre);
            y += spacing;

            // Especialidad
            Label lblEspecialidad = new Label { Text = "Especialidad:", Location = new Point(10, y), Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = colorPrimario, AutoSize = true };
            TextBox txtEspecialidad = new TextBox { Location = new Point(10, y + 20), Width = 375, Height = 26, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(248, 249, 250) };
            body.Controls.Add(lblEspecialidad);
            body.Controls.Add(txtEspecialidad);
            y += spacing;

            // Telefono
            Label lblTelefono = new Label { Text = "Telefono:", Location = new Point(10, y), Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = colorPrimario, AutoSize = true };
            TextBox txtTelefono = new TextBox { Location = new Point(10, y + 20), Width = 375, Height = 26, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(248, 249, 250), MaxLength = 12 };
            txtTelefono.KeyPress += SoloNumerosYGion;
            txtTelefono.TextChanged += FormatoTelefonoCompleto;
            body.Controls.Add(lblTelefono);
            body.Controls.Add(txtTelefono);
            y += spacing + 15;

            // Botones
            Button btnGuardar = new Button
            {
                Text = "Guardar",
                Location = new Point(10, y),
                Width = 180,
                Height = 36,
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(205, y),
                Width = 180,
                Height = 36,
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnGuardar.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    string.IsNullOrWhiteSpace(txtEspecialidad.Text))
                {
                    MessageBox.Show("Complete los campos obligatorios.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    bool resultado = doctoresDAL.InsertarDoctor(
                        txtNombre.Text.Trim(),
                        txtEspecialidad.Text.Trim(),
                        txtTelefono.Text.Trim()
                    );

                    if (resultado)
                    {
                        MessageBox.Show("Doctor registrado exitosamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarDatos();
                        f.Close();
                    }
                    else
                    {
                        MessageBox.Show("Error al guardar el doctor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnCancelar.Click += (s, e) => f.Close();

            body.Controls.AddRange(new Control[] { btnGuardar, btnCancelar });
            f.ShowDialog(this);
        }

        void EditarDoctor()
        {
            if (doctorIDSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un doctor en la tabla.", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (dtDoctores == null) return;

            string colID = null;
            foreach (DataColumn col in dtDoctores.Columns)
            {
                if (col.ColumnName.ToLower().Contains("id"))
                {
                    colID = col.ColumnName;
                    break;
                }
            }
            if (colID == null) colID = dtDoctores.Columns[0].ColumnName;

            string colNombre = null;
            foreach (DataColumn col in dtDoctores.Columns)
            {
                string nombreLower = col.ColumnName.ToLower();
                if (nombreLower.Contains("nombre") || nombreLower.Contains("nombres"))
                {
                    colNombre = col.ColumnName;
                    break;
                }
            }
            if (colNombre == null && dtDoctores.Columns.Count > 1)
                colNombre = dtDoctores.Columns[1].ColumnName;

            string colEspecialidad = null;
            foreach (DataColumn col in dtDoctores.Columns)
            {
                if (col.ColumnName.ToLower().Contains("especialidad"))
                {
                    colEspecialidad = col.ColumnName;
                    break;
                }
            }

            string colTelefono = null;
            foreach (DataColumn col in dtDoctores.Columns)
            {
                if (col.ColumnName.ToLower().Contains("telefono") || col.ColumnName.ToLower().Contains("fono"))
                {
                    colTelefono = col.ColumnName;
                    break;
                }
            }

            DataRow[] rows = dtDoctores.Select($"{colID} = {doctorIDSeleccionado}");
            if (rows.Length == 0)
            {
                MessageBox.Show("No se encontró el doctor seleccionado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataRow doctor = rows[0];

            Form f = new Form
            {
                Text = "Editar Doctor",
                Size = new Size(420, 370),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Panel header = new Panel { Dock = DockStyle.Top, Height = 45, BackColor = colorSecundario };
            Label titulo = new Label { Text = "EDITAR DOCTOR", ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true, Location = new Point(15, 12) };
            header.Controls.Add(titulo);
            f.Controls.Add(header);

            Panel body = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15), BackColor = Color.White };
            f.Controls.Add(body);
            int y = 60;
            int spacing = 55;

            // Nombre Completo
            Label lblNombre = new Label { Text = "Nombre Completo:", Location = new Point(10, y), Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = colorPrimario, AutoSize = true };
            TextBox txtNombre = new TextBox { Text = colNombre != null ? (doctor[colNombre]?.ToString() ?? "") : "", Location = new Point(10, y + 20), Width = 375, Height = 26, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(248, 249, 250) };
            body.Controls.Add(lblNombre);
            body.Controls.Add(txtNombre);
            y += spacing;

            // Especialidad
            Label lblEspecialidad = new Label { Text = "Especialidad:", Location = new Point(10, y), Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = colorPrimario, AutoSize = true };
            TextBox txtEspecialidad = new TextBox { Text = colEspecialidad != null ? (doctor[colEspecialidad]?.ToString() ?? "") : "", Location = new Point(10, y + 20), Width = 375, Height = 26, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(248, 249, 250) };
            body.Controls.Add(lblEspecialidad);
            body.Controls.Add(txtEspecialidad);
            y += spacing;

            // Telefono
            Label lblTelefono = new Label { Text = "Telefono:", Location = new Point(10, y), Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = colorPrimario, AutoSize = true };
            TextBox txtTelefono = new TextBox { Text = colTelefono != null ? (doctor[colTelefono]?.ToString() ?? "") : "", Location = new Point(10, y + 20), Width = 375, Height = 26, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(248, 249, 250), MaxLength = 12 };
            txtTelefono.KeyPress += SoloNumerosYGion;
            txtTelefono.TextChanged += FormatoTelefonoCompleto;
            body.Controls.Add(lblTelefono);
            body.Controls.Add(txtTelefono);
            y += spacing + 15;

            // Botones
            Button btnActualizar = new Button
            {
                Text = "Actualizar",
                Location = new Point(10, y),
                Width = 180,
                Height = 36,
                BackColor = colorSecundario,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(205, y),
                Width = 180,
                Height = 36,
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnActualizar.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtEspecialidad.Text))
                {
                    MessageBox.Show("Complete los campos obligatorios.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    bool resultado = doctoresDAL.EditarDoctor(
                        doctorIDSeleccionado,
                        txtNombre.Text.Trim(),
                        txtEspecialidad.Text.Trim(),
                        txtTelefono.Text.Trim()
                    );

                    if (resultado)
                    {
                        MessageBox.Show("Doctor actualizado exitosamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarDatos();
                        f.Close();
                    }
                    else
                    {
                        MessageBox.Show("Error al actualizar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnCancelar.Click += (s, e) => f.Close();

            body.Controls.AddRange(new Control[] { btnActualizar, btnCancelar });
            f.ShowDialog(this);
        }

        void EliminarDoctor()
        {
            if (doctorIDSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un doctor.", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show("Esta seguro de eliminar este doctor?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                bool eliminado = doctoresDAL.EliminarDoctor(doctorIDSeleccionado);

                if (eliminado)
                {
                    MessageBox.Show("Doctor eliminado.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    doctorIDSeleccionado = 0;
                    CargarDatos();
                }
                else
                {
                    MessageBox.Show("Error al eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void CargarDatos()
        {
            try
            {
                dtDoctores = doctoresDAL.MostrarDoctores();

                if (dtDoctores == null || dtDoctores.Rows.Count == 0)
                {
                    grid.DataSource = null;
                    lblContador.Text = "0 registros encontrados";
                    ActualizarEstadisticas();
                    return;
                }

                grid.DataSource = null;
                grid.DataSource = dtDoctores;

                foreach (DataGridViewColumn col in grid.Columns)
                {
                    col.Visible = false;
                }

                if (grid.Columns.Contains("Nombres"))
                {
                    grid.Columns["Nombres"].Visible = true;
                    grid.Columns["Nombres"].HeaderText = "Nombre Completo";
                }

                if (grid.Columns.Contains("Especialidad"))
                {
                    grid.Columns["Especialidad"].Visible = true;
                    grid.Columns["Especialidad"].HeaderText = "Especialidad";
                }

                if (grid.Columns.Contains("Telefono"))
                {
                    grid.Columns["Telefono"].Visible = true;
                    grid.Columns["Telefono"].HeaderText = "Teléfono";
                }

                ActualizarEstadisticas();
                ActualizarIndicadorSeleccion();
                lblContador.Text = $"{dtDoctores.Rows.Count} registros encontrados";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar doctores: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void FiltrarDoctores()
        {
            if (dtDoctores == null) return;

            string filtro = txtBuscar.Text.Trim();

            if (string.IsNullOrEmpty(filtro))
            {
                grid.DataSource = dtDoctores;
                lblContador.Text = $"{dtDoctores.Rows.Count} registros encontrados";
            }
            else
            {
                List<string> condiciones = new List<string>();
                foreach (DataColumn col in dtDoctores.Columns)
                {
                    if (col.DataType == typeof(string))
                        condiciones.Add($"{col.ColumnName} LIKE '%{filtro}%'");
                }

                if (condiciones.Count > 0)
                {
                    DataView dv = dtDoctores.DefaultView;
                    dv.RowFilter = string.Join(" OR ", condiciones);
                    grid.DataSource = dv;
                    lblContador.Text = $"{dv.Count} registros encontrados";
                }
            }

            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (col.Name.ToLower().Contains("id"))
                    col.Visible = false;
            }
        }

        void ActualizarEstadisticas()
        {
            if (dtDoctores == null || dtDoctores.Columns.Count == 0)
            {
                ActualizarCardEstadistica(0, "0");
                ActualizarCardEstadistica(1, "0");
                ActualizarCardEstadistica(2, "0");
                ActualizarCardEstadistica(3, "0");
                return;
            }

            int total = dtDoctores.Rows.Count;
            int activos = total;
            var especialidades = new HashSet<string>();

            string colEspecialidad = null;
            foreach (DataColumn col in dtDoctores.Columns)
            {
                if (col.ColumnName.ToLower().Contains("especialidad"))
                {
                    colEspecialidad = col.ColumnName;
                    break;
                }
            }

            foreach (DataRow row in dtDoctores.Rows)
            {
                if (colEspecialidad != null && row[colEspecialidad] != DBNull.Value)
                    especialidades.Add(row[colEspecialidad].ToString());
            }

            ActualizarCardEstadistica(0, total.ToString());
            ActualizarCardEstadistica(1, activos.ToString());
            ActualizarCardEstadistica(2, especialidades.Count.ToString());
            ActualizarCardEstadistica(3, "0");
        }

        void ActualizarCardEstadistica(int index, string valor)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Panel && control.Width == 380)
                {
                    Panel panelIzquierdo = (Panel)control;
                    int cardIndex = 0;
                    foreach (Control card in panelIzquierdo.Controls)
                    {
                        if (card is Panel && card.Size.Height == 28)
                        {
                            if (cardIndex == index)
                            {
                                foreach (Control label in card.Controls)
                                {
                                    if (label is Label && label.Name == "lblValor")
                                    {
                                        label.Text = valor;
                                        return;
                                    }
                                }
                            }
                            cardIndex++;
                        }
                    }
                }
            }
        }

        void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = grid.Rows[e.RowIndex];
                foreach (DataGridViewColumn col in grid.Columns)
                {
                    if (col.Name.ToLower().Contains("id") && row.Cells[col.Name].Value != null)
                    {
                        doctorIDSeleccionado = Convert.ToInt32(row.Cells[col.Name].Value);
                        ActualizarIndicadorSeleccion();
                        break;
                    }
                }
            }
        }

        private void SoloNumerosYGion(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '-' && e.KeyChar != (char)8)
                e.Handled = true;
        }

        private void FormatoTelefonoCompleto(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            string numeros = new string(txt.Text.Where(char.IsDigit).ToArray());

            if (numeros.Length > 10) numeros = numeros.Substring(0, 10);

            string resultado = "";
            if (numeros.Length > 0)
            {
                if (numeros.Length <= 3) resultado = numeros;
                else if (numeros.Length <= 6) resultado = numeros.Substring(0, 3) + "-" + numeros.Substring(3);
                else resultado = numeros.Substring(0, 3) + "-" + numeros.Substring(3, 3) + "-" + numeros.Substring(6);
            }

            txt.TextChanged -= FormatoTelefonoCompleto;
            txt.Text = resultado;
            txt.SelectionStart = txt.Text.Length;
            txt.TextChanged += FormatoTelefonoCompleto;
        }
    }
}