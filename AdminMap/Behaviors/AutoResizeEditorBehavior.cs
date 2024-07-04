using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMap.Behaviors;

public class AutoResizeEditorBehavior : Behavior<Editor>
{
    protected override void OnAttachedTo(Editor bindable)
    {
        base.OnAttachedTo(bindable);
        bindable.TextChanged += OnEditorTextChanged;
    }

    protected override void OnDetachingFrom(Editor bindable)
    {
        base.OnDetachingFrom(bindable);
        bindable.TextChanged -= OnEditorTextChanged;
    }

    private void OnEditorTextChanged(object sender, TextChangedEventArgs e)
    {
        var editor = sender as Editor;
        if (editor != null)
        {
            editor.HeightRequest = -1;
            var height = editor.Measure(double.PositiveInfinity, double.PositiveInfinity).Request.Height;
            editor.HeightRequest = height;
        }
    }
}
