using DepartmentStructure.Extension;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DepartmentStructure
{
    public partial class DepartmentsForm : Form
    {
        private ViewModel viewModel { get; }
        private bool editMode => editedRowIndex >= 0;
        private int editedRowIndex = -1;
        private TreeNode lastOverNode;
        private TreeNode showedNode;

        public DepartmentsForm(ViewModel viewModel)
        {
            InitializeComponent();
            InitializeDragDrop();
            InitializeButtons();
            this.viewModel = viewModel;
            dataGridView.DataSource = viewModel.ShowedEmployees;
            viewModel.FillTreeFromDB(treeView);
            
            dataGridView.SelectionChanged += DataGridView_SelectionChanged;
            treeView.AfterSelect += TreeView_AfterSelect;
            treeView.AfterLabelEdit += TreeView_AfterLabelEdit;
            treeView.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                    treeView.SelectedNode = treeView.GetNodeAt(e.Location);
            };
            treeView.ContextMenu = new ContextMenu(new MenuItem[]
            {
                new MenuItem("Добавить подотдел", (s, e) => AddSubDepatrment()),
                new MenuItem("Удалить", (s, e) => RemoveDepatrment())
            });
        }

        private void InitializeDragDrop()
        {
            treeView.DragOver += (s, e) => HighlightDepartmentNode(e);
            treeView.DragDrop += (s, e) => ReplaceItem(e);

            treeView.ItemDrag += (s, e) =>
            {
                if (e.Button == MouseButtons.Left && !editMode)
                    DoDragDrop(e.Item, DragDropEffects.Move);
            };

            dataGridView.CellMouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left && e.Clicks == 1 && e.RowIndex >=0 && !editMode)
                {
                    dataGridView.Rows[e.RowIndex].Selected = true;
                    DoDragDrop(dataGridView.Rows[e.RowIndex], DragDropEffects.Move);
                }
            };

            treeView.DragLeave += (s, e) =>
            {
                if (lastOverNode != null)
                    lastOverNode.BackColor = Color.White;
            };
        }

        private void InitializeButtons()
        {
            editButton.Click += EditEmployeeButton_Click;
            applyButton.Click += ApplyEmployeeButton_Click;
            cancelEditButton.Click += (s, e) => CancelEditMode();
            addButton.Click += AddEmployeeButton_Click;
            removeButton.Click += RemoveEmployeeButton_Click;
            closeButton.Click += (s, e) => Close();
        }
        
        private void AddSubDepatrment()
        {
            if (editMode)
                CancelEditMode();
            var node = treeView.SelectedNode;
            var department = viewModel.AddDepartment(node?.Text);
            if (node != null)
                node.Nodes.Add(department.Name);
            else
                treeView.Nodes.Add(department.Name);
        }

        private void RemoveDepatrment()
        {
            if (editMode)
                CancelEditMode();
            var node = treeView.SelectedNode;
            if (node == null || DialogResult.No ==
                MessageBox.Show($"Вы действительно хотите удалить этот отдел, его подотделы и сотрудников:\n" +
                $"{node.Text}?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                return;
            viewModel.RemoveDepartment(node.Text);
            node.Remove();
        }

        private void EditEmployeeButton_Click(object sender, EventArgs e)
        {
            if (editMode)
                throw new InvalidOperationException();
            if (dataGridView.SelectedRows.Count == 0)
                return;
            OnEditMode(dataGridView.SelectedRows[0].Index);
        }

        private void ApplyEmployeeButton_Click(object sender, EventArgs e)
        {
            var rowIndex = dataGridView.SelectedCells[0].RowIndex;
            if (!((EmployeeDTO)dataGridView.Rows[rowIndex].DataBoundItem).IsValid(out string message))
            {
                MessageBox.Show(message, "Предупреждение!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            OffEditMode();//Сначала нужно отключить, иначе StackOverflowException
            viewModel.UpdateEmployee(rowIndex);
        }

        private void AddEmployeeButton_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null)
                return;
            viewModel.CreateEmployee(treeView.SelectedNode.Text);
            OnEditMode(dataGridView.RowCount - 1);
        }

        private void RemoveEmployeeButton_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0 || dataGridView.SelectedRows[0].DataBoundItem is EmployeeDTO employeeDTO &&
                DialogResult.No == MessageBox.Show($"Вы действительно хотите удалить этого сотрудника:\n" +
                $"{employeeDTO.Position} - {employeeDTO.SurName} {employeeDTO.FirstName} " +
                $"{(!string.IsNullOrWhiteSpace(employeeDTO.Patronymic) ? employeeDTO.Patronymic : "")} ?",
                "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                return;
            viewModel.RemoveEmployee(dataGridView.SelectedRows[0].Index);
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (editMode)
                OffEditMode();
            viewModel.ShowEmployeesFromTheDepartment(e.Node.Text);
            showedNode = e.Node;
        }
        
        private void DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (editMode && dataGridView.SelectedCells.Count > 0 && dataGridView.SelectedCells[0].RowIndex != editedRowIndex)
                CancelEditMode();
        }

        private void TreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label == null || e.Node.Text == e.Label) // Если Label не изменен, он null
                return;
            if (!LabelIsValid(e.Label, out string message))
            {
                MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.CancelEdit = true;
                return;
            }
            viewModel.UpdateDepartmentName(e.Node.Text, e.Label);
        }

        private static bool LabelIsValid(string name, out string message)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                message = "Название отдела не может быть пустым";
                return false;
            }
            else if (name.All(x => char.IsDigit(x)))
            {
                message = "Название отдела должно содержать буквы";
                return false;
            }
            else if (char.IsDigit(name[0]) || char.IsLower(name[0])) 
            {
                message = "Название отдела должно начинаться с прописной буквы";
                return false;
            }
            message = null;
            return true;
        }

        private void CancelEditMode()
        {
            OffEditMode();
            viewModel.ShowEmployeesFromTheDepartment(treeView.SelectedNode.Text);// Вернуть значения полей из БД
        }

        private void OnEditMode(int rowIndex)
        {
            editedRowIndex = rowIndex;
            dataGridView.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridView.Rows[rowIndex].Cells[0].Selected = true;
            editButton.Visible =
                addButton.Visible =
                removeButton.Visible = false;
            applyButton.Visible =
                cancelEditButton.Visible = true;
        }

        private void OffEditMode()
        {
            editedRowIndex = -1;
            dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            editButton.Visible =
                addButton.Visible =
                removeButton.Visible = true;
            applyButton.Visible =
                cancelEditButton.Visible = false;
        }
        
        private void HighlightDepartmentNode(DragEventArgs e)
        {
            if (e.AllowedEffect == DragDropEffects.Move)
            {
                var overNode = treeView.GetNodeAt(treeView.PointToClient(new Point(e.X, e.Y)));
                if (lastOverNode == overNode)
                    return;
                if (lastOverNode != null)
                    lastOverNode.BackColor = Color.White;
                lastOverNode = overNode;
                if (e.Data.GetDataPresent(typeof(DataGridViewRow)) && showedNode == overNode ||
                    e.Data.GetDataPresent(typeof(TreeNode)) && 
                    ((TreeNode)e.Data.GetData(typeof(TreeNode))).IsItselfOrChildOrForefatherOf(overNode)) 
                {
                    e.Effect = DragDropEffects.None;
                    return;
                }
                if (overNode != null)
                {
                    e.Effect = e.AllowedEffect;
                    overNode.BackColor = Color.LightBlue;
                }
                else
                    e.Effect = e.Effect = DragDropEffects.None;
            }
        }

        private void ReplaceItem(DragEventArgs e)
        {
            if (e.Effect != DragDropEffects.Move)
                return;
            var newDepartmentName = lastOverNode.Text;
            if (e.Data.GetDataPresent(typeof(DataGridViewRow)))
            {
                var row = e.Data.GetData(typeof(DataGridViewRow)) as DataGridViewRow;
                var employeeDTO = (EmployeeDTO)row.DataBoundItem;
                if (DialogResult.No == MessageBox.Show($"{employeeDTO.Position} - " +
                    $"{employeeDTO.SurName} {employeeDTO.FirstName} " +
                    $"{(!string.IsNullOrEmpty(employeeDTO.Patronymic) ? employeeDTO.Patronymic : "")}\n" +
                    $"в\n{newDepartmentName}?", "Переместить", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    return;
                viewModel.ReplaceEmployee((EmployeeDTO)row.DataBoundItem, newDepartmentName);
                viewModel.ShowEmployeesFromTheDepartment(showedNode.Text);
            }
            else if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                var node = e.Data.GetData(typeof(TreeNode)) as TreeNode;
                if (MessageBox.Show($"{node.Text} (вместе с подотделами)\nв\n{newDepartmentName}?",
                    "Переместить", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) 
                    return;
                viewModel.ReplaceDepartment(node.Text, newDepartmentName);
                node.Remove();
                lastOverNode.Nodes.Add(node);
            }
            lastOverNode.BackColor = Color.White;
        }
    }
}
