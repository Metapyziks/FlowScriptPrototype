using System.Windows.Forms;

namespace FlowScriptPrototype
{
    class EditorPanel : Panel
    {
        public EditorPanel()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true);
        }
    }
}
