using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SiparisTakip_Odev.Data
{
    public static class Theme
    {
        public static event EventHandler ThemeChanged;

        public static string Current { get; private set; } = "Light";

        public static void Set(string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            Current = name;
            ThemeChanged?.Invoke(null, EventArgs.Empty);
        }

        public static void ApplyToForm(Form f)
        {
            var colors = ColorsFor(Current);
            // If this is admin form, slightly change accent to red-ish tint for admin theme
            var colorsToUse = colors;
            if (f != null && f.Text != null && f.Text.IndexOf("Admin", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                // override accent to a red tone while keeping bg/fg
                colorsToUse = (colors.bg, colors.fg, Color.FromArgb(196, 57, 57));
            }
            f.BackColor = colorsToUse.bg;
            f.ForeColor = colorsToUse.fg;
            f.Font = new Font("Segoe UI", 9);

            ApplyToControls(f.Controls, colorsToUse);

            // Special handling for DataGridView controls
            foreach (Control c in f.Controls)
            {
                if (c is DataGridView dgv) ApplyToDataGridView(dgv, colorsToUse);
            }
        }

        private static void ApplyToControls(Control.ControlCollection controls, (Color bg, Color fg, Color accent) colors)
        {
            foreach (Control c in controls)
            {
                if (c is Button btn)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.BackColor = colors.accent;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = ControlPaint.Dark(colors.accent);
                }
                else if (c is Label) // keep label style
                {
                    c.ForeColor = colors.fg;
                }
                else
                {
                    // For dark themes prefer explicit dark background so text remains readable
                    bool isDark = colors.bg.GetBrightness() < 0.5f;
                    if (isDark)
                    {
                        // Use a slightly lighter than page background for input controls to create contrast
                        if (c is TextBox || c is ComboBox || c is NumericUpDown || c is RichTextBox)
                        {
                            c.BackColor = Color.FromArgb(60, 60, 64);
                            c.ForeColor = colors.fg;
                        }
                        else
                        {
                            c.BackColor = colors.bg;
                            c.ForeColor = colors.fg;
                        }
                    }
                    else
                    {
                        c.BackColor = ControlPaint.Light(colors.bg);
                        c.ForeColor = colors.fg;
                    }
                }

                // DataGridView handled separately
                if (c is Panel || c is GroupBox || c is TabControl || c is UserControl)
                {
                    ApplyToControls(c.Controls, colors);
                }

                if (c is DataGridView dgv) ApplyToDataGridView(dgv, colors);
            }
        }

        private static void ApplyToDataGridView(DataGridView dgv, (Color bg, Color fg, Color accent) colors)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.BackgroundColor = colors.bg;
            dgv.ForeColor = colors.fg;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = colors.accent;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.RowHeadersVisible = false;
            dgv.GridColor = ControlPaint.Dark(colors.bg);
            dgv.DefaultCellStyle.SelectionBackColor = ControlPaint.Dark(colors.accent);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            // Enhance row contrast for dark themes
            bool isDark = colors.bg.GetBrightness() < 0.5f;
            if (isDark)
            {
                dgv.DefaultCellStyle.BackColor = Color.FromArgb(45,45,48);
                dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(50,50,54);
                dgv.DefaultCellStyle.ForeColor = colors.fg;
            }
            else
            {
                dgv.DefaultCellStyle.BackColor = Color.White;
                dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250,250,250);
                dgv.DefaultCellStyle.ForeColor = colors.fg;
            }
        }

        private static (Color bg, Color fg, Color accent) ColorsFor(string name)
        {
            switch (name)
            {
                case "Dark":
                    return (Color.FromArgb(45, 45, 48), Color.WhiteSmoke, Color.FromArgb(28, 151, 234));
                case "Blue":
                    return (Color.FromArgb(240, 248, 255), Color.FromArgb(20, 33, 61), Color.FromArgb(0, 120, 215));
                case "Green":
                    return (Color.FromArgb(242, 255, 245), Color.FromArgb(15, 80, 45), Color.FromArgb(34, 153, 84));
                default:
                    // Light
                    return (Color.White, Color.FromArgb(20, 20, 20), Color.FromArgb(0, 120, 215));
            }
        }

        public static void ApplyToAllOpenForms()
        {
            foreach (Form f in Application.OpenForms.Cast<Form>().ToArray())
            {
                ApplyToForm(f);
            }
        }
    }
}
