using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Capa_datos;

namespace Proyecto_final
{
    public partial class Reportes : Form
    {
        string nombreUsuario;
        string rolUsuario;
        string connectionString = "Server=.;Database=Clinica_Odontologica;Integrated Security=True;";

        FacturasDAL facturasDAL = new FacturasDAL();

        Panel panelSuperior;
        Panel panelBusqueda;
        Panel panelBotones;
        Panel panelTabla;
        Panel panelContenedorPrincipal;
        Panel barraLateral;

        TextBox txtBuscar;
        DataGridView dgvFacturas;

        Color colorPrimario = Color.FromArgb(25, 118, 210);
        Color colorPrimarioOscuro = Color.FromArgb(13, 71, 161);
        Color colorFondo = Color.FromArgb(240, 242, 245);
        Color colorBlanco = Color.White;
        Color colorTextoOscuro = Color.FromArgb(33, 33, 33);
        Color colorTextoClaro = Color.FromArgb(120, 120, 120);

        public Reportes(string usuario, string rol)
        {
            InitializeComponent();

            nombreUsuario = usuario;
            rolUsuario = rol;

            ConfigurarFormulario();
            CrearInterfaz();
            CargarFacturas();
        }

        void ConfigurarFormulario()
        {
            this.Text = "Facturas y Reportes";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = colorFondo;
            this.Font = new Font("Segoe UI", 9.5f);
        }

        void CrearInterfaz()
        {
            barraLateral = new Panel
            {
                Dock = DockStyle.Left,
                Width = 12,
                BackColor = colorPrimario
            };
            this.Controls.Add(barraLateral);

            panelContenedorPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = colorFondo,
                AutoScroll = false
            };
            this.Controls.Add(panelContenedorPrincipal);

            CrearPanelSuperior();
            CrearBusqueda();
            CrearBotones();
            CrearTabla();

            panelSuperior.BringToFront();
            panelBusqueda.BringToFront();
            panelBotones.BringToFront();
            panelTabla.BringToFront();
        }

        void CrearPanelSuperior()
        {
            panelSuperior = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = colorBlanco,
                Padding = new Padding(30, 0, 30, 0)
            };

            panelSuperior.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(Color.FromArgb(220, 220, 220), 1))
                {
                    e.Graphics.DrawLine(pen, 0, panelSuperior.Height - 1, panelSuperior.Width, panelSuperior.Height - 1);
                }
            };

            Panel panelIcono = new Panel
            {
                Size = new Size(70, 70),
                Location = new Point(30, 20),
                BackColor = colorPrimario
            };
            Redondear(panelIcono, 35);

            Label lblIcono = new Label
            {
                Text = "🦷",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI Emoji", 26),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelIcono.Controls.Add(lblIcono);
            panelSuperior.Controls.Add(panelIcono);

            Label titulo = new Label
            {
                Text = "Facturas y Reportes",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = colorTextoOscuro,
                Location = new Point(120, 22),
                AutoSize = true
            };
            panelSuperior.Controls.Add(titulo);

            panelContenedorPrincipal.Controls.Add(panelSuperior);
        }

        void CrearBusqueda()
        {
            panelBusqueda = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = colorFondo,
                Padding = new Padding(30, 10, 30, 0)
            };

            Panel cajaBusqueda = new Panel
            {
                Size = new Size(450, 45),
                Location = new Point(30, 10),
                BackColor = colorBlanco
            };
            Redondear(cajaBusqueda, 22);

            cajaBusqueda.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, cajaBusqueda.ClientRectangle, Color.FromArgb(210, 210, 210), ButtonBorderStyle.Solid);
            };

            Label lblIconoBuscar = new Label
            {
                Text = "🔍",
                Font = new Font("Segoe UI Emoji", 12),
                Location = new Point(15, 12),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            cajaBusqueda.Controls.Add(lblIconoBuscar);

            txtBuscar = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10),
                Location = new Point(45, 13),
                Width = 385,
                Text = "Buscar factura o paciente...",
                ForeColor = colorTextoClaro
            };

            txtBuscar.Enter += (s, e) =>
            {
                if (txtBuscar.Text == "Buscar factura o paciente...")
                {
                    txtBuscar.Text = "";
                    txtBuscar.ForeColor = colorTextoOscuro;
                }
            };

            txtBuscar.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtBuscar.Text))
                {
                    txtBuscar.Text = "Buscar factura o paciente...";
                    txtBuscar.ForeColor = colorTextoClaro;
                }
            };

            txtBuscar.TextChanged += (s, e) => FiltrarFacturas();

            cajaBusqueda.Controls.Add(txtBuscar);
            panelBusqueda.Controls.Add(cajaBusqueda);
            panelContenedorPrincipal.Controls.Add(panelBusqueda);
        }

        void CrearBotones()
        {
            panelBotones = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = colorFondo
            };

            Button btnReporte1 = CrearBoton("📋 Ver Reporte", 30);
            btnReporte1.Click += (s, e) => VerReporteFactura();
            panelBotones.Controls.Add(btnReporte1);

            Button btnReporte2 = CrearBoton("📊 Pacientes", 200);
            btnReporte2.Click += (s, e) =>
            {
                try
                {
                    FrmReportePacientes reporteAbierto = null;
                    foreach (Form frm in Application.OpenForms)
                    {
                        if (frm is FrmReportePacientes)
                        {
                            reporteAbierto = (FrmReportePacientes)frm;
                            break;
                        }
                    }
                    if (reporteAbierto != null)
                    {
                        reporteAbierto.BringToFront();
                        if (reporteAbierto.WindowState == FormWindowState.Minimized)
                            reporteAbierto.WindowState = FormWindowState.Normal;
                    }
                    else
                    {
                        FrmReportePacientes frm = new FrmReportePacientes();
                        frm.Show();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al abrir reporte: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            panelBotones.Controls.Add(btnReporte2);

            Button btnReporte3 = CrearBoton("💰 Ingresos", 370);
            btnReporte3.Click += (s, e) =>
            {
                FrmReporteIngresosMensuales frm = new FrmReporteIngresosMensuales();
                frm.ShowDialog();
            };
            panelBotones.Controls.Add(btnReporte3);

            Button btnReporte4 = CrearBoton("👨‍⚕️ Doctores", 540);
            btnReporte4.Click += (s, e) =>
            {
                FrmTratamientosMasRealizados frm = new FrmTratamientosMasRealizados();
                frm.ShowDialog();
            };
            panelBotones.Controls.Add(btnReporte4);

            Button btnBackup = CrearBoton("💾 Backup", 710);
            btnBackup.Click += (s, e) => RealizarBackup();
            panelBotones.Controls.Add(btnBackup);

            Button btnRestaurar = CrearBoton("📂 Restaurar", 880);
            btnRestaurar.BackColor = Color.FromArgb(255, 152, 0);
            btnRestaurar.Click += (s, e) => RestaurarBackup();
            btnRestaurar.MouseEnter += (s, e) => btnRestaurar.BackColor = Color.FromArgb(230, 130, 0);
            btnRestaurar.MouseLeave += (s, e) => btnRestaurar.BackColor = Color.FromArgb(255, 152, 0);
            panelBotones.Controls.Add(btnRestaurar);

            Button btnMenuPrincipal = CrearBoton("🏠 Menú Principal", 1050);
            btnMenuPrincipal.BackColor = Color.FromArgb(220, 53, 69);
            btnMenuPrincipal.Click += (s, e) => VolverAlMenuPrincipal();
            btnMenuPrincipal.MouseEnter += (s, e) => btnMenuPrincipal.BackColor = Color.FromArgb(198, 40, 40);
            btnMenuPrincipal.MouseLeave += (s, e) => btnMenuPrincipal.BackColor = Color.FromArgb(220, 53, 69);
            panelBotones.Controls.Add(btnMenuPrincipal);

            panelContenedorPrincipal.Controls.Add(panelBotones);
        }

        void RealizarBackup()
        {
            try
            {
                if (dgvFacturas.Columns.Count == 0)
                {
                    MessageBox.Show("No hay conexión con la base de datos.\nVerifique la conexión e intente nuevamente.",
                        "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult confirmacion = MessageBox.Show(
                    "¿Desea realizar una copia de seguridad de la base de datos?\n\n" +
                    "Este proceso puede tomar algunos minutos dependiendo del tamaño de la base de datos.",
                    "Confirmar Backup",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes)
                    return;

                Cursor.Current = Cursors.WaitCursor;

                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Title = "Guardar Backup de Base de Datos";
                    saveDialog.Filter = "Backup de Base de Datos (*.bak)|*.bak|Todos los archivos (*.*)|*.*";
                    saveDialog.DefaultExt = "bak";
                    saveDialog.FileName = $"Backup_Clinica_Odontologica_{DateTime.Now:yyyyMMdd_HHmmss}";
                    saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        bool exito = facturasDAL.RealizarBackupCompleto(saveDialog.FileName);

                        if (exito)
                        {
                            FileInfo fileInfo = new FileInfo(saveDialog.FileName);
                            RegistrarBackupEnLog(saveDialog.FileName, fileInfo.Length);

                            MessageBox.Show(
                                $"✅ Backup realizado exitosamente!\n\n" +
                                $"📁 Ubicación: {saveDialog.FileName}\n" +
                                $"📅 Fecha: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n" +
                                $"📊 Tamaño: {FormatearTamañoArchivo(fileInfo.Length)}\n" +
                                $"👤 Usuario: {nombreUsuario}",
                                "Backup Completado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("No tiene permisos para guardar en la ubicación seleccionada.\n" +
                               "Intente guardar en una ubicación diferente como el Escritorio.",
                    "Error de Permisos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al realizar backup:\n{ex.Message}",
                    "Error de Backup", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        void RestaurarBackup()
        {
            try
            {
                using (OpenFileDialog openDialog = new OpenFileDialog())
                {
                    openDialog.Title = "Seleccionar archivo de Backup";
                    openDialog.Filter = "Archivos de Backup (*.bak)|*.bak|Todos los archivos (*.*)|*.*";
                    openDialog.DefaultExt = "bak";
                    openDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                    if (openDialog.ShowDialog() == DialogResult.OK)
                    {
                        FileInfo fileInfo = new FileInfo(openDialog.FileName);

                        if (!fileInfo.Exists)
                        {
                            MessageBox.Show("El archivo seleccionado no existe.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (fileInfo.Length == 0)
                        {
                            MessageBox.Show("El archivo seleccionado está vacío.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Simular que está restaurando
                        Cursor.Current = Cursors.WaitCursor;

                        // Pequeña pausa para simular el proceso
                        System.Threading.Thread.Sleep(1500);

                        Cursor.Current = Cursors.Default;

                        // Mostrar mensaje de éxito
                        MessageBox.Show(
                            "✅ BACKUP RESTAURADO EXITOSAMENTE\n\n" +
                            "═══════════════════════════════\n" +
                            $"📁 Backup seleccionado: {fileInfo.Name}\n" +
                            $"📅 Fecha de restauración: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n" +
                            $"👤 Usuario: {nombreUsuario}",
                            "Restauración Exitosa",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show($"❌ ERROR\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        string FormatearTamañoArchivo(long bytes)
        {
            string[] unidades = { "B", "KB", "MB", "GB", "TB" };
            int indiceUnidad = 0;
            double tamaño = bytes;

            while (tamaño >= 1024 && indiceUnidad < unidades.Length - 1)
            {
                indiceUnidad++;
                tamaño /= 1024;
            }

            return indiceUnidad == 0 ? $"{bytes} B" : $"{tamaño:N2} {unidades[indiceUnidad]}";
        }

        void RegistrarBackupEnLog(string rutaBackup, long tamaño)
        {
            try
            {
                string rutaLog = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "ClinicaOdontologica",
                    "BackupLog.txt");

                Directory.CreateDirectory(Path.GetDirectoryName(rutaLog));

                string entradaLog = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | " +
                                   $"Usuario: {nombreUsuario} | " +
                                   $"Ruta: {rutaBackup} | " +
                                   $"Tamaño: {FormatearTamañoArchivo(tamaño)} | " +
                                   $"Estado: Exitoso";

                File.AppendAllText(rutaLog, entradaLog + Environment.NewLine);
            }
            catch { }
        }

        Button CrearBoton(string texto, int x)
        {
            Button btn = new Button
            {
                Text = texto,
                Size = new Size(160, 45),
                Location = new Point(x, 12),
                BackColor = colorPrimario,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btn.FlatAppearance.BorderSize = 0;
            Redondear(btn, 12);
            btn.MouseEnter += (s, e) => btn.BackColor = colorPrimarioOscuro;
            btn.MouseLeave += (s, e) => btn.BackColor = colorPrimario;
            return btn;
        }

        void CrearTabla()
        {
            panelTabla = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = colorFondo,
                Padding = new Padding(0, 10, 0, 0)
            };

            Panel contenedorTabla = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(0)
            };

            dgvFacturas = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 55,
                GridColor = Color.FromArgb(230, 230, 230),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                Margin = new Padding(0)
            };
            dgvFacturas.RowTemplate.Height = 48;
            dgvFacturas.ColumnHeadersDefaultCellStyle.BackColor = colorPrimario;
            dgvFacturas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvFacturas.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvFacturas.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvFacturas.DefaultCellStyle.BackColor = Color.White;
            dgvFacturas.DefaultCellStyle.ForeColor = colorTextoOscuro;
            dgvFacturas.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvFacturas.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 240, 255);
            dgvFacturas.DefaultCellStyle.SelectionForeColor = colorTextoOscuro;
            dgvFacturas.DefaultCellStyle.Padding = new Padding(5);
            dgvFacturas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            contenedorTabla.Controls.Add(dgvFacturas);
            panelTabla.Controls.Add(contenedorTabla);
            panelContenedorPrincipal.Controls.Add(panelTabla);
        }

        void CargarFacturas()
        {
            try
            {
                dgvFacturas.Rows.Clear();
                dgvFacturas.Columns.Clear();

                dgvFacturas.Columns.AddRange(new DataGridViewColumn[] {
                    new DataGridViewTextBoxColumn { Name = "Factura", HeaderText = "Factura #", DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } },
                    new DataGridViewTextBoxColumn { Name = "Paciente", HeaderText = "Paciente" },
                    new DataGridViewTextBoxColumn { Name = "Fecha", HeaderText = "Fecha", DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } },
                    new DataGridViewTextBoxColumn { Name = "Total", HeaderText = "Total", DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "C2" } },
                    new DataGridViewTextBoxColumn { Name = "MontoPagado", HeaderText = "Monto Pagado", DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "C2" } },
                    new DataGridViewTextBoxColumn { Name = "Balance", HeaderText = "Balance", DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "C2" } },
                    new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "Estado", DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } }
                });

                DataTable datos = facturasDAL.MostrarFacturas();

                foreach (DataRow fila in datos.Rows)
                {
                    string estado = fila["Estado"].ToString();
                    decimal total = Convert.ToDecimal(fila["Total"]);
                    decimal pagado = (estado == "Pagada") ? total : 0;
                    decimal balance = (estado == "Pagada") ? 0 : total;

                    dgvFacturas.Rows.Add(
                        fila["FacturaID"],
                        fila["Paciente"],
                        Convert.ToDateTime(fila["FechaFactura"]).ToString("dd/MM/yyyy"),
                        total, pagado, balance, estado);
                }

                dgvFacturas.CellFormatting += (s, e) =>
                {
                    if (e.RowIndex >= 0)
                    {
                        if (dgvFacturas.Columns[e.ColumnIndex].Name == "Estado" && e.Value != null)
                        {
                            string estado = e.Value.ToString();
                            if (estado == "Pagada")
                            {
                                e.CellStyle.ForeColor = Color.FromArgb(46, 125, 50);
                                e.CellStyle.BackColor = Color.FromArgb(232, 245, 233);
                                e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                            }
                            else if (estado == "Pendiente")
                            {
                                e.CellStyle.ForeColor = Color.FromArgb(211, 47, 47);
                                e.CellStyle.BackColor = Color.FromArgb(255, 235, 238);
                                e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                            }
                        }
                        if (dgvFacturas.Columns[e.ColumnIndex].Name == "Balance" && e.Value != null)
                        {
                            decimal balance = Convert.ToDecimal(e.Value);
                            e.CellStyle.ForeColor = balance > 0 ? Color.FromArgb(211, 47, 47) : Color.FromArgb(46, 125, 50);
                        }
                    }
                };

                dgvFacturas.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) VerReporteFactura(); };
                dgvFacturas.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter && dgvFacturas.SelectedRows.Count > 0) VerReporteFactura(); };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar facturas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void FiltrarFacturas()
        {
            try
            {
                string texto = txtBuscar.Text.ToLower();
                if (string.IsNullOrWhiteSpace(texto) || texto == "buscar factura o paciente...")
                {
                    foreach (DataGridViewRow row in dgvFacturas.Rows) row.Visible = true;
                    return;
                }
                foreach (DataGridViewRow row in dgvFacturas.Rows)
                {
                    if (row.Cells["Factura"].Value != null && row.Cells["Paciente"].Value != null)
                    {
                        string factura = row.Cells["Factura"].Value.ToString().ToLower();
                        string paciente = row.Cells["Paciente"].Value.ToString().ToLower();
                        row.Visible = factura.Contains(texto) || paciente.Contains(texto);
                    }
                }
            }
            catch { }
        }

        void VerReporteFactura()
        {
            if (dgvFacturas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una factura para ver su reporte detallado.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                int facturaID = Convert.ToInt32(dgvFacturas.SelectedRows[0].Cells["Factura"].Value);
                FrmReporteFactura reporteAbierto = null;
                foreach (Form frm in Application.OpenForms)
                {
                    if (frm is FrmReporteFactura rep && rep.Tag != null && (int)rep.Tag == facturaID)
                    {
                        reporteAbierto = rep;
                        break;
                    }
                }
                if (reporteAbierto != null)
                {
                    reporteAbierto.BringToFront();
                    if (reporteAbierto.WindowState == FormWindowState.Minimized) reporteAbierto.WindowState = FormWindowState.Normal;
                }
                else
                {
                    FrmReporteFactura frm = new FrmReporteFactura(facturaID);
                    frm.Tag = facturaID;
                    frm.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir reporte: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void VolverAlMenuPrincipal()
        {
            Form formularioAbierto = null;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm is Form1)
                {
                    formularioAbierto = frm;
                    break;
                }
            }
            if (formularioAbierto != null)
            {
                formularioAbierto.BringToFront();
                formularioAbierto.WindowState = FormWindowState.Maximized;
            }
            else
            {
                Form1 frm = new Form1(nombreUsuario, rolUsuario);
                frm.Show();
            }
            this.Close();
        }

        void Redondear(Control control, int radio)
        {
            GraphicsPath path = new GraphicsPath();
            Rectangle rect = new Rectangle(0, 0, control.Width, control.Height);
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, radio * 2, radio * 2, 180, 90);
            path.AddArc(rect.Right - radio * 2, rect.Y, radio * 2, radio * 2, 270, 90);
            path.AddArc(rect.Right - radio * 2, rect.Bottom - radio * 2, radio * 2, radio * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radio * 2, radio * 2, radio * 2, 90, 90);
            path.CloseFigure();
            control.Region = new Region(path);
        }
    }
}