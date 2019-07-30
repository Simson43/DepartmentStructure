using System.Windows.Forms;

namespace DepartmentStructure.Extension
{
    public static class TreeNodeExtension
    {
        public static bool IsItselfOrChildOrForefatherOf(this TreeNode node1, TreeNode node2)
        {
            return node1 == node2 || node1.Parent == node2 || node1.IsParentOf(node2);
        }

        private static bool IsParentOf(this TreeNode parentNode, TreeNode treeNode)
        {
            return treeNode != null && (parentNode == treeNode.Parent || 
                parentNode.IsParentOf(treeNode.Parent));
        }
    }
}
