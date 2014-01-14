using System;
using System.ComponentModel;
using System.Windows.Forms;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Xinq
{
    internal partial class XinqEditor : UserControl
    {
        private XinqPackage _package;
        private XinqEditorPane _editorPane;
        private uint _updateLevel;

        public XinqDocument Document
        {
            get;
            set;
        }

        public XinqEditor()
        {
            InitializeComponent();

            tsRemoveQuery.Enabled = false;
            txtQuery.Enabled = false;
        }

        public XinqEditor(XinqPackage package, XinqEditorPane editorPane) : this()
        {
            _package = package;
            _editorPane = editorPane;
        }

        public void LoadDocument()
        {
            _updateLevel++;

            dgQueries.Rows.Clear();

            foreach (var query in Document.Queries)
            {
                var row = new DataGridViewQueryRow(dgQueries, query);
                dgQueries.Rows.Add(row);
            }

            _updateLevel--;
        }

        public void ClearUndoStack()
        {
            txtQuery.ClearUndo();
        }

        public bool CanSelectAll()
        {
            var activeControl = SplitContainer.ActiveControl;

            if (activeControl is TextBoxBase)
                return true;

            return false;
        }

        public bool CanCut()
        {
            var activeControl = SplitContainer.ActiveControl;

            if (activeControl is DataGridView)
            {
                var currentGridView = (DataGridView)activeControl;
                var currentCell = currentGridView.CurrentCell;
                var currentColumn = currentCell.OwningColumn;

                if (!string.IsNullOrEmpty((string)currentCell.Value))
                    if (currentColumn == dgcComment)
                        return true;
            }

            if (activeControl is TextBoxBase)
                if (((TextBoxBase)activeControl).SelectionLength > 0)
                    return true;

            return false;
        }

        public bool CanCopy()
        {
            var activeControl = SplitContainer.ActiveControl;

            if (activeControl is DataGridView)
            {
                var currentGridView = (DataGridView)activeControl;
                var currentCell = currentGridView.CurrentCell;

                if (!string.IsNullOrEmpty((string)currentCell.Value))
                    return true;
            }

            if (activeControl is TextBoxBase)
                if (((TextBoxBase)activeControl).SelectionLength > 0)
                    return true;

            return false;
        }

        public bool CanPaste()
        {
            var activeControl = SplitContainer.ActiveControl;

            if (activeControl is DataGridView)
                return true;

            if (activeControl is TextBoxBase)
                if (Clipboard.ContainsText())
                    return true;

            return false;
        }

        public void OnSelectAll()
        {
            var activeControl = SplitContainer.ActiveControl;

            if (activeControl is TextBoxBase)
            {
                ((TextBoxBase)activeControl).SelectAll();

                return;
            }

            throw new NotImplementedException();
        }

        public void OnCut()
        {
            var activeControl = SplitContainer.ActiveControl;

            if (activeControl is DataGridView)
            {
                var currentGridView = (DataGridView)activeControl;
                var currentCell = currentGridView.CurrentCell;

                Clipboard.SetText((string)currentCell.Value);
                currentCell.Value = null;

                return;
            }

            if (activeControl is TextBoxBase)
            {
                ((TextBoxBase)activeControl).Cut();

                return;
            }

            throw new NotImplementedException();
        }

        public void OnCopy()
        {
            var activeControl = SplitContainer.ActiveControl;

            if (activeControl is DataGridView)
            {
                var currentGridView = (DataGridView)activeControl;
                var currentCell = currentGridView.CurrentCell;

                Clipboard.SetText((string)currentCell.Value);

                return;
            }

            if (activeControl is TextBoxBase)
            {
                ((TextBoxBase)activeControl).Copy();

                return;
            }

            throw new NotImplementedException();
        }

        public void OnPaste()
        {
            var activeControl = SplitContainer.ActiveControl;

            if (activeControl is DataGridView)
            {
                var currentGridView = (DataGridView)activeControl;
                var currentCell = currentGridView.CurrentCell;
                var currentColumn = currentCell.OwningColumn;
                var text = Clipboard.GetText();

                if (currentColumn == dgcName)
                {
                    if (Document.Queries[text] != null)
                    {
                        MessageBox.Show(_package.GetResourceString(121), _package.GetResourceString(110), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                currentCell.Value = text;
                dgQueries_CellEndEdit(null, new DataGridViewCellEventArgs(currentCell.ColumnIndex, currentCell.RowIndex));

                return;
            }

            if (activeControl is TextBoxBase)
            {
                ((TextBoxBase)activeControl).Paste();

                return;
            }

            throw new NotImplementedException();
        }

        public void CommitPendingEdit()
        {
            var activeControl = SplitContainer.ActiveControl;

            if (activeControl is IDataGridViewEditingControl)
            {
                var currentGridView = ((IDataGridViewEditingControl)activeControl).EditingControlDataGridView;
                var currentCell = currentGridView.CurrentCell;
                var currentColumn = currentCell.OwningColumn;
                var currentRow = (DataGridViewQueryRow)currentCell.OwningRow;

                if (currentCell.IsInEditMode)
                {
                    if (currentColumn == dgcName)
                    {
                        var name = (activeControl).Text;

                        if (name.Trim().Length == 0)
                        {
                            currentGridView.CancelEdit();
                        }
                        else
                        {
                            var existingQuery = Document.Queries[name];

                            if (existingQuery != null && existingQuery != currentRow.Query)
                                currentGridView.CancelEdit();
                        }
                    }
                    else
                    {
                        currentGridView.EndEdit();
                    }
                }
            }
        }

        private void tsAddQuery_Click(object sender, EventArgs e)
        {
            if (!_editorPane.CanEdit())
                return;

            var newQuery = new Query(Document);
            var nameSuffix = 1;

            while (true)
            {
                var name = string.Format("MyQuery{0}", nameSuffix++);

                if (Document.Queries[name] == null)
                {
                    newQuery.Name = name;
                    break;
                }
            }

            Document.Queries.Add(newQuery);

            var newRow = new DataGridViewQueryRow(dgQueries, newQuery);
            dgQueries.Rows.Add(newRow);

            newRow.Selected = true;
            dgQueries.CurrentCell = newRow.Cells[0];
            dgQueries.BeginEdit(true);
        }

        private void tsRemoveQuery_Click(object sender, EventArgs e)
        {
            if (_updateLevel > 0)
                return;

            if (MessageBox.Show(_package.GetResourceString(122), _package.GetResourceString(110), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            if (!_editorPane.CanEdit())
                return;

            dgQueries.Rows.Remove(dgQueries.CurrentRow);
        }

        private void dgQueries_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (!_editorPane.CanEdit())
                e.Cancel = true;
        }

        private void dgQueries_SelectionChanged(object sender, EventArgs e)
        {
            _updateLevel++;

            if (dgQueries.SelectedRows.Count > 0)
            {
                var row = (DataGridViewQueryRow)dgQueries.SelectedRows[0];

                txtQuery.Text = row.Query.Text;
                txtQuery.Enabled = true;
            }
            else
            {
                txtQuery.Clear();
                txtQuery.Enabled = false;
            }

            _updateLevel--;
        }

        private void dgQueries_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var row = (DataGridViewQueryRow)dgQueries.Rows[e.RowIndex];
            var column = dgQueries.Columns[e.ColumnIndex];
            var cell = row.Cells[e.ColumnIndex];

            if (row.IsNewRow)
                return;

            if (!cell.IsInEditMode)
                return;

            if (column == dgcName)
            {
                var name = (string)e.FormattedValue;

                if (name.Trim().Length == 0)
                {
                    MessageBox.Show(_package.GetResourceString(120), _package.GetResourceString(110), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
                else
                {
                    var existingQuery = Document.Queries[name];

                    if (existingQuery != null && existingQuery != row.Query)
                    {
                        MessageBox.Show(_package.GetResourceString(121), _package.GetResourceString(110), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }
            }
        }

        private void dgQueries_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (_updateLevel > 0)
                return;

            var row = (DataGridViewQueryRow)dgQueries.Rows[e.RowIndex];
            var column = dgQueries.Columns[e.ColumnIndex];
            var cell = row.Cells[e.ColumnIndex];

            var text = ((string)cell.Value).Trim();
            cell.Value = text;

            if (column == dgcName)
            {
                row.Query.Name = text;
                return;
            }

            if (column == dgcComment)
            {
                row.Query.Comment = text;
                return;
            }
        }

        private void dgQueries_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show(_package.GetResourceString(122), _package.GetResourceString(110), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            if (!_editorPane.CanEdit())
            {
                e.Cancel = true;
                return;
            }
        }

        private void dgQueries_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            tsRemoveQuery.Enabled = dgQueries.Rows.Count > 0;
        }

        private void dgQueries_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (_updateLevel > 0)
                return;

            Document.Queries.RemoveAt(e.RowIndex);
            tsRemoveQuery.Enabled = dgQueries.Rows.Count > 0;
        }

        private void txtQuery_TextChanging(object sender, CancelEventArgs e)
        {
            e.Cancel = (!_editorPane.CanEdit());
        }

        private void txtQuery_TextChanged(object sender, EventArgs e)
        {
            if (_updateLevel > 0)
                return;

            var row = (DataGridViewQueryRow)dgQueries.CurrentRow;
            row.Query.Text = txtQuery.Text;
        }
    }

    internal class DataGridViewQueryRow : DataGridViewRow
    {
        private Query _query;

        public Query Query
        {
            get
            {
                return _query;
            }
        }

        public DataGridViewQueryRow(DataGridView dataGridView, Query query)
        {
            CreateCells(dataGridView, query.Name, query.Comment);

            _query = query;
        }
    }
}
