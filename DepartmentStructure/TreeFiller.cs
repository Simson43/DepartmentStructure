using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DepartmentStructure
{
    public class DepTreeFiller
    {
        public DepTreeFiller(TreeView tree)
        {
            this.tree = tree;
        }
        
        private TreeView tree;
        private List<DepartmentDTO> departments;
        
        public void Fill(List<DepartmentDTO> departments)
        {
            this.departments = departments;
            Fill(null, tree.Nodes);
        }
        
        private void Fill(Guid? id, TreeNodeCollection nodes)
        {
            var children = departments.FindAll(x => x.ParentDepartmentID == id);
            foreach (var child in children)
            {
                var node = new TreeNode(child.Name);
                nodes.Add(node);
                Fill(child.ID, node.Nodes);
            }
        }
    }
}
