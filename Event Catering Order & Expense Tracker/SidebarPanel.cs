using System;
using System.Drawing;
using System.Windows.Forms;

namespace Event_Catering_Order___Expense_Tracker
{
    public partial class SidebarPanel : UserControl
    {
        // Colors
        //private Color originalSidebarLabelForeColor = Color.FromArgb(241, 234, 218);
        private Color originalSidebarLabelForeColor = Color.White;
        private Color hoverSidebarLabelForeColor = Color.White;
        private Color activePanelColor = Color.FromArgb(170, 163, 150);
        private Color inactivePanelColor = Color.FromArgb(206, 193, 168);

        // Panels
        private Panel dashboardPanel;
        private Panel calendarPanel;
        private Panel spreadsheetsPanel;
        private Panel addNewPanel;

        // Logout button
        private Button logoutButton;

        // Current active panel
        private Panel activePanel;

        private string currentPanelName = "Dashboard";

        public event EventHandler<Form> NavigationRequested;

        public SidebarPanel(string activePanel = "Dashboard")
        {
            InitializeComponent();
            this.BackColor = Color.FromArgb(206, 193, 168);
            this.Dock = DockStyle.Left;
            this.Size = new Size(173, 561);
            currentPanelName = activePanel;
            InitializeSidebar();
        }

        private void InitializeSidebar()
        {
            // Create panels
            dashboardPanel = CreatePanel("Dashboard", 0);
            calendarPanel = CreatePanel("Calendar", 1);
            spreadsheetsPanel = CreatePanel("Spreadsheets", 2);
            addNewPanel = CreatePanel("Add New", 3);

            // Create logout button
            logoutButton = new Button
            {
                Text = "Log Out",
                BackColor = activePanelColor,
                ForeColor = Color.White,
                Font = new Font("Calibri", 10),
                Size = new Size(151, 29),
                Location = new Point(11, 521),
                FlatStyle = FlatStyle.Flat
            };
            logoutButton.Click += LogoutButton_Click;

            // Add controls to the sidebar
            Controls.AddRange(new Control[] { dashboardPanel, calendarPanel, spreadsheetsPanel, addNewPanel, logoutButton });

            // Set initial active panel based on currentPanelName
            switch (currentPanelName)
            {
                case "Dashboard": SetActivePanel(dashboardPanel); break;
                case "Calendar": SetActivePanel(calendarPanel); break;
                case "Spreadsheets": SetActivePanel(spreadsheetsPanel); break;
                case "Add New": SetActivePanel(addNewPanel); break;
                default: SetActivePanel(dashboardPanel); break;
            }
        }

        private Panel CreatePanel(string text, int index)
        {
            var panel = new Panel
            {
                Size = new Size(173, 40),
                Location = new Point(0, index * 43),
                BackColor = inactivePanelColor,
                Cursor = Cursors.Hand
            };

            var icon = new PictureBox
            {
                Size = new Size(25, 19),
                Location = new Point(22, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = GetIconForPanel(text)
            };

            var label = new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Calibri Light", 12),
                ForeColor = originalSidebarLabelForeColor,
                Location = new Point(52, 10),
                Cursor = Cursors.Hand
            };

            // Add icon and label to the panel
            panel.Controls.Add(icon);
            panel.Controls.Add(label);

            // Attach events to the panel (not just the label)
            panel.Click += (s, e) => Panel_Click(panel, text);
            panel.MouseEnter += (s, e) => Panel_MouseEnter(panel);
            panel.MouseLeave += (s, e) => Panel_MouseLeave(panel);
            // Also forward events from child controls to the parent panel
            icon.Click += (s, e) => Panel_Click(panel, text);
            label.Click += (s, e) => Panel_Click(panel, text);
            icon.MouseEnter += (s, e) => Panel_MouseEnter(panel);
            label.MouseEnter += (s, e) => Panel_MouseEnter(panel);
            icon.MouseLeave += (s, e) => Panel_MouseLeave(panel);
            label.MouseLeave += (s, e) => Panel_MouseLeave(panel);

            return panel;
        }

        private void Panel_MouseEnter(Panel panel)
        {
            if (panel != activePanel)
            {
                panel.BackColor = activePanelColor;
                foreach (Control ctrl in panel.Controls)
                {
                    if (ctrl is Label lbl) lbl.ForeColor = Color.White;
                }
            }
        }

        private void Panel_MouseLeave(Panel panel)
        {
            if (panel != activePanel)
            {
                panel.BackColor = inactivePanelColor;
                foreach (Control ctrl in panel.Controls)
                {
                    if (ctrl is Label lbl) lbl.ForeColor = Color.White;
                }
            }
        }

        private Image GetIconForPanel(string panelName)
        {
            // You'll need to add these resources to your project
            switch (panelName)
            {
                case "Dashboard":
                    return Properties.Resources.Untitled_design_removebg_preview;
                case "Calendar":
                    return Properties.Resources.Untitled_design__1__removebg_preview;
                case "Spreadsheets":
                    return Properties.Resources.Untitled_design__2__removebg_preview;
                case "Add New":
                    return Properties.Resources.Untitled_design__3__removebg_preview;
                default:
                    return null;
            }
        }

        private void Panel_Click(Panel panel, string panelName)
        {
            if (panel == activePanel) return;

            SetActivePanel(panel);

            Form formToOpen = null;
            switch (panelName)
            {
                case "Dashboard":
                    formToOpen = new Home();
                    break;
                case "Calendar":
                    formToOpen = new Calendar();
                    break;
                case "Spreadsheets":
                    formToOpen = new Spreadsheet();
                    break;
                case "Add New":
                    formToOpen = new AddNew();
                    break;
            }

            if (formToOpen != null)
            {
                NavigationRequested?.Invoke(this, formToOpen);
            }
        }

        private void SetActivePanel(Panel panel)
        {
            if (activePanel != null)
            {
                activePanel.BackColor = inactivePanelColor;
                foreach (Control ctrl in activePanel.Controls)
                {
                    if (ctrl is Label lbl) lbl.ForeColor = Color.White;
                }
            }

            activePanel = panel;
            activePanel.BackColor = activePanelColor;
            foreach (Control ctrl in activePanel.Controls)
            {
                if (ctrl is Label lbl) lbl.ForeColor = Color.White;
            }
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            // Only use fade animation for logout
            NavigationRequested?.Invoke(this, new Login());
        }

        public void SetActivePanelByName(string panelName)
        {
            currentPanelName = panelName;
            switch (panelName)
            {
                case "Dashboard": SetActivePanel(dashboardPanel); break;
                case "Calendar": SetActivePanel(calendarPanel); break;
                case "Spreadsheets": SetActivePanel(spreadsheetsPanel); break;
                case "Add New": SetActivePanel(addNewPanel); break;
                default: SetActivePanel(dashboardPanel); break;
            }
        }
    }
} 