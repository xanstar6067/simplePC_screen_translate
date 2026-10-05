using System.Drawing.Drawing2D;

namespace simplePC_screen_translate.UI;

public sealed class RegionSelectionForm : Form
{
    private readonly Bitmap _screenshot;
    private readonly Rectangle _screenBounds;
    private Point _start;
    private Point _end;
    private bool _dragging;
    public Rectangle SelectedBounds { get; private set; }

    public RegionSelectionForm(Bitmap screenshot, Rectangle screenBounds)
    {
        _screenshot = screenshot;
        _screenBounds = screenBounds;
        AutoScaleMode = AutoScaleMode.None;
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        Bounds = screenBounds;
        TopMost = true;
        ShowInTaskbar = false;
        DoubleBuffered = true;
        KeyPreview = true;
        Cursor = Cursors.Cross;
        Text = "Выберите область для перевода";
        AccessibleName = Text;
    }

    private Rectangle Selection => Rectangle.FromLTRB(Math.Min(_start.X, _end.X), Math.Min(_start.Y, _end.Y),
        Math.Max(_start.X, _end.X), Math.Max(_start.Y, _end.Y));

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.DrawImageUnscaled(_screenshot, Point.Empty);
        using var shade = new SolidBrush(Color.FromArgb(110, Color.Black));
        e.Graphics.FillRectangle(shade, ClientRectangle);
        var selection = Selection;
        if (selection.Width > 0 && selection.Height > 0)
        {
            e.Graphics.DrawImage(_screenshot, selection, selection, GraphicsUnit.Pixel);
            using var border = new Pen(SystemColors.Highlight, 2);
            e.Graphics.DrawRectangle(border, selection);
        }
        var monitor = Screen.FromPoint(Cursor.Position).Bounds;
        var origin = new Point(monitor.X - _screenBounds.X + 24, monitor.Y - _screenBounds.Y + 24);
        using var font = new Font("Segoe UI", 18, GraphicsUnit.Pixel);
        var instruction = "Выделите область мышью · Esc или правая кнопка — отмена";
        var size = e.Graphics.MeasureString(instruction, font);
        e.Graphics.FillRectangle(Brushes.Black, origin.X - 10, origin.Y - 8, size.Width + 20, size.Height + 16);
        e.Graphics.DrawString(instruction, font, Brushes.White, origin);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button == MouseButtons.Right) { DialogResult = DialogResult.Cancel; Close(); return; }
        if (e.Button != MouseButtons.Left) return;
        _dragging = true;
        _start = _end = e.Location;
        Capture = true;
        Invalidate();
    }
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (_dragging)
        {
            _end = new(Math.Clamp(e.X, 0, ClientSize.Width), Math.Clamp(e.Y, 0, ClientSize.Height));
            Invalidate();
        }
    }
    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (!_dragging || e.Button != MouseButtons.Left) return;
        _dragging = false;
        Capture = false;
        if (Selection.Width < 8 || Selection.Height < 8) { Invalidate(); return; }
        SelectedBounds = Selection;
        SelectedBounds = new(SelectedBounds.X + _screenBounds.X, SelectedBounds.Y + _screenBounds.Y,
            SelectedBounds.Width, SelectedBounds.Height);
        DialogResult = DialogResult.OK;
        Close();
    }
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape) { DialogResult = DialogResult.Cancel; Close(); }
        base.OnKeyDown(e);
    }
}
