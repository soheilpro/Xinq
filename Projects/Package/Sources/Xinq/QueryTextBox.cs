using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Xinq
{
    internal class QueryTextBox : TextBox
    {
        private const int WM_KEYDOWN = 0x100;
        private const int WM_CHAR = 0x102;
        private const int WM_CUT = 0x300;
        private const int WM_PASTE = 0x302;
        private const int WM_CLEAR = 0x303;

        public event CancelEventHandler TextChanging;

        protected override void WndProc(ref Message m)
        {
            var causesTextChange = false;
            Keys keyData;

            switch (m.Msg)
            {
                case WM_KEYDOWN:

                    keyData = (Keys)((int)((long)m.WParam)) | ModifierKeys;

                    if (keyData == Keys.Delete)
                        causesTextChange = true;

                    break;

                case WM_CHAR:

                    keyData = (Keys)((int)((long)m.WParam)) | ModifierKeys;

                    if (keyData != Keys.Escape)
                        causesTextChange = true;

                    break;

                case WM_CUT:
                case WM_PASTE:
                case WM_CLEAR:

                    causesTextChange = true;
                    break;
            }

            if (causesTextChange)
            {
                var e = new CancelEventArgs(false);

                if (TextChanging != null)
                    TextChanging(this, e);

                if (e.Cancel)
                    return;
            }

            base.WndProc(ref m);
        }
    }
}
